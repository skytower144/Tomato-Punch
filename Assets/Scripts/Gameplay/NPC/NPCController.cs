using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringSpriteanim : SerializableDictionary<string, SpriteAnimation>{}

public class NPCController : MonoBehaviour, Interactable
{
    [Header("Ink JSON")]
    [SerializeField] private string inkFileName;
    //[SerializeField] private TextAsset inkJsonData;

    [Header("Graphic Control")]
    [SerializeField] private SpriteRenderer sprite_renderer;
    [SerializeField] private StringSpriteanim sprite_dict= new StringSpriteanim();
    private SpriteAnimator spriteAnimator;

    [SerializeField] private bool fixedFacingDirection;

    [HideInInspector] public bool banInteractDirection;
    [HideInInspector] public bool lock_u, lock_ru, lock_r, lock_rd, lock_d, lock_ld, lock_l, lock_lu;
    
    private void Start()
    {
        if (sprite_renderer == null)
            sprite_renderer = GetComponent<SpriteRenderer>();
        Play("idle");

        AnimManager.instance.npc_dict[gameObject.name] = this;
    }

    private void Update()
    {
        spriteAnimator.Animate();
    }

    public void Interact()
    {
        if (ValidInteractDirection())
        {
            FacePlayer();
            TextAsset inkJsonData = Resources.Load<TextAsset>($"Dialogue/{UIControl.currentLangMode}/{gameObject.scene.name}/{gameObject.name}/{inkFileName}");
            DialogueManager.instance.EnterDialogue(inkJsonData);
        }
    }

    private void FacePlayer()
    {
        if (!fixedFacingDirection)
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

    public void Play(string animTag)
    {
        if (sprite_dict.ContainsKey(animTag))
        {
            SpriteAnimation animation = sprite_dict[animTag];
            spriteAnimator = new SpriteAnimator(sprite_renderer, animation.sprites, animation.fps, animation.is_loop);
        }

        else
        {
            List<Sprite> singleSprite = new List<Sprite>();
            singleSprite.Add(sprite_renderer.sprite);
            spriteAnimator = new SpriteAnimator(sprite_renderer, singleSprite, 0, false);
        }   
    }

    private bool ValidInteractDirection()
    {
        PlayerMovement playerMovement = PlayerMovement.instance;
        bool finalFlag = true;

        if (lock_u && playerMovement.CheckFacingDirection("UP"))
            finalFlag = false;
        
        if (lock_ru && playerMovement.CheckFacingDirection("RU") )
            finalFlag = false;
        
        if (lock_r && playerMovement.CheckFacingDirection("RIGHT") )
            finalFlag = false;
        
        if (lock_rd && playerMovement.CheckFacingDirection("RD"))
            finalFlag = false;
        
        if (lock_d && playerMovement.CheckFacingDirection("DOWN"))
            finalFlag = false;
        
        if (lock_ld && playerMovement.CheckFacingDirection("LD"))
            finalFlag = false;

        if (lock_l && playerMovement.CheckFacingDirection("LEFT"))
            finalFlag = false;
        
        if (lock_lu && playerMovement.CheckFacingDirection("LU"))
            finalFlag = false;

        return finalFlag;
    }

}

[System.Serializable]
public class SpriteAnimation
{
    public bool is_loop;
    public int fps;
    public List<Sprite> sprites;
}
