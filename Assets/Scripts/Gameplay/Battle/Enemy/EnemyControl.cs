using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public EnemyBase _base;
    private Animator anim;
    [SerializeField] private GameObject counterBox;
    [SerializeField] private GameObject enemy_LA, enemy_RA, enemy_DA, enemy_PJ, enemy_Counter;
    [SerializeField] private Transform Parent;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private Animator tomatoAnim;
    [SerializeField] private Enemy_is_hurt enemyHurt;
    [HideInInspector] public static bool isPhysical = true;
    [HideInInspector] public bool action_afterSuffer = false;
    [HideInInspector] public bool enemy_supered = false;
    [HideInInspector] public int attackType;
    [HideInInspector] public string pjTag;     // pj selection string
    [SerializeField] private EnemyAIControl enemyAIControl;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = _base.AnimationController;
        enemyAIControl.InvokeRepeating(_base.EnemyName,1f,2f);
    }
    
    void Update()
    {
        if(gatleCircleControl.failUppercut)
        {
            anim.Play(_base.ParriedToIdle_AnimationString,-1,0f);
        }
        else if(tomatoControl.enemyUppered)
        {
            anim.Play(_base.Uppered_AnimationString,-1,0f);
            enemyHurt.ParryBonus();
            enemyHurt.enemyHurtDamage(tomatocontrol.dmg_upperPunch);
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
                anim.Play(_base.EnemySuperedAnim[tomatocontrol.tomatoSuper],-1,0f); 
                // Depending on tomatocontrol.tomatoSuper index, choose Enemy supered animation
                
                enemyHurt.enemyHurtDamage(tomatocontrol.dmg_super);
            }
        }
        
    }
    void actionOver()
    {
        enemy_supered = false;
        if(!Enemy_countered.enemy_isCountered && !Enemy_is_hurt.enemy_isPunched)
            anim.Play(_base.Idle_AnimationString,-1,0f);
    }
    void enemy_isPunchedEnd()
    {
        Enemy_is_hurt.enemy_isPunched = false;
    }
    void hurtOver()
    {
        if(Enemy_parried.isParried && EnemyControl.isPhysical)                             // punching enemy when enemy is parried
            anim.Play(_base.ParriedAft_AnimationString,-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched && Enemy_countered.enemy_isCountered)    // punching enemy when enemy is countered
            anim.Play(_base.Suffer_AnimationString,-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched)                                            // go back to idle when player did not attack
            anim.Play(_base.Idle_AnimationString,-1,0f);
    }

    void enemyCounterStart()
    {
        counterBox.SetActive(true);
    }
    void enemyCounterEnd()
    {
        counterBox.SetActive(false);
    }

    void hitFrame() //depending on the animation, this function decides whether it should instantiate LA/RA/DA collider.
                      // -1: left, 1: right, -101: down, 0: center                
    {
        if(attackType == -1)
        {   
            Instantiate (enemy_LA, Parent);
        }
        else if(attackType == 1)
        {
            Instantiate (enemy_RA, Parent);
        }
        else if(attackType == -101)
        {
            Instantiate (enemy_DA, Parent);
        }
        else if(attackType == 0)
        {
            Instantiate (enemy_PJ, Parent);
        }
    }

    void detectEvasion()
    {
        if(tomatoAnim.GetCurrentAnimatorStateInfo(0).IsName("tomato_Levade") || tomatoAnim.GetCurrentAnimatorStateInfo(0).IsName("tomato_Revade") || tomatoAnim.GetCurrentAnimatorStateInfo(0).IsName("tomato_jump"))
        {
            tomatocontrol.currentStamina += 5;
            if (tomatocontrol.currentStamina > tomatocontrol.maxStamina)
                tomatocontrol.currentStamina = tomatocontrol.maxStamina;
            
            staminaIcon.SetStamina(tomatocontrol.currentStamina);
        }
    }
    void beginSuffer()
    {
        Invoke("return_CounterToIdle", 3f);
        if(!Enemy_is_hurt.enemy_isPunched)     // when enemy is hurt at the exact frame transitioning to the suffer animation, 
            anim.Play(_base.Suffer_AnimationString,-1,0f);   // the 'if' statement makes it prioritize the hurt animation.
    }

    void beginStun()
    {
        anim.Play(_base.ParriedAft_AnimationString,-1,0f);
    }
    void return_CounterToIdle()
    {
        Enemy_is_hurt.enemy_isPunched = false;
        Enemy_countered.enemy_isCountered = false;
        action_afterSuffer = true;
        anim.Play(_base.Idle_AnimationString);
    }

    void upperRecover()
    {
        anim.Play(_base.UpperRecover_AnimationString,-1,0f);
    }

    void superedRecover()
    {
        anim.Play(_base.SuperedRecover_AnimationString,-1,0f);
    }

    void projectileSpawn()
    {
        Instantiate(_base.EnemyPjSelect(pjTag).EnemyProjectile);
    }

    void disableBools()
    {
        Enemy_is_hurt.enemy_isPunched = false;
        Enemy_countered.enemy_isCountered = false;
        Enemy_parried.isParried = false;
        enemy_supered = false;
        
        action_afterSuffer = true;
    }

    void guardDown() // apply to all enemy attack animations' first frame
    {
        enemyHurt.guardUp = false;
        enemyHurt.hitct = 0;
    }
}
