using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    private SpriteRenderer spriteRenderer;
    private List<Sprite> frames;
    private int fps;
    private float frameRate;
    private bool isLoop;
    
    private int currentFrame;
    private int totalFrames;
    private float timer;
    private bool stopAnim = false;

    // Constructor
    public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> frames, int fps, bool is_loop = false)
    {
        stopAnim = true;

        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.fps = fps;
        this.isLoop = is_loop;

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

                if ((nextFrame == totalFrames) && (!isLoop))
                    return;
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
