using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class tomatoStatus : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Slider status_HP, status_ATK, status_DEF, status_STM;
    [SerializeField] private TextMeshProUGUI text_hpt, text_atkpt, text_defpt, text_stmpt;
    private void Start()
    {
        status_HP.value  = tomatocontrol.maxHealth;
        status_ATK.value = tomatocontrol.tomatoAtk;
        status_DEF.value = tomatocontrol.maxGuard;
        status_STM.value = tomatocontrol.maxStamina;

        text_hpt.text = status_HP.value.ToString("F0");
        text_atkpt.text = status_ATK.value.ToString("F0");
        text_defpt.text = status_DEF.value.ToString("F0");
        text_stmpt.text = status_STM.value.ToString("F0");

    }
}
