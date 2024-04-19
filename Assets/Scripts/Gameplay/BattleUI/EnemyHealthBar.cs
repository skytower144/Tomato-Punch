using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class EnemyHealthBar : MonoBehaviour
{
    public float enemy_hpShrinkTimer;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Enemy_is_hurt enemy_is_hurt;
    [SerializeField] private Slider slider;
    [SerializeField] private Image enemyFace, fill, damagedFill, enemy_whiteFill;
    [SerializeField] private TextMeshProUGUI healthText;
    private float increaseAmount, targetHealth;
    
    private void Update(){
        healthText.text = enemy_is_hurt.Enemy_currentHealth.ToString("F0")+ "/" + enemy_is_hurt.Enemy_maxHealth.ToString("F0");
        
        if (battleSystem.increaseEnemyHealth)
            IncreaseHealth(increaseAmount);
        
        enemy_hpShrinkTimer -= Time.deltaTime; // count down timer
        if (enemy_hpShrinkTimer < 0 && slider.normalizedValue < damagedFill.fillAmount)
        {
            float shrinkSpeed = 5f * Time.deltaTime;
            damagedFill.fillAmount = Mathf.Lerp(damagedFill.fillAmount, slider.normalizedValue, shrinkSpeed);
        }
    }
    public void Enemy_SetMaxHealth(float health)
    {
        slider.maxValue = health;
    }
    public void Enemy_SetHealth(float health)
    {
        slider.value = health;
        faceChange();
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

    public void Enemy_SetFace(Sprite frontSprite)
    {
        enemyFace.sprite = frontSprite;
    }
    private void faceChange()
    {
        if (slider.normalizedValue >= 0.5f){
            enemyFace.sprite = battleSystem.GetEnemyBase().defaultFace;
        }
        else if(0 < slider.normalizedValue && slider.normalizedValue < 0.5f){
            enemyFace.sprite = battleSystem.GetEnemyBase().hurtFace;
        }
        else if(slider.normalizedValue == 0){
            enemyFace.sprite = battleSystem.GetEnemyBase().koFace;
        }
    }
    public void SetIncreaseHealthAmount(float amount, bool isRevive = false)
    {
        increaseAmount = isRevive ? GetLeftoverHealth() : amount;
        targetHealth = Mathf.Clamp(slider.value + increaseAmount, 0, slider.maxValue);

        battleSystem.increaseEnemyHealth = true;
    }
    private void IncreaseHealth(float totalIncrease)
    {
        float increased_hp = slider.value + totalIncrease * 1.5f * Time.deltaTime;

        if (increased_hp >= targetHealth){
            battleSystem.increaseEnemyHealth = false;
            increased_hp = targetHealth;
            Enemy_setDamageFill();
        }
        enemy_is_hurt.Enemy_currentHealth = increased_hp;
        Enemy_SetHealth(increased_hp);
    }
    private float GetLeftoverHealth()
    {
        return slider.maxValue - slider.value;
    }
    public void SetBarColor(string color = "OG")
    {
        if (color == "RED")
            fill.color = new Color32(190, 22, 50, 255);
        else
            fill.color = new Color32(193, 34, 118, 255);
    }
}
