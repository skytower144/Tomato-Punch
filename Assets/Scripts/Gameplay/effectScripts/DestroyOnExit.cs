using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnExit : StateMachineBehaviour
{
    public bool destroyParent;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (destroyParent)
            Destroy(animator.transform.parent.gameObject, stateInfo.length);
        else
            Destroy(animator.gameObject, stateInfo.length);
    }
    
}
