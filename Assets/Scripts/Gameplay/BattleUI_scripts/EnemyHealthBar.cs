using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public float enemy_hpShrinkTimer;
    [SerializeField] private Image fill, damagedFill;
    
    private void Update(){
        enemy_hpShrinkTimer -= Time.deltaTime; // count down timer
        if (enemy_hpShrinkTimer < 0)
        {
            if (slider.normalizedValue < damagedFill.fillAmount)
            {
                float shrinkSpeed = 1f;
                damagedFill.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }
    public void Enemy_SetMaxHealth(float health)
    {
        slider.maxValue = health;
    }
    public void Enemy_SetHealth(float health)
    {
        slider.value = health;
    }
    public void Enemy_setDamageFill()
    {
        damagedFill.fillAmount = slider.normalizedValue;
    }
}
