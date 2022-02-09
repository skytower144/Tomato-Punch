using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Enemy_is_hurt enemy_is_hurt;
    [SerializeField] private Slider slider;
    public float enemy_hpShrinkTimer;
    [SerializeField] private Image fill, damagedFill, enemy_whiteFill;
    [SerializeField] private TextMeshProUGUI healthText;
    
    private void Update(){
        healthText.text = enemy_is_hurt.Enemy_currentHealth.ToString("F0")+ "/" + enemy_is_hurt.Enemy_maxHealth.ToString("F0");
        enemy_hpShrinkTimer -= Time.deltaTime; // count down timer
        if (enemy_hpShrinkTimer < 0)
        {
            if (slider.normalizedValue < damagedFill.fillAmount)
            {
                float shrinkSpeed = 5f * Time.deltaTime;
                damagedFill.fillAmount = Mathf.Lerp(damagedFill.fillAmount,slider.normalizedValue,shrinkSpeed);
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
    public void Enemy_setWhiteFill()
    {
        enemy_whiteFill.fillAmount = slider.normalizedValue;
        enemy_whiteFill.enabled = true;
    }
    public void Enemy_WhiteFillOff()
    {
        enemy_whiteFill.enabled = false;
    }
}
