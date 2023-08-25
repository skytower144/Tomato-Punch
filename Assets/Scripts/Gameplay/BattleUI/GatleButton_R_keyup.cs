using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatleButton_R_keyup : MonoBehaviour
{
    [SerializeField] Animator gatleButton_anim_R;
    [SerializeField] private GameObject gatleButtons;
    void gatleButton_R_up()
    {
        if(!BattleUI_Control.stopGatle)
            gatleButton_anim_R.Play("gatleButton_R_keyup",-1,0f);
    }
    void deactivate_GatleButton()
    {
        gatleButtons.SetActive(false);
    }
}
