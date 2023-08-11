using System;
using System.Collections;

public interface Character
{
    bool IsAnimating();
    void SetIsAnimating(bool state);
    void Play(string animTag, Action dialogueAction = null, bool stopAfterAnimation = false);
    IEnumerator PlayMoveActions(string[] posStrings, float moveSpeed, bool isAnimate);
}
