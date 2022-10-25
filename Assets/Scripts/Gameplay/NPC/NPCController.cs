using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int fps;
    SpriteAnimator spriteAnimator;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJsonData;
    [SerializeField] private bool disableIdleAnim;

    private void Start()
    {
        if (!disableIdleAnim)
        {
            spriteAnimator = new SpriteAnimator(sprites, GetComponent<SpriteRenderer>(), fps);
            spriteAnimator.InitializeAnimator();
        }
    }

    private void Update()
    {
        if (!disableIdleAnim)
            spriteAnimator.HandleUpdate();
    }

    public void Interact()
    {
        DialogueManager.instance.EnterDialogue(inkJsonData);
    }
}
