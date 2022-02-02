using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private Animator anim;
    private GameObject counter_instance;
    public EnemyBase _base;
    [SerializeField] private GameObject enemy_LA, enemy_RA, enemy_DA, enemy_Counter;
    [SerializeField] private Transform Parent;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Enemy_is_hurt enemyHurt;
    [HideInInspector] public static bool isPhysical = true;
    private bool action_afterSuffer = false;
    private bool enemy_supered = false;
    private int attackType;
    private string pjTag;     // pj selection string

    
    void Start()
    {
        anim = GetComponent<Animator>();
        InvokeRepeating("jolaAction", 1f,2f);

    }
    
    void Update()
    {
        if(gatleCircleControl.failUppercut)
        {
            anim.Play("battleJola_parried2idle",-1,0f);
        }
        else if(tomatoControl.enemyUppered)
        {
            anim.Play("battleJola_uppered",-1,0f);
            enemyHurt.ParryBonus();
        }
        else
        {
            if(tomatoControl.enemyFreeze)
            {
                tomatoControl.enemyFreeze = false;
                anim.enabled = false;
            }
            else if(tomatocontrol.enemy_supered)
            {
                tomatocontrol.enemy_supered = false;
                enemy_supered = true;

                anim.enabled = true;
                if(tomatocontrol.super == 1)
                {
                    anim.Play("battleJola_supered_chili",-1,0f);
                }
            }
        }
        
    }

    void jolaAction()
    {
        if(action_afterSuffer)
        {
            action_afterSuffer = false;
            return;
        }
        else if(!Enemy_parried.isParried && !Enemy_countered.enemy_isCountered && !Enemy_is_hurt.enemy_isPunched && !enemy_supered)
        {
            
            if((Random.value<= 0.1))
            {
                anim.Play("battleJola_idle");
            }
            
            else if((Random.value<= 0.25))
            {
                Enemy_AttackDetail LA = _base.EnemyAttack("LA");
                attackType = LA.EnemyAttackType;
                isPhysical = LA.PhysicalAttack;
                tomatoguard.damage = LA.EnemyAttackDmg;
                anim.Play("battleJola_LA");
            }
            else if((Random.value<= 0.25))
            {
                Enemy_AttackDetail RA = _base.EnemyAttack("RA");
                attackType = RA.EnemyAttackType;
                isPhysical = RA.PhysicalAttack;
                tomatoguard.damage = RA.EnemyAttackDmg;
                anim.Play("battleJola_RA");
            }
            
            else if((Random.value<= 0.2))
            {
                Enemy_AttackDetail DA = _base.EnemyAttack("DA");
                attackType = DA.EnemyAttackType;
                isPhysical = DA.PhysicalAttack;
                tomatoguard.damage = DA.EnemyAttackDmg;
                anim.Play("battleJola_DA");
            }
            
            else if((Random.value<= 0.2))
            {
                Enemy_AttackDetail HAT = _base.EnemyAttack("HatAttack");
                pjTag = "HatAttack";
                attackType = HAT.EnemyAttackType;
                isPhysical = HAT.PhysicalAttack; 
                tomatoguard.damage = HAT.EnemyAttackDmg;
                anim.Play("battleJola_HatAttack");
            }
            
        }
    }

    void actionOver()
    {
        enemy_supered = false;
        if(!Enemy_countered.enemy_isCountered && !Enemy_is_hurt.enemy_isPunched)
            anim.Play("battleJola_idle",-1,0f);
    }
    void enemy_isPunchedEnd()
    {
        Enemy_is_hurt.enemy_isPunched = false;
    }
    void hurtOver()
    {
        if(Enemy_parried.isParried && EnemyControl.isPhysical)                             // punching enemy when enemy is parried
            anim.Play("battleJola_parriedAft",-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched && Enemy_countered.enemy_isCountered)    // punching enemy when enemy is countered
            anim.Play("battleJola_suffer",-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched)                                            // go back to idle when player did not attack
            anim.Play("battleJola_idle",-1,0f);
    }

    void enemyCounterStart()
    {
        counter_instance = Instantiate (enemy_Counter, Parent);
    }
    void enemyCounterEnd()
    {
        Destroy(counter_instance);
    }

    void hitFrame() //depending on the animation, this function decides whether it should instantiate LA/RA/DA collider.
                      // -1: left, 1: right, 0: down                
    {
        if(attackType == -1)
        {   
            Instantiate (enemy_LA, Parent);
        }
        else if(attackType == 1)
        {
            Instantiate (enemy_RA, Parent);
        }
        else if(attackType == 0)
        {
            Instantiate (enemy_DA, Parent);
        }
    }
    void beginSuffer()
    {
        Invoke("return_CounterToIdle", 3f);
        if(!Enemy_is_hurt.enemy_isPunched)     // when enemy is hurt at the exact frame transitioning to the suffer animation, 
            anim.Play("battleJola_suffer",-1,0f);   // the 'if' statement makes it prioritize the hurt animation.
    }

    void beginStun()
    {
        anim.Play("battleJola_parriedAft",-1,0f);
    }
    void return_CounterToIdle()
    {
        Enemy_is_hurt.enemy_isPunched = false;
        Enemy_countered.enemy_isCountered = false;
        action_afterSuffer = true;
    }

    void upperRecover()
    {
        anim.Play("battleJola_upperRecover",-1,0f);
    }

    void superedRecover()
    {
        anim.Play("battleJola_superedRecover",-1,0f);
    }

    void projectileSpawn()
    {
        Instantiate(_base.EnemyPjSelect(pjTag).EnemyProjectile);
    }

}
