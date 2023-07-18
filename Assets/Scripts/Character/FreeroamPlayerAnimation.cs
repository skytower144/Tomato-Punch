using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeroamPlayerAnimation : MonoBehaviour
{
    private void SetIsAnimatingFalse()
    {
        GameManager.gm_instance.player_movement.SetIsAnimating(false);
    }

    private void SetIsAnimatingTrue()
    {
        GameManager.gm_instance.player_movement.SetIsAnimating(true);
    }
}
