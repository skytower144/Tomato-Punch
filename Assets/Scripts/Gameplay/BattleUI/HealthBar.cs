using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class HealthBar : MonoBehaviour
{
    public const float HP_SHRINKTIMER_MAX = 0.5f;
    [System.NonSerialized] public float hpShrinkTimer;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill, damagedFill, tomatoFace;
    [SerializeField] private Sprite face1, face2, face3, face4;
    [SerializeField] private TextMeshProUGUI healthText;
    private void Update()
    {
        healthText.text = tomatocontrol.currentHealth.ToString("F0")+ "/" + tomatocontrol.maxHealth.ToString("F0");

        if(battleSystem.resetPlayerHealth) // Reset Health after player revive
        {
            float increased_hp = slider.value + (slider.maxValue * 1.5f) * Time.deltaTime;

            if (increased_hp > slider.maxValue){
                battleSystem.resetPlayerHealth = false;
                increased_hp = slider.maxValue;
                setDamageFill();
            }

            tomatocontrol.currentHealth = increased_hp;
            SetHealth(increased_hp);
        }

        hpShrinkTimer -= Time.deltaTime; // count down timer
        if (hpShrinkTimer < 0)
        {
            if (slider.normalizedValue < damagedFill.fillAmount)
            {
                float shrinkSpeed = 5f * Time.deltaTime;
                damagedFill.fillAmount = Mathf.Lerp(damagedFill.fillAmount, slider.normalizedValue, shrinkSpeed);
                //damagedFill.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        fill.color = gradient.Evaluate(1f); //  max health color setting
    }
    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        faceChange();
    }
    public void setDamageFill()
    {
        damagedFill.fillAmount = slider.normalizedValue;
    }
    private void faceChange()
    {
        if (slider.normalizedValue >= 0.5f){
            tomatoFace.sprite = face1;
        } 
        else if(0.15f <= slider.normalizedValue && slider.normalizedValue < 0.5f){
            tomatoFace.sprite = face2;
        }
        else if(0 < slider.normalizedValue &&  slider.normalizedValue < 0.15f){
            tomatoFace.sprite = face3;
        }
        else if(slider.normalizedValue == 0){
            tomatoFace.sprite = face4;
        }
    }
}
