using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class tomatoStatus : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Slider status_HP, status_ATK, status_DEF;
    [SerializeField] private TextMeshProUGUI text_hpt, text_atkpt, text_defpt, text_money;

    public int playerMoney = 0;
    private void OnEnable()
    {
        status_HP.value  = tomatocontrol.maxHealth;
        status_ATK.value = tomatocontrol.tomatoAtk;
        status_DEF.value = tomatocontrol.maxGuard;

        text_hpt.text = status_HP.value.ToString("F0");
        text_atkpt.text = status_ATK.value.ToString("F0");
        text_defpt.text = status_DEF.value.ToString("F0");
        text_money.text = playerMoney.ToString("F0");
    }
}
