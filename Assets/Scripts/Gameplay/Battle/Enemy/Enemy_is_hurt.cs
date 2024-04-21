using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_is_hurt : MonoBehaviour
{
    private BattleSystem battleSystem;
    private EnemyBase enemyBase;
    [SerializeField] private Transform Parent, BattleCanvas_Parent;
    [SerializeField] private EnemyControl enemyControl;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Animator tomatoAnim;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private EnemyHealthBar enemyHealthBar;
    [SerializeField] private GameObject hitEffect, hitSpark, gatHit1, gatHit2, enemy_guardEffect, defeatedEffect_flash, defeatedEffect_beam, deflectLaserHit;
    
    public EnemyHealthBar EnemyHealthBar => enemyHealthBar;
    [HideInInspector] public static bool enemy_isPunched, enemy_isDefeated, enemyIsHit;
    [System.NonSerialized] public bool guardUp;
    [System.NonSerialized] public bool isGuarding;
    [System.NonSerialized] public int hitct;
    public float Enemy_maxHealth, Enemy_currentHealth;
    private int hpReachedZero = 0;
    public int HpReachedZero => hpReachedZero;
    
    void Start()
    {
        battleSystem = GameManager.gm_instance.battle_system;
    }
    void OnEnable()
    {
        enemyBase = enemyControl._base;

        Enemy_maxHealth = enemyBase.EnemyMaxHealth;
        Enemy_currentHealth = enemyBase.EnemyCurrentHealth;

        enemyHealthBar.Enemy_SetFace(enemyBase.defaultFace);
        enemyHealthBar.Enemy_SetMaxHealth(Enemy_maxHealth);
        enemyHealthBar.Enemy_SetHealth(Enemy_currentHealth);
        enemyHealthBar.Enemy_setDamageFill();

        hitct = 0;
        hpReachedZero = 0;
        guardUp = false;
    }
    void OnTriggerEnter2D(Collider2D col) 
    {
        enemyControl.enemyAnimControl.CancelScheduledInvokes();
        
        if(Enemy_parried.isParried && EnemyControl.isPhysical)
        {
            //GATLING MODE
            tomatocontrol.parry_bar.parryFill.fillAmount += battleSystem.parryBonus;
            tomatocontrol.parry_bar.SetWhiteBar();
            Invoke("parryWhiteOff", 0.05f);
            tomatocontrol.parry_bar.parryWhiteBar.SetActive(true);

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

                //DEFLECT LASER
                if (col.gameObject.tag.Equals("tomato_DeflectLaser"))
                {
                    tomatocontrol.currentSkillType = SkillType.Deflect_Skill;
                    
                    float projectileDmg = enemyControl.GetCurrentAttackDamage();
                    enemyHurtDamage(projectileDmg + tomatocontrol.dmg_normalPunch);
                    checkDefeat("SK");
                    Instantiate(deflectLaserHit, Parent);
                }
                //NORMAL PUNCHES
                else if(col.gameObject.tag.Equals("tomato_LP"))
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
                    float skillDmg = 0;
                    switch (tomatocontrol.currentSkillType) {
                        case SkillType.Equip_Skill:
                            skillDmg = tomatoDamage.SkillAttack(tomatocontrol.tomatoAtk, tomatocontrol.tomatoEquip[0].skillDamage);
                            break;
                        
                        case SkillType.Assist_Skill:
                            skillDmg = tomatoDamage.SkillAttack(tomatocontrol.tomatoAtk, GameManager.gm_instance.assistManager.assistDamage);
                            break;
                    }
                    enemyHurtDamage(skillDmg);
                    checkDefeat("SK");
                }
            }
        }
    }

    public void ParryBonus()
    {
        tomatocontrol.parry_bar.parryFill.fillAmount += 0.1f;
        tomatocontrol.parry_bar.CheckGaksung();
    }

    private void parryWhiteOff()
    {
        tomatocontrol.parry_bar.parryWhiteBar.SetActive(false);
    }

    public void enemyHurtDamage(float damage)
    {
        if (tomatoControl.isFainted || tomatoControl.isVictory)
            return;
        
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
        if (guardUp || hitct >= enemyBase.max_hitct || hitct == Random.Range(enemyBase.min_hitct, enemyBase.max_hitct))
        {
            guardUp = true;
            enemy_isPunched = false;
            isGuarding = true;

            enemyControl.enemyAnimControl.Act(enemyBase.Guard_AnimationString, BattleActType.Guard);
            Instantiate(enemy_guardEffect, Parent);
        }
    }

    void minusTomatoStamina()
    {
        tomatocontrol.currentStamina -= battleSystem.blockStamina;
        if (tomatocontrol.currentStamina < 0)
            tomatocontrol.currentStamina = 0;

        staminaIcon.SetStamina(tomatocontrol.currentStamina);
        tomatocontrol.playTomatoKnockback();
    }

    public bool checkDefeat(string animString = "")
    {
        if (Enemy_currentHealth == 0){
            if (enemy_isDefeated)
                return true;
            
            if (tomatoControl.isFainted)
                return false;
            
            hpReachedZero++;
            enemyControl.EraseAllAttacks();
            enemyControl.DestroyProjectiles();

            if (animString == "GP")
                battleSystem.battleUI_Control.CancelGatleCircle();
            
            if (!battleSystem.IsNextPhase && hpReachedZero <= enemyBase.Phases.Count) {
                battleSystem.IsNextPhase = true;
                enemyControl.NextRound(hpReachedZero, animString);
                return false;
            }
            tomatoControl.isVictory = true;
            enemy_isDefeated = true;

            Instantiate(defeatedEffect_beam);
            Instantiate(defeatedEffect_flash, BattleCanvas_Parent);

            if (animString != "SUPPER") {
                tomatoAnim.enabled = false;
                enemyControl.enemyAnimControl.Defeated();
            }
            return true;
        }
        else if (animString != "" && animString != "DUNK" && animString != "SUPPER") {
            HitEffect(animString);
            HurtReact(animString);
        }
        return false;
    }
    private void HurtReact(string anim_string)
    {
        if (anim_string == "R")
            enemyControl.enemyAnimControl.Act(enemyBase.HurtR_AnimationString, BattleActType.Hurt);

        else if (anim_string == "L")
            enemyControl.enemyAnimControl.Act(enemyBase.HurtL_AnimationString, BattleActType.Hurt);

        else if (anim_string == "GP")
            enemyControl.enemyAnimControl.Act(enemyBase.HurtL_AnimationString, BattleActType.Hurt);

        else if (anim_string == "SK") {
            if (GameManager.gm_instance.assistManager.isBlast && tomatocontrol.currentSkillType == SkillType.Assist_Skill) {
                enemy_isPunched = false;
                enemyControl.enemyAnimControl.Act(enemyBase.Blasted, BattleActType.Blast);
            }
            else
                enemyControl.enemyAnimControl.Act(enemyBase.HurtAnimList[Random.Range(0, enemyBase.HurtAnimList.Count)], BattleActType.Hurt);
        }
    }
    public void HitEffect(string anim_string, float distance = 4.5f, int direction = 1)
    {
        if (anim_string != "CTR")
            enemyControl.enemyHurtFlash();
        
        if (anim_string == "R") {
            distance = 5f;
            direction = -1;
        }
        if (anim_string == "SK") {
            if (GameManager.gm_instance.assistManager.isBlast && tomatocontrol.currentSkillType == SkillType.Assist_Skill)
                tomatocontrol.BlastEffect();

            else if (tomatocontrol.currentSkillType == SkillType.Equip_Skill)
                tomatocontrol.SkillEffect();
            
            else
                SparkEffect(distance, direction);
        }
        else if (anim_string != "GP")
            SparkEffect(distance, direction);
    }
    private void SparkEffect(float distance, int direction)
    {
        Instantiate(hitEffect, new Vector2 (transform.position.x + distance, transform.position.y), Quaternion.identity);
        var spark = Instantiate(hitSpark, transform);
        spark.transform.localScale = new Vector2(spark.transform.localScale.x * direction, spark.transform.localScale.y);
    }
}
