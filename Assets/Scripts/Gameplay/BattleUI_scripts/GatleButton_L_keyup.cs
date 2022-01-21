using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatleButton_L_keyup : MonoBehaviour
{
    [SerializeField] Animator gatleButton_anim_L;
    [SerializeField] private GameObject gatleButtons;
    void gatleButton_L_up()
    {
        if(!BattleUI_Control.stopGatle)
            gatleButton_anim_L.Play("gatleButton_L_keyup",-1,0f);
    }

    void deactivate_GatleButton()
    {
        gatleButtons.SetActive(false);
    }
}
