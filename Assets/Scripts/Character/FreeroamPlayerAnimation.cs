using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeroamPlayerAnimation : MonoBehaviour
{
    // ex) Player Wakeup Animation
    private void SetIsAnimatingFalse()
    {
        GameManager.gm_instance.player_movement.SetIsAnimating(false);
    }

    private void SetIsAnimatingTrue()
    {
        GameManager.gm_instance.player_movement.SetIsAnimating(true);
    }
}
