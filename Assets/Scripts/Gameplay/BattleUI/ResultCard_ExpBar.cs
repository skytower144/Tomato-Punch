using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResultCard_ExpBar : MonoBehaviour
{
    private ParticleSystem sparkleEffect;
    private Slider result_expFill;
    private TextMeshProUGUI result_levelText;
    private GameObject expBlink, cardBlink, levelUpEffect;
    private int playerLevel;
    private int EXP_CURVE = 13;
    private float exp_initial, earned_exp, exp, exp_count, temp_count;

    public float INCREASE_SPEED = 1f;
    private bool startIncrease;

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

                PassPlayerData();
                startIncrease = false;
            }
        }
    }

    public void InitializeExpBar(int level, float maxExp, float currentExp, float gainExp)
    {
        result_expFill = gameObject.GetComponent<Slider>();
        result_levelText = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        expBlink = transform.GetChild(2).gameObject;
        cardBlink = transform.parent.gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject;
        levelUpEffect = transform.parent.gameObject.transform.GetChild(5).gameObject.transform.GetChild(1).gameObject;
        sparkleEffect = transform.parent.gameObject.transform.GetChild(5).gameObject.transform.GetChild(2).GetComponent<ParticleSystem>();

        playerLevel = level;

        result_expFill.maxValue = maxExp;
        result_expFill.value = currentExp;
        exp_initial = currentExp;
        earned_exp = gainExp;

        result_levelText.text = playerLevel.ToString("F0");

        INCREASE_SPEED = result_expFill.maxValue;
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
        cardBlink.SetActive(true);
        levelUpEffect.SetActive(true);
        
        Invoke("turnOffBlink", 0.7f);
        var sparkle_em = sparkleEffect.emission;
        sparkle_em.enabled = true;
        sparkleEffect.Play();

        playerLevel += 1;
        result_expFill.value = 0;
        exp_initial = 0;

        result_expFill.maxValue = playerLevel * (playerLevel + 1) * EXP_CURVE - EXP_CURVE * 2;

        INCREASE_SPEED = result_expFill.maxValue;
        result_levelText.text = playerLevel.ToString("F0");
        DOTween.Rewind("text_levelup");
        DOTween.Play("text_levelup");
    }

    private void PassPlayerData()
    {
        transform.parent.gameObject.GetComponent<ResultCard>().updateLevel = playerLevel;
        transform.parent.gameObject.GetComponent<ResultCard>().max_exp = result_expFill.maxValue;
        transform.parent.gameObject.GetComponent<ResultCard>().current_exp = result_expFill.value;
    }

    private void turnOffBlink()
    {
        expBlink.SetActive(false);
        cardBlink.SetActive(false);
        levelUpEffect.SetActive(false);
    }
    private void AdjustDifference()
    {
        temp_count += result_expFill.maxValue - exp_initial;
        exp_count = temp_count;
    }

}
