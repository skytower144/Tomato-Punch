using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class tomatoStatus : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private TextMeshProUGUI text_maxHp, text_currentHp, text_atkpt, text_defpt, text_money, text_totalExp, text_leftExp, text_statPt;
    [SerializeField] private GameObject statusArrow, status_up_effect, frameshine_effect;
    [SerializeField] private Transform spawnPoint;
    [System.NonSerialized] public float player_maxHp, player_currentHp, player_atk, player_def;
    [System.NonSerialized] public int STATPOINT = 3;
    public float player_totalExp, player_leftExp;
    public int player_statPt;

    [Header("[ Stat Increase Amount ]")]
    [SerializeField] private int HpIncrease;
    [SerializeField] private int AtkIncrease;
    [SerializeField] private int DefIncrease;

    [SerializeField] private List<Color32> levelEffectColors;
    private float levelEffect_x = -106.6f;
    private float[] levelEffect_y = new float[] { 43f, 126f, 211.89f };

    public int playerMoney;
    public void OnEnable()
    {
        player_maxHp  = tomatocontrol.maxHealth;
        player_currentHp = tomatocontrol.currentHealth;
        player_atk = tomatocontrol.tomatoAtk;
        player_def = tomatocontrol.maxGuard;
        
        DisplayAccurateHealthColor();
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

            SpawnStatEffect(number);

            if (number == 2)
            {
                float hp_ratio = tomatocontrol.currentHealth / tomatocontrol.maxHealth;
                
                tomatocontrol.maxHealth += HpIncrease;
                player_maxHp = tomatocontrol.maxHealth;
                text_maxHp.text = player_maxHp.ToString("F0");

                tomatocontrol.currentHealth = Mathf.Ceil(player_maxHp * hp_ratio);
                player_currentHp = tomatocontrol.currentHealth;
                text_currentHp.text = player_currentHp.ToString("F0");

                DOTween.Rewind("textshine_hp");
                DOTween.Play("textshine_hp");
            }
            else if (number == 1)
            {
                tomatocontrol.tomatoAtk += AtkIncrease;
                player_atk = tomatocontrol.tomatoAtk;
                text_atkpt.text = player_atk.ToString("F0");

                DOTween.Rewind("textshine_atk");
                DOTween.Play("textshine_atk");
            }
            else if (number == 0)
            {
                tomatocontrol.maxGuard += DefIncrease;
                player_def = tomatocontrol.maxGuard;
                text_defpt.text = player_def.ToString("F0");

                DOTween.Rewind("textshine_def");
                DOTween.Play("textshine_def");
            }
        }
    }

    private void DisplayAccurateHealthColor()
    {
        // Since Battle system UI does not support decimals,
        // for example 9.5 HP will also show as 10 HP in the UI.
        // Decided to implicitly show the difference by text color.

        if (player_currentHp == player_maxHp)
            text_currentHp.color = text_maxHp.color;
        else
            text_currentHp.color = new Color32(109, 115, 111, 255);
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

    private void SpawnStatEffect(int number)
    {
        GameObject effect = Instantiate(status_up_effect, spawnPoint);
        Instantiate(frameshine_effect, spawnPoint);

        SpriteRenderer effectSr = effect.GetComponent<SpriteRenderer>();

        effect.transform.localPosition = new Vector2(levelEffect_x, levelEffect_y[number]);
        effectSr.color = levelEffectColors[number];

        effect.transform.SetParent(GameManager.gm_instance.transform.parent);
        effect.transform.localScale = new Vector3(1, 1, 1);
    }

    public bool CheckEnoughMoney(int loss)
    {
        if (playerMoney < loss)
            return false;
        return true;
    }

    public void UpdatePlayerMoney(int amount)
    {
        playerMoney += amount;
    }
}
