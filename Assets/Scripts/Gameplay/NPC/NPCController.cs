using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [Header("Ink JSON")]
    [SerializeField] private string inkFileName;
    //[SerializeField] private TextAsset inkJsonData;

    [Header("Graphic Control")]
    [SerializeField] private SpriteRenderer sprite_renderer = null;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int fps;
    [SerializeField] private bool disableIdleAnim;
    private SpriteAnimator spriteAnimator;

    [HideInInspector] public bool banInteractDirection;
    [HideInInspector] public bool lock_u, lock_ru, lock_r, lock_rd, lock_d, lock_ld, lock_l, lock_lu;
    
    private void Start()
    {
        if (!disableIdleAnim)
        {
            if (sprite_renderer == null)
                sprite_renderer = GetComponent<SpriteRenderer>();
            spriteAnimator = new SpriteAnimator(sprites, sprite_renderer, fps);
            spriteAnimator.InitializeAnimator();
        }
    }

    private void Update()
    {
        if (!disableIdleAnim)
            spriteAnimator.LoopAnimate();
    }

    public void Interact()
    {
        if (ValidInteractDirection())
        {
            TextAsset inkJsonData = Resources.Load<TextAsset>($"Dialogue/{UIControl.currentLangMode}/{gameObject.scene.name}/{gameObject.name}/{inkFileName}");
            DialogueManager.instance.EnterDialogue(inkJsonData);
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
