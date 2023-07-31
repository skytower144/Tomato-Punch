using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class StringSpriteanim : SerializableDictionary<string, SpriteAnimation>{}

public class NPCController : MonoBehaviour, Interactable, ObjectProgress
{

    [Header("[ Ink JSON ]")]
    [SerializeField] private string inkFileName;
    [SerializeField] private List<KeyEventDialogue> keyEventDialogues;
    private string cacheDialogueFile;

    [Header("[ Dialogueless ]")]
    [SerializeField] private string interactAnimation;
    [System.NonSerialized] public string idleAnimation = "idle";

    [Header("[ Graphic Control ]")]
    [SerializeField] private SpriteRenderer sprite_renderer;
    [SerializeField] private StringSpriteanim sprite_dict= new StringSpriteanim();
    private SpriteAnimator spriteAnimator;
    private bool isInteractAnimating = false;

// DO NOT CHANGE [HideInInspector] ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [Header("[ Player Viewpoint Standard ]")]
    [SerializeField] private bool isFixedSprite;
    [SerializeField] private bool disableSpriteAnimator;
    [HideInInspector] public bool banInteractDirection;
    [HideInInspector] public bool lock_u, lock_ru, lock_r, lock_rd, lock_d, lock_ld, lock_l, lock_lu;

    [HideInInspector] public bool canBattle;
    [HideInInspector] public bool instantBattle;
    [HideInInspector] public EnemyBase enemyData;
    [HideInInspector] public PlayerReviveState reviveState;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public bool isDisabled = false;

    private void Start()
    {
        if (sprite_renderer == null)
            sprite_renderer = GetComponent<SpriteRenderer>();
        Play("idle");

        AnimManager.instance.npc_dict[ReturnID()] = this;
    }

    private void Update()
    {
        if (!disableSpriteAnimator)
            spriteAnimator.Animate();
    }

    public void Interact()
    {
        if (ValidInteractDirection())
        {
            if (!isInteractAnimating)
            {
                if (!String.IsNullOrEmpty(interactAnimation))
                {
                    isInteractAnimating = true;
                    StartCoroutine(PlayInteractAnimation());
                }

                if (instantBattle)
                {
                    StartBattle(enemyData);
                }
                else if (!String.IsNullOrEmpty(inkFileName))
                {
                    InitiateTalk();
                }
            }
            else
                return;
        }
    }

    private void FacePlayer()
    {
        if (!isFixedSprite)
        {
            PlayerMovement playerMovement = PlayerMovement.instance;
            
            if ((playerMovement.CheckFacingDirection("DOWN")) || (playerMovement.CheckFacingDirection("LD")) || playerMovement.CheckFacingDirection("RD"))
                Play("up");
            
            else if ((playerMovement.CheckFacingDirection("UP")) || (playerMovement.CheckFacingDirection("LU")) || playerMovement.CheckFacingDirection("RU"))
                Play("down");
            
            else if (playerMovement.CheckFacingDirection("RIGHT"))
                Play("left");
            
            else if (playerMovement.CheckFacingDirection("LEFT"))
                Play("right");
        }
    }

    public void InitiateTalk()
    {
        FacePlayer();
        GameManager.gm_instance.playerKeyEventManager.CheckPlayerKeyEvent(this, keyEventDialogues);
        TextAsset inkJsonData = Resources.Load<TextAsset>($"Dialogue/{UIControl.currentLangMode}/{gameObject.scene.name}/{gameObject.name}/{inkFileName}");
        DialogueManager.instance.EnterDialogue(inkJsonData, this);
    }

    public void LoadNextDialogue(string nextFileName)
    {
        if (nextFileName == "_") nextFileName = "";
        cacheDialogueFile = inkFileName;
        inkFileName = nextFileName;
    }

    public void RollbackDialogue()
    {
        inkFileName = cacheDialogueFile;
    }
    
    public void ChangeIdleAnimation(string changed)
    {
        idleAnimation = changed;
    }

    public void Play(string animTag, Action dialogueAction = null, bool stopAfterAnimation = false)
    {
        if (sprite_dict.ContainsKey(animTag))
        {
            SpriteAnimation animation = sprite_dict[animTag];
            spriteAnimator = new SpriteAnimator(this, sprite_renderer, animation.sprites, animation.fps, animation.is_loop, dialogueAction, stopAfterAnimation);
        }
        else
        {
            List<Sprite> singleSprite = new List<Sprite>();
            singleSprite.Add(sprite_renderer.sprite);
            spriteAnimator = new SpriteAnimator(this, sprite_renderer, singleSprite, 0, false);
        }   
    }

    IEnumerator PlayInteractAnimation()
    {
        Play("interact");
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(2f));
        Play("idle");
        isInteractAnimating = false;
    }

    public void StartBattle(EnemyBase enemy_data)
    {
        CustomBattleMode custom_mode = gameObject.GetComponent<CustomBattleMode>();
        if (custom_mode != null)
            custom_mode.ChangeBattleMode();
        
        GameManager.gm_instance.Initiate_Battle(enemy_data, reviveState);
    }

    private bool ValidInteractDirection()
    {
        PlayerMovement playerMovement = PlayerMovement.instance;
        bool finalFlag = true;

        if (
            (lock_u && playerMovement.CheckFacingDirection("UP")) ||
            (lock_ru && playerMovement.CheckFacingDirection("RU")) ||
            (lock_r && playerMovement.CheckFacingDirection("RIGHT")) ||
            (lock_rd && playerMovement.CheckFacingDirection("RD")) ||
            (lock_d && playerMovement.CheckFacingDirection("DOWN")) ||
            (lock_ld && playerMovement.CheckFacingDirection("LD")) ||
            (lock_l && playerMovement.CheckFacingDirection("LEFT")) ||
            (lock_lu && playerMovement.CheckFacingDirection("LU"))
            )
        {
            finalFlag = false;
        }

        return finalFlag;
    }

    public void Teleport(float input_x, float input_y)
    {
        float input_z = this.gameObject.transform.localPosition.z;
        this.gameObject.transform.localPosition = new Vector3(input_x, input_y, input_z);
    }

    public ProgressData Capture()
    {
        ProgressData game_data = new ProgressData();
        game_data.string_value_0 = inkFileName;
        game_data.bool_value_0 = isDisabled;
        game_data.keyEventDialogues = keyEventDialogues;

        return game_data;
    }

    public void Restore(ProgressData game_data)
    {
        inkFileName = game_data.string_value_0;
        isDisabled = game_data.bool_value_0;
        gameObject.SetActive(!isDisabled);
        keyEventDialogues = game_data.keyEventDialogues;
    }

    public string ReturnID()
    {
        return $"{gameObject.scene.name}_{gameObject.name}";
    }
}

[System.Serializable]
public class SpriteAnimation
{
    public bool is_loop;
    public int fps;
    public List<Sprite> sprites;
}

public enum PlayerReviveState { LoseTalk, Cafe, Bench }
