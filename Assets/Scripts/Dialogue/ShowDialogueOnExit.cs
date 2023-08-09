using UnityEngine;

public class ShowDialogueOnExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (DialogueManager.instance.hide_dialogue)
            DialogueManager.instance.ShowAndContinueDialogue();
    }
}