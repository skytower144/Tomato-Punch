using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimWaitPlay
{
    public static IEnumerator Play(Animator targetAnim, string animName) {
        targetAnim.Play(animName,-1,0f);

        float counter = 0;
        float waitTime = ReturnClipLength(targetAnim, animName);
        // float waitTime = anim.GetCurrentAnimatorStateInfo(0).length;

        // Debug.Log(waitTime);

        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }
    }
    
    private static float ReturnClipLength(Animator targetAnim, string animName)
    {
        AnimationClip[] clips = targetAnim.runtimeAnimatorController.animationClips;
        
        foreach (AnimationClip clip in clips)
        {
            if (animName == clip.name)
                return clip.length;
        }
        return 0f;
    }

    /*////////////////////////////////////////////////////////////////////////////////////////////
    Paste this function within a script where you're trying to create animation sequences.

    private object Play(string animName)
    {
        return StartCoroutine(AnimWaitPlay.Play(anim, animName));
    }

    *////////////////////////////////////////////////////////////////////////////////////////////
}

