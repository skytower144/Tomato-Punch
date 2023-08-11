using System.Collections;
using UnityEngine;

public class DialogueAnimStateControl : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        DialogueManager.instance.cutsceneHandler.AfterAnimComplete(animator.GetComponent<CharacterPointer>().targetCharacter, stateInfo.length);
    }
}