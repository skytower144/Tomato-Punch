using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class StringSpriteanim : SerializableDictionary<string, SpriteAnimation>{}

public class NPCController : MonoBehaviour, Interactable, ObjectProgress, Character
{
    [Header("[ SAVING ]")]
    [SerializeField] private bool isSaveTarget = true;
    [SerializeField] private bool isPartyCandidate = false;

    [Header("[ Ink JSON ]")]
    [SerializeField] private string inkFileName;

    [Header("[ Reserved Key Events ]")]
    [SerializeField] private List<KeyEventProgressData> keyEventProgressList;
    private string cacheDialogueFile;

    private bool isInteractAnimating = false;
    [System.NonSerialized] public string idleAnimation = "idle";

    [Header("[ Graphic Control ]")]
    [SerializeField] private SpriteRenderer sprite_renderer;
    [SerializeField] private StringSpriteanim sprite_dict= new StringSpriteanim();
    private SpriteAnimator spriteAnimator;

    [Header("[ Player Viewpoint Standard ]")]
    [SerializeField] private bool isFixedSprite;

// DO NOT CHANGE [HideInInspector] ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [HideInInspector] public bool isUniqueID;
    [HideInInspector] public string npcID;

    [HideInInspector] public bool willMove;
    [HideInInspector] public NPCMove npcMove;
    [HideInInspector] public BoxCollider2D npcCollider;

    [HideInInspector] public bool disableSpriteAnimator;
    [HideInInspector] public Animator npcAnim;

    [HideInInspector] public bool hasNoDialogue;
    [HideInInspector] public string interactAnimation;

    [HideInInspector] public bool banInteractDirection;
    [HideInInspector] public bool lock_u, lock_ru, lock_r, lock_rd, lock_d, lock_ld, lock_l, lock_lu;

    [HideInInspector] public bool canBattle;
    [HideInInspector] public bool instantBattle;
    [HideInInspector] public EnemyBase enemyData;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private bool initOnce = false;
    private bool isAnimating = false;

    public string ReturnID()
    {
        if (isUniqueID) return npcID;
        return $"{gameObject.scene.name}_{gameObject.name}";
    }

    private void OnEnable()
    {
        if (initOnce) return;
        initOnce = true;

        if (sprite_renderer == null)
            sprite_renderer = GetComponent<SpriteRenderer>();
        
        if (!disableSpriteAnimator)
            Play(idleAnimation);
        
        NPCManager.instance.npc_dict[ReturnID()] = this;

        if (!isSaveTarget) return;
        string sceneName = isPartyCandidate ? GameManager.gm_instance.partyManager.candidateControl.sceneName : gameObject.scene.name;
        
        if (!ProgressManager.instance.assistants[sceneName].objectProgressList.Contains(this))
            ProgressManager.instance.assistants[sceneName].objectProgressList.Add(this);
    }
    void OnDestroy()
    {
        if (NPCManager.instance != null && NPCManager.instance.npc_dict.ContainsKey(ReturnID()))
            NPCManager.instance.npc_dict.Remove(ReturnID());
    }

    private void Update()
    {
        if (!disableSpriteAnimator)
            spriteAnimator.Animate();
    }

    public void Interact()
    {
        if (ValidInteractDirection() && !isInteractAnimating)
        {
            if (hasNoDialogue && !string.IsNullOrEmpty(interactAnimation))
            {
                isInteractAnimating = true;
                StartCoroutine(PlayInteractAnimation());
            }
            else if (instantBattle)
            {
                GameManager.gm_instance.battle_system.StartBattle(enemyData);
            }
            else
            {
                InitiateTalk();
            }
        }
    }

    private void FacePlayer()
    {
        if (!isFixedSprite)
        {
            PlayerMovement playerMovement = PlayerMovement.instance;
            
            if (playerMovement.CheckFacingDirection("DOWN") || playerMovement.CheckFacingDirection("LD") || playerMovement.CheckFacingDirection("RD"))
                Turn("up");
            
            else if (playerMovement.CheckFacingDirection("UP") || playerMovement.CheckFacingDirection("LU") || playerMovement.CheckFacingDirection("RU"))
                Turn("down");
            
            else if (playerMovement.CheckFacingDirection("RIGHT"))
                Turn("left");
            
            else if (playerMovement.CheckFacingDirection("LEFT"))
                Turn("right");
        }
    }

    public void InitiateTalk()
    {
        FacePlayer();
        GameManager.gm_instance.playerKeyEventManager.CheckProgressKeyEvent(this);
        TextAsset inkJsonData = InkDB.ReturnTextAsset(gameObject.scene.name, gameObject.name, inkFileName, isUniqueID);
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

    public void Play(string clipName, Action dialogueAction = null, bool stopAfterAnimation = false)
    {
        if (disableSpriteAnimator && gameObject.activeSelf && npcAnim.gameObject.activeSelf) {
            if (CutsceneHandler.DoesAnimationExist(npcAnim, clipName))
                npcAnim.Play(clipName, -1, 0f);

            if (dialogueAction != null) {
                AnimationClip clip = CutsceneHandler.ReturnAnimationClip(npcAnim, clipName);
                StartCoroutine(DialogueManager.instance.DialogueAction(npcAnim, dialogueAction, clip.length, stopAfterAnimation));
            }
        }
        else if (sprite_dict.ContainsKey(clipName))
        {
            SpriteAnimation animation = sprite_dict[clipName];
            spriteAnimator = new SpriteAnimator(this, sprite_renderer, animation.sprites, animation.fps, animation.is_loop, dialogueAction, stopAfterAnimation);
        }
        else
        {
            List<Sprite> singleSprite = new List<Sprite> { sprite_renderer.sprite };
            spriteAnimator = new SpriteAnimator(this, sprite_renderer, singleSprite, 0, false);
        }
    }

    IEnumerator PlayInteractAnimation()
    {
        Play("interact");
        yield return WaitForCache.GetWaitForSecond(2f);
        Play("idle");
        isInteractAnimating = false;
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
        float input_z = gameObject.transform.localPosition.z;
        gameObject.transform.localPosition = new Vector3(input_x, input_y, input_z);
    }
    public ProgressData Capture()
    {
        ProgressData game_data = new ProgressData
        {
            InkFileName = inkFileName,
            AnimationState = idleAnimation,
            IsVisible = gameObject.activeSelf,
            Position = transform.position,
            KeyEventList = keyEventProgressList
        };
        if (disableSpriteAnimator && npcAnim != null)
            game_data.FacingDir = new Vector2(Mathf.Round(npcAnim.GetFloat("moveX")), Mathf.Round(npcAnim.GetFloat("moveY")));
        
        return game_data;
    }

    public void Restore(ProgressData game_data)
    {
        LoadNextDialogue(game_data.InkFileName);
        idleAnimation = game_data.AnimationState;
        transform.position = game_data.Position;
        keyEventProgressList = game_data.KeyEventList;

        if (disableSpriteAnimator && npcAnim != null) {
            npcAnim.SetFloat("moveX", game_data.FacingDir.x);
            npcAnim.SetFloat("moveY", game_data.FacingDir.y);
        }
        gameObject.SetActive(game_data.IsVisible);
        Play(idleAnimation);
    }
    public void ApplyKeyEvent(KeyEventProgressData data)
    {
        if (data.ShowInkFileName)
            LoadNextDialogue(data.InkFileName);
        
        if (data.ShowPosition)
            transform.position = data.Position;
        
        if (data.ShowIsVisible)
            gameObject.SetActive(data.IsVisible);
        
        if (data.ShowAnimationState) {
            idleAnimation = data.AnimationState;
            Play(idleAnimation);
        }
        if (data.ShowFacingDir)
            Turn(data.FacingDir);
    }
    public List<KeyEventProgressData> ReturnKeyEventProgressList()
    {
        return keyEventProgressList;
    }
    public void SetIsAnimating(bool state)
    {
        isAnimating = state;
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    public Animator UsesDefaultAnimator()
    {
        return npcAnim;
    }

    public IEnumerator PlayMoveActions(string[] posStrings, float moveSpeed, bool isAnimate)
    {
        string[] posString;
        float originalSpeed = npcMove.FollowSpeed;
        
        npcCollider.enabled = false;

        if (moveSpeed != -1f)
            npcMove.SetFollowSpeed(moveSpeed);

        foreach (string xy in posStrings) {
            posString = xy.Split('~');
            float x = posString[0] == "_" ? transform.position.x : float.Parse(posString[0]);
            float y = posString[1] == "_" ? transform.position.y : float.Parse(posString[1]);
            float z = transform.position.z;
            Vector3 targetPos = new Vector3(x, y, z);

            while ((targetPos - transform.position).magnitude >= 0.01f) {
                yield return npcMove.Move(targetPos, isAnimate);
            }
            transform.position = new Vector3(x, y, transform.position.z);
        }
        npcMove.SetFollowSpeed(originalSpeed);
        npcMove.Animate(false, default, false);
        npcCollider.enabled = true;
    }

    public void Turn(string direction)
    {
        if (disableSpriteAnimator)
            DialogueManager.instance.cutsceneHandler.FaceAdjustment(npcAnim, direction.ToUpper());
        else
            Play($"{direction.ToLower()}");
    }
}

[System.Serializable]
public class SpriteAnimation
{
    public bool is_loop;
    public int fps;
    public List<Sprite> sprites;
}

public enum PlayerReviveState { Cafe, LoseTalk, Bench }
