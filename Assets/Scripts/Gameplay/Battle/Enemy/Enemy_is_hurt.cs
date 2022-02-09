using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_is_hurt : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private EnemyControl enemyControl;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private ParryBar tomatoParryBar;
    [SerializeField] private EnemyHealthBar enemyHealthBar;
    [SerializeField] private GameObject hitEffect, gatHit1, gatHit2;
    [HideInInspector] public static bool enemy_isPunched;
    public float Enemy_maxHealth, Enemy_currentHealth;
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        Enemy_maxHealth = enemyControl._base.EnemyMaxHealth;
        Enemy_currentHealth = enemyControl._base.EnemyCurrentHealth;

        enemyHealthBar.Enemy_SetMaxHealth(Enemy_maxHealth);
        enemyHealthBar.Enemy_SetHealth(Enemy_currentHealth);
        enemyHealthBar.Enemy_setDamageFill();
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
            anim.Play(enemyControl._base.HurtL_AnimationString,-1,0f);
            enemyHurtDamage(tomatocontrol.dmg_gatlePunch);
        }
        else
        {
            //NORMAL PUNCHES
            enemy_isPunched = true;
            if(col.gameObject.tag.Equals("tomato_LP"))
            {
                Instantiate (hitEffect, new Vector2 (transform.position.x + 4.5f, transform.position.y), Quaternion.identity);
                anim.Play(enemyControl._base.HurtL_AnimationString,-1,0f);
            }
            else if(col.gameObject.tag.Equals("tomato_RP"))
            {
                Instantiate (hitEffect, new Vector2 (transform.position.x + 5f, transform.position.y), Quaternion.identity);
                anim.Play(enemyControl._base.HurtR_AnimationString,-1,0f);
            }
            enemyHurtDamage(tomatocontrol.dmg_normalPunch);
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
    
}
