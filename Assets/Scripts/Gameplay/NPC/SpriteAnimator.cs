using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    int fps;
    float frameRate;

    int currentFrame;
    int totalFrames;
    float timer;

    // Constructor
    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, int fps)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.fps = fps;
    }

    public void InitializeAnimator()
    {
        currentFrame = 0;
        totalFrames = frames.Count;

        timer = 0f;
        spriteRenderer.sprite = frames[0];

        frameRate = 1f / (float)fps;
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % totalFrames;
            spriteRenderer.sprite = frames[currentFrame];
            timer = 0f;
        }
    }
}
