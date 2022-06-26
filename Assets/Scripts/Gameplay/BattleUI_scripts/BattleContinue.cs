using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BattleContinue : MonoBehaviour
{
    private BattleSystem battle_system;
    private GameObject continue_group, insert_coin, ko_obj;
    private TextMeshProUGUI countdown_text, money_text;
    private float TIMER_SPEED = 0.8f;
    private float timeRemaining;
    private bool timerIsRunning = false;

    void Start()
    {
        timeRemaining = 9;

        DOTween.Play("insert_coin");
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

            if(Input.GetKeyDown(KeyCode.P)){
                ExitContinue();
            }
        }
    }

    public void Continue_InitializeData(BattleSystem script, GameObject KO)
    {
        battle_system = script;
        ko_obj = KO;

        continue_group = transform.GetChild(0).gameObject;
        insert_coin = transform.GetChild(1).gameObject;
        
        countdown_text = continue_group.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        money_text = insert_coin.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        money_text.text = battle_system.GetPlayerMoney().ToString("F0");

        timerIsRunning = true;
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
        Instantiate(ko_obj, transform.parent);

        Destroy(gameObject);
    }

}
