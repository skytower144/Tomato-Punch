using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int fps;
    SpriteAnimator spriteAnimator;

    private void Start()
    {
        spriteAnimator = new SpriteAnimator(sprites, GetComponent<SpriteRenderer>(), fps);
        spriteAnimator.InitializeAnimator();
    }

    private void Update()
    {
        spriteAnimator.HandleUpdate();
    }
}
