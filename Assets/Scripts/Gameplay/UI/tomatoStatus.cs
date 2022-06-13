using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class tomatoStatus : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private TextMeshProUGUI text_maxHp, text_currentHp, text_atkpt, text_defpt, text_money, text_totalExp, text_leftExp, text_statPt;
    [SerializeField] private GameObject statusArrow;
    [System.NonSerialized] public float player_maxHp, player_currentHp, player_atk, player_def;
    [System.NonSerialized] public int STATPOINT = 3;
    public float player_totalExp, player_leftExp;
    public int player_statPt;

    // Stat Increase Amount
    private int HpIncrease = 10;
    private int AtkIncrease = 2;
    private int DefIncrease = 5;

    public int playerMoney = 0;
    private void OnEnable()
    {
        player_maxHp  = tomatocontrol.maxHealth;
        player_currentHp = tomatocontrol.currentHealth;
        player_atk = tomatocontrol.tomatoAtk;
        player_def = tomatocontrol.maxGuard;

        text_maxHp.text = player_maxHp.ToString("F0");
        text_currentHp.text = player_currentHp.ToString("F0");
        text_atkpt.text = player_atk.ToString("F0");
        text_defpt.text = player_def.ToString("F0");

        text_totalExp.text = player_totalExp.ToString("F0");
        text_leftExp.text = player_leftExp.ToString("F0");

        text_money.text = playerMoney.ToString("F0");

        text_statPt.text = player_statPt.ToString("F0");

        CheckStatPt();
    }

    public void IncreaseStat(int number)
    {
        if (player_statPt - 1 >= 0){
            player_statPt -= 1;
            text_statPt.text = player_statPt.ToString("F0");
            CheckStatPt();

            if (number == 2)
            {
                tomatocontrol.maxHealth += HpIncrease;
                player_maxHp = tomatocontrol.maxHealth;
                text_maxHp.text = player_maxHp.ToString("F0");
            }
            else if (number == 1)
            {
                tomatocontrol.tomatoAtk += AtkIncrease;
                player_atk = tomatocontrol.tomatoAtk;
                text_atkpt.text = player_atk.ToString("F0");
            }
            else if (number == 0)
            {
                tomatocontrol.maxGuard += DefIncrease;
                player_def = tomatocontrol.maxGuard;
                text_defpt.text = player_def.ToString("F0");
            }
        }
    }

    private void CheckStatPt()
    {
        if (player_statPt > 0)
        {
            statusArrow.SetActive(true);
        }
        else if (player_statPt == 0)
        {
            statusArrow.SetActive(false);
        }
    }
}
