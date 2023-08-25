using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BattleContinue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reviveCostUI;
    [SerializeField] private TextMeshProUGUI countdown_text, money_text, reviveCost_text;
    [SerializeField] private GameObject continue_group, insert_coin, revive_obj, cover;
    private GameObject ko_obj;
    private BattleSystem battle_system;
    private int reviveCost, left_playerMoney;
    private float playerMoney;
    private float TIMER_SPEED = 0.8f;
    private float DECREASE_SPEED = 90f;
    private float timeRemaining;
    private bool timerIsRunning = false;
    private bool runMoneyTimer;

    void Start()
    {
        timeRemaining = 9;
        runMoneyTimer = false;

        DOTween.Play("insert_coin");

        string waitAnimation = battle_system.GetEnemyBase().Wait;
        battle_system.GetEnemyAnim().Play(waitAnimation, -1, 0f);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > -0.99f)
            {
                float prevTime = timeRemaining;

                timeRemaining -= Time.deltaTime * TIMER_SPEED;
                DisplayTime(prevTime, timeRemaining);
            }
            else
            {
                ExitContinue();
            }

            if (battle_system.Press_Key("RightPunch")){
                ExitContinue();
            }
            else if (battle_system.Press_Key("LeftPunch")){
                Revive();
            }
        }
        else if (runMoneyTimer)
        {
            DecreaseMoney();
        }
    }

    public void Continue_InitializeData(BattleSystem script, GameObject KO)
    {
        battle_system = script;
        ko_obj = KO;

        playerMoney = battle_system.GetPlayerMoney();
        money_text.text = playerMoney.ToString("F0");

        DetermineCost();
        AdjustLanguage();
        
        timerIsRunning = true;
    }

    private void DetermineCost()
    {
        reviveCost = battle_system.ReviveCostFormula();
        
        left_playerMoney = (int)playerMoney - reviveCost;

        if (reviveCost == 0)
            reviveCost = 1;
        
        reviveCost_text.text = reviveCost.ToString("F0");
    }

    private void DisplayTime(float previous_time, float display_time)
    {
        previous_time += 1; // Considering FloorToInt: 9.9F -> 9
        display_time += 1;

        float previous_seconds = Mathf.FloorToInt(previous_time % 60);
        float seconds = Mathf.FloorToInt(display_time % 60);

        if (previous_seconds > seconds)
        {
            DOTween.Rewind("count_down");
            DOTween.Play("count_down");

            DOTween.Rewind("shake_continue");
            DOTween.Play("shake_continue");
        }

        countdown_text.text = seconds.ToString("F0");
    }

    private void ExitContinue()
    {
        timerIsRunning = false;

        continue_group.SetActive(false);
        insert_coin.SetActive(false);
        revive_obj.SetActive(false);
        cover.SetActive(false);
        Instantiate(ko_obj, transform.parent);

        Destroy(gameObject);
    }

    private void Revive()
    {
        timerIsRunning = false;

        Invoke("ReviveTomato", 1f);

        continue_group.SetActive(false);
        insert_coin.transform.GetChild(0).gameObject.SetActive(false);
        revive_obj.SetActive(false);
        cover.SetActive(false);

        battle_system.CoinFlip();

        money_text.color = new Color32(207, 58, 68, 255);

        runMoneyTimer = true; // Begins DecreaseMoney() in Update
    }

    private void DecreaseMoney()
    {
        playerMoney -= Time.deltaTime * DECREASE_SPEED;

        if (playerMoney <= left_playerMoney)
        {
            runMoneyTimer = false;
            playerMoney = left_playerMoney;
            battle_system.UpdatePlayerMoney(reviveCost);

            money_text.color = new Color32(128, 79, 69, 255);

            DOTween.Rewind("coin_inserted");
            DOTween.Play("coin_inserted");
        }

        money_text.text = playerMoney.ToString("F0");
    }

    private void ReviveTomato()
    {
        battle_system.GetTomatoAnim().Play("tomato_revive", -1, 0f);
        battle_system.ScreenFlash();
        Destroy(gameObject, 1f);
    }

    private void AdjustLanguage()
    {
        reviveCostUI.text = UIControl.instance.uiTextDict["ReviveCost_Text"];
        UIControl.instance.SetFontData(reviveCostUI, "ReviveCost_Text");
    }
}
