using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_is_hurt : MonoBehaviour
{
    private Animator anim;
    private EnemyBase enemyBase;
    [SerializeField] private Transform Parent, BattleCanvas_Parent;
    [SerializeField] private EnemyControl enemyControl;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private tomatoDamage tomatodamage;
    [SerializeField] private Animator tomatoAnim;
    [SerializeField] private ParryBar tomatoParryBar;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private EnemyHealthBar enemyHealthBar;
    [SerializeField] private GameObject hitEffect, gatHit1, gatHit2, enemy_guardEffect, defeatedEffect_flash, defeatedEffect_beam;
    [HideInInspector] public static bool enemy_isPunched, enemy_isDefeated, enemyIsHit;
    [System.NonSerialized] public bool guardUp;
    [System.NonSerialized] public int hitct;
    public float Enemy_maxHealth, Enemy_currentHealth;
    
    void OnEnable()
    {
        enemyBase = enemyControl._base;

        anim = GetComponentInParent<Animator>();
        Enemy_maxHealth = enemyBase.EnemyMaxHealth;
        Enemy_currentHealth = enemyBase.EnemyCurrentHealth;

        enemyHealthBar.Enemy_SetMaxHealth(Enemy_maxHealth);
        enemyHealthBar.Enemy_SetHealth(Enemy_currentHealth);
        enemyHealthBar.Enemy_setDamageFill();

        hitct = 0;
        guardUp = false;
    }
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(Enemy_parried.isParried && EnemyControl.isPhysical)
        {
            //GATLING MODE
            tomatoParryBar.parryFill.fillAmount += 0.008f;
            tomatoParryBar.SetWhiteBar();
            Invoke("parryWhiteOff", 0.05f);
            tomatoParryBar.parryWhiteBar.SetActive(true);

            if(col.gameObject.tag.Equals("tomato_LP"))
            {
                Instantiate (gatHit1, new Vector2 (transform.position.x + 2.8f, transform.position.y + 0.8f), Quaternion.identity);
                Instantiate (gatHit2, new Vector2 (transform.position.x + 4f, transform.position.y + 2f), Quaternion.identity);
            }
            else if(col.gameObject.tag.Equals("tomato_RP"))
            {
                Instantiate (gatHit1, new Vector2 (transform.position.x + 4f, transform.position.y - 0.2f), Quaternion.identity);
                Instantiate (gatHit2, new Vector2 (transform.position.x + 6.5f, transform.position.y + 0.2f), Quaternion.identity);
            }
            enemyHurtDamage(tomatocontrol.dmg_gatlePunch);
            checkDefeat("GP");
        }
        else if(!enemy_isDefeated)
        {
            if (!Enemy_countered.enemy_isCountered){
                hitct += 1;
                enemyReflex();
            }
            
            if (guardUp)
                minusTomatoStamina();
        
            if(!guardUp)
            {
                enemy_isPunched = true;
                enemyIsHit = true;           // Boolean to prevent enemyHurt anim and counter2idle anim happening at the same time

                //NORMAL PUNCHES
                if(col.gameObject.tag.Equals("tomato_LP"))
                {
                    enemyHurtDamage(tomatocontrol.dmg_normalPunch);
                    checkDefeat("L");
                }
                else if(col.gameObject.tag.Equals("tomato_RP"))
                {
                    enemyHurtDamage(tomatocontrol.dmg_normalPunch);
                    checkDefeat("R");
                }

                //SKILL
                else if(col.gameObject.tag.Equals("tomato_SK"))
                {
                    float skillDmg = tomatodamage.SkillAttack(tomatocontrol.tomatoAtk, tomatocontrol.tomatoEquip[0].skillDamage);
                    enemyHurtDamage(skillDmg);
                    checkDefeat("SK");
                }
            }
        }
    }

    public void ParryBonus()
    {
        tomatoParryBar.parryFill.fillAmount += 0.1f;
    }

    private void parryWhiteOff()
    {
        tomatoParryBar.parryWhiteBar.SetActive(false);
    }

    public void enemyHurtDamage(float damage)
    {
        if(Enemy_currentHealth < damage)
            damage = Enemy_currentHealth;
        Enemy_currentHealth -= damage;

        enemyHealthBar.Enemy_SetHealth(Enemy_currentHealth);
        enemyHealthBar.enemy_hpShrinkTimer = HealthBar.HP_SHRINKTIMER_MAX;
        enemyHealthBar.Invoke("Enemy_WhiteFillOff",0.05f);
        enemyHealthBar.Enemy_setWhiteFill();
    }

    private void enemyReflex()
    {
        if (guardUp || hitct >= enemyBase.max_hitct)
        {
            anim.Play(enemyBase.Guard_AnimationString,-1,0f);
            Instantiate(enemy_guardEffect, Parent);
            guardUp = true;
            enemy_isPunched = false;
            return;
        }

        int randct = Random.Range(enemyBase.min_hitct, enemyBase.max_hitct);
        if (hitct == randct)
        {
            anim.Play(enemyBase.Guard_AnimationString,-1,0f);
            Instantiate(enemy_guardEffect, Parent);
            guardUp = true;
            enemy_isPunched = false;
        }
    }

    void minusTomatoStamina()
    {
        tomatocontrol.currentStamina -= 3;
        if (tomatocontrol.currentStamina < 0)
            tomatocontrol.currentStamina = 0;

        staminaIcon.SetStamina(tomatocontrol.currentStamina);
        tomatocontrol.playTomatoKnockback();
    }

    public bool checkDefeat(string animString)
    {
        if (Enemy_currentHealth == 0){
            tomatoControl.isVictory = true;
            enemy_isDefeated = true;

            tomatoAnim.enabled = false;
            anim.Play(enemyBase.Defeated_AnimationString,-1,0f);

            Instantiate(defeatedEffect_beam);
            Instantiate(defeatedEffect_flash, BattleCanvas_Parent);

            if (animString == "GP"){
                gatleCircleControl.failUppercut = true;
            }
            return true;
        }
        else{
            if (animString == "L"){
                Instantiate (hitEffect, new Vector2 (transform.position.x + 4.5f, transform.position.y), Quaternion.identity);
                anim.Play(enemyBase.HurtL_AnimationString,-1,0f);
            }
            else if (animString == "R"){
                Instantiate (hitEffect, new Vector2 (transform.position.x + 5f, transform.position.y), Quaternion.identity);
                anim.Play(enemyBase.HurtR_AnimationString,-1,0f);
            }
            else if (animString == "SK" || animString == "GP"){
                anim.Play(enemyBase.HurtL_AnimationString,-1,0f);
            }
            return false;
        }
    }

}
