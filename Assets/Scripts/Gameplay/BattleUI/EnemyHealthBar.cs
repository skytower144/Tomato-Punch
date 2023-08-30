using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] Enemy_is_hurt enemy_is_hurt;
    [SerializeField] private Slider slider;
    public float enemy_hpShrinkTimer;
    [SerializeField] private Image enemyFace, fill, damagedFill, enemy_whiteFill;
    [SerializeField] private TextMeshProUGUI healthText;
    private float INCREASE_SPEED = 1;
    private bool checkOnce = false;
    
    private void Update(){
        healthText.text = enemy_is_hurt.Enemy_currentHealth.ToString("F0")+ "/" + enemy_is_hurt.Enemy_maxHealth.ToString("F0");

        if (battleSystem.resetEnemyHealth)
        {
            INCREASE_SPEED = EqualizeSpeed();
            float increased_hp = slider.value + (INCREASE_SPEED * 1.5f) * Time.deltaTime;

            if (increased_hp >= slider.maxValue){
                battleSystem.resetEnemyHealth = checkOnce = false;
                increased_hp = slider.maxValue;
                Enemy_setDamageFill();
            }

            enemy_is_hurt.Enemy_currentHealth = increased_hp;
            Enemy_SetHealth(increased_hp);
        }

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

    private float EqualizeSpeed()
    {
        if(!checkOnce){
            checkOnce = true;
            INCREASE_SPEED = slider.maxValue - slider.value;
        }
        return INCREASE_SPEED;
    }
}
