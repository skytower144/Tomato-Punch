using UnityEngine;

public class DisableParryLaser : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameManager.gm_instance.battle_system.tomato_control.StartCoroutine(
            GameManager.gm_instance.battle_system.tomato_control.SetDeflectLaser(animator.gameObject, false, stateInfo.length)
        );
    }
}
