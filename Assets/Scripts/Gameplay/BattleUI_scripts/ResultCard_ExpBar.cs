using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCard_ExpBar : MonoBehaviour
{
    private Slider result_expFill;
    private TextMeshProUGUI result_levelText;
    private GameObject expBlink;
    private int playerLevel;
    private int EXP_CURVE = 13;
    private float exp_initial, earned_exp, exp, exp_count, temp_count;
    private float INCREASE_SPEED = 35f;
    private float SPEED_CURVE;
    private bool startIncrease;
    void Start()
    {
        SPEED_CURVE = INCREASE_SPEED/30f;
    }
    void OnEnable()
    {
        startIncrease = false;
        exp = 0;
        exp_count = 0;
        temp_count = 0;
    }
    void Update()
    {
        if(startIncrease)
        {
            if (exp_count < earned_exp){
                exp = Time.deltaTime * INCREASE_SPEED;
                exp_count += exp;

                if (exp_count > earned_exp){
                    exp = exp - (exp_count - earned_exp);
                }
                
                if (result_expFill.value + exp >= result_expFill.maxValue){
                    AdjustDifference();
                    LevelUp();
                }
                else {
                    result_expFill.value += exp;
                }
            }
            else {
                result_expFill.value = Mathf.Round(result_expFill.value);
                if (result_expFill.value == result_expFill.maxValue)
                    LevelUp();
                startIncrease = false;
            }
        }
    }

    public void InitializeExpBar(int level, float maxExp, float currentExp, float gainExp)
    {
        result_expFill = gameObject.GetComponent<Slider>();
        expBlink = transform.GetChild(2).gameObject;
        result_levelText = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        playerLevel = level;
        result_expFill.maxValue = maxExp;

        result_expFill.value = currentExp;
        exp_initial = currentExp;

        earned_exp = gainExp;

        result_levelText.text = playerLevel.ToString("F0");
    }

    public void DisplayExp()
    {
        startIncrease = true;
    }

    public bool DisplayExp_isOver()
    {
        return (startIncrease == false);
    }

    private void LevelUp()
    {
        expBlink.SetActive(true);
        Invoke("turnOffBlink", 0.5f);

        playerLevel += 1;
        result_expFill.value = 0;
        exp_initial = 0;

        result_expFill.maxValue = playerLevel * (playerLevel + 1) * EXP_CURVE - EXP_CURVE * 2;

        INCREASE_SPEED = result_expFill.maxValue * SPEED_CURVE;
        result_levelText.text = playerLevel.ToString("F0");
    }

    private void turnOffBlink()
    {
        expBlink.SetActive(false);
    }
    private void AdjustDifference()
    {
        temp_count += result_expFill.maxValue - exp_initial;
        exp_count = temp_count;
    }
}
