using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    NPCController npc;
    private SpriteRenderer spriteRenderer;
    private List<Sprite> frames;
    private int fps;
    private float frameRate;
    private bool isLoop;
    private Action dialogueAction;
    
    private int currentFrame;
    private int totalFrames;
    private float timer;
    private bool stopAnim = false;
    private bool stop_after_animation = false;

    // Constructor
    public SpriteAnimator(NPCController npc, SpriteRenderer spriteRenderer, List<Sprite> frames, int fps, bool is_loop = false, Action dialogueAction = null, bool stopAfterAnimation = false)
    {
        stopAnim = true;

        this.npc = npc;
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.fps = fps;
        this.isLoop = is_loop;
        this.dialogueAction = dialogueAction;
        this.stop_after_animation = stopAfterAnimation;

        InitializeAnimator();
    }

    private void InitializeAnimator()
    {
        currentFrame = 0;
        totalFrames = frames.Count;

        timer = 0f;
        spriteRenderer.sprite = frames[0];

        if (fps == 0)
            frameRate = 0;
        else
            frameRate = 1f / (float)fps;

        stopAnim = false;
    }

    public void Animate()
    {
        if (!stopAnim)
        {
            timer += Time.deltaTime;
            if (timer > frameRate)
            {
                int nextFrame = currentFrame + 1;

                if ((nextFrame == totalFrames) && (!isLoop)) {
                    stopAnim = true;

                    if (!stop_after_animation)
                        npc.Play(npc.idleAnim);

                    if (dialogueAction != null)
                        dialogueAction.Invoke();
                    return;
                }
                
                else
                {
                    currentFrame = nextFrame % totalFrames;
                    spriteRenderer.sprite = frames[currentFrame];
                    timer = 0f;
                }
            }
        }
    }
}
