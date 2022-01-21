using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    public const float HP_SHRINKTIMER_MAX = 1f;
    public float hpShrinkTimer;
    public Image fill, damagedFill;
    
    private void Update(){
        hpShrinkTimer -= Time.deltaTime; // count down timer
        if (hpShrinkTimer < 0)
        {
            if (slider.normalizedValue < damagedFill.fillAmount)
            {
                float shrinkSpeed = 1f;
                damagedFill.fillAmount -= shrinkSpeed * Time.deltaTime;
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
    }
    public void setDamageFill()
    {
        damagedFill.fillAmount = slider.normalizedValue;
    }
}
