using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleJolaControl : MonoBehaviour
{
    private Animator anim;
    private GameObject counter_instance;
    [SerializeField] private GameObject jola_LA, jola_RA, jola_DA, jola_Counter;
    [SerializeField] private GameObject pj_1, pj_2, pj_3;
    [SerializeField] private Transform Parent;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private battleJola_is_hurt enemyHurt;
    [HideInInspector] public static bool isPhysical = true;
    private bool action_afterSuffer = false;
    private int attackType;
    private int pj = 0;     // pj selection number

    
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
        if(tomatoControl.enemyUppered)
        {
            anim.Play("battleJola_uppered",-1,0f);
            enemyHurt.enemyUpperedDamage();
        }
        if(tomatocontrol.enemyFreeze)
        {
            anim.enabled = false;
        }
    }

    void jolaAction()
    {
        if(action_afterSuffer)
        {
            action_afterSuffer = false;
            return;
        }
        else if(!battleJola_parried.isParried && !battleJola_countered.enemy_countered && !battleJola_is_hurt.enemy_isPunched)
        {
            
            if((Random.value<= 0.1))
            {
                anim.Play("battleJola_idle");
            }
            
            else if((Random.value<= 0.25))
            {
                attackType = -1;
                isPhysical = true;
                tomatoguard.damage = 5;
                anim.Play("battleJola_LA");
            }
            else if((Random.value<= 0.25))
            {
                attackType = 1;
                isPhysical = true;
                tomatoguard.damage = 5;
                anim.Play("battleJola_RA");
            }
            
            else if((Random.value<= 0.2))
            {
                attackType = 0;
                isPhysical = true;
                tomatoguard.damage = 8;
                anim.Play("battleJola_DA");
            }
            
            else if((Random.value<= 0.2))
            {
                attackType = -1;
                isPhysical = false; 
                pj = 1;
                tomatoguard.damage = 3;
                anim.Play("battleJola_HatAttack");
            }
            
        }
    }

    void actionOver()
    {
        if(!battleJola_countered.enemy_countered && !battleJola_is_hurt.enemy_isPunched)
            anim.Play("battleJola_idle",-1,0f);
    }
    void enemy_isPunchedEnd()
    {
        battleJola_is_hurt.enemy_isPunched = false;
    }
    void hurtOver()
    {
        if(battleJola_parried.isParried && battleJolaControl.isPhysical)                        // punching jola when jola is parried
            anim.Play("battleJola_parriedAft",-1,0f);
        else if(!battleJola_is_hurt.enemy_isPunched && battleJola_countered.enemy_countered)    // punching jola when jola is countered
            anim.Play("battleJola_suffer",-1,0f);
        else if(!battleJola_is_hurt.enemy_isPunched)                                            // go back to idle when player did not attack
            anim.Play("battleJola_idle",-1,0f);
    }

    void jolaCounterStart()
    {
        counter_instance = Instantiate (jola_Counter, Parent);
    }
    void jolaCounterEnd()
    {
        Destroy(counter_instance);
    }

    void jolaAttack() //depending on the animation, this function decides whether it should instantiate LA/RA/DA collider.
                      // -1: left, 1: right, 0: down                
    {
        if(attackType == -1)
        {   
            Instantiate (jola_LA, Parent);
        }
        else if(attackType == 1)
        {
            Instantiate (jola_RA, Parent);
        }
        else if(attackType == 0)
        {
            Instantiate (jola_DA, Parent);
        }
    }
    void beginSuffer()
    {
        Invoke("return_CounterToIdle", 3f);
        if(!battleJola_is_hurt.enemy_isPunched)     // when enemy is hurt at the exact frame transitioning to the suffer animation, 
            anim.Play("battleJola_suffer",-1,0f);   // the 'if' statement makes it prioritize the hurt animation.
    }

    void beginStun()
    {
        anim.Play("battleJola_parriedAft",-1,0f);
    }
    void return_CounterToIdle()
    {
        battleJola_is_hurt.enemy_isPunched = false;
        battleJola_countered.enemy_countered = false;
        action_afterSuffer = true;
    }

    void upperRecover()
    {
        anim.Play("battleJola_upperRecover",-1,0f);
    }

    void projectileSpawn()
    {
        if(pj==1)
        {
            Instantiate(pj_1);
        }
    }

}
