using System;
using System.Collections;
using UnityEngine;

public interface Character
{
    bool IsAnimating();
    Animator UsesDefaultAnimator();
    void SetIsAnimating(bool state);
    void Play(string clipName, Action dialogueAction = null, bool stopAfterAnimation = false);
    void Turn(string direction);
    IEnumerator PlayMoveActions(string[] posStrings, float moveSpeed, bool isAnimate);
}
