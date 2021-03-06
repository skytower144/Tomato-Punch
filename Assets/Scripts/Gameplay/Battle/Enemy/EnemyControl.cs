using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyControl : MonoBehaviour
{
    public EnemyBase _base;
    private Animator anim;
    private SpriteRenderer enemy_renderer;
    private Material matDefault;
    [SerializeField] private GameObject duplicate_r, counterBox;
    [SerializeField] private GameObject enemy_LA, enemy_RA, enemy_DA, enemy_PJ, enemy_Counter;
    [SerializeField] private GameObject defeatedEffect_pop, defeatedEffect_beam, defeatedEffect_flash;
    [SerializeField] private Transform Parent;
    [SerializeField] private Animator tomatoAnim;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private EnemyAIControl enemyAIControl;
    [SerializeField] private Enemy_is_hurt enemyHurt;
    [SerializeField] private Enemy_countered enemy_Countered;
    [SerializeField] private TextSpawn textSpawn;
    [HideInInspector] public static bool isPhysical = true;
    [HideInInspector] public bool action_afterSuffer = false;
    [HideInInspector] public bool enemy_supered = false;
    [HideInInspector] public int attackType;
    [HideInInspector] public string pjTag;     // pj selection string
    
    public static int totalParry = 0;
    public int totalSuper = 0;

    void OnEnable()
    {
        disableBools();

        enemy_renderer = GetComponent<SpriteRenderer>();
        matDefault = enemy_renderer.material;

        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = _base.AnimationController;
        enemyAIControl.InvokeRepeating(_base.EnemyName,1f,2f);
    }

    void OnDisable()
    {
        enemy_Countered.counter_is_initialized = false;
        enemyAIControl.CancelInvoke();
    }
    
    void Update()
    {
        if(!Enemy_is_hurt.enemy_isDefeated && gatleCircleControl.failUppercut)
        {
            anim.Play(_base.ParriedToIdle_AnimationString,-1,0f);
        }
        else if(tomatoControl.enemyUppered)
        {
            anim.Play(_base.Uppered_AnimationString,-1,0f);
            enemyHurt.ParryBonus();

            enemyHurt.enemyHurtDamage(tomatocontrol.dmg_upperPunch);
            if (enemyHurt.Enemy_currentHealth == 0){
                super_upper_KO();
            }
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
                totalSuper += 1;
                
                tomatocontrol.enemy_supered = false;
                enemy_supered = true;

                anim.enabled = true;
                anim.Play(_base.EnemySuperedAnim[tomatocontrol.tomatoSuper],-1,0f); 
                // Depending on tomatocontrol.tomatoSuper index, choose Enemy supered animation
                
                enemyHurt.enemyHurtDamage(tomatocontrol.dmg_super);
                if (enemyHurt.Enemy_currentHealth == 0){
                    super_upper_KO();
                }
            }
        }
        
    }

    void enemyIntroOver()
    {
        anim.Play(_base.Idle_AnimationString,-1,0f);
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
        Enemy_is_hurt.enemyIsHit = false;
        if(Enemy_parried.isParried && EnemyControl.isPhysical)                             // punching enemy when enemy is parried
            anim.Play(_base.ParriedAft_AnimationString,-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched && Enemy_countered.enemy_isCountered)    // punching enemy when enemy is countered
            anim.Play(_base.Suffer_AnimationString,-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched){                                            // go back to idle when player did not attack
            anim.Play(_base.Idle_AnimationString,-1,0f);
        }
    }

    void enemyCounterFlash(float flashDuration)
    {
        Invoke("ResetFlash", flashDuration);
        duplicate_r.GetComponent<DuplicateRenderer>().flashSpeed = (1 - flashDuration) * 0.001f;
        duplicate_r.SetActive(true);
        enemyCounterStart();
    }
    public void ResetFlash()
    {
        enemyCounterEnd();
        enemy_renderer.material = matDefault;
        duplicate_r.SetActive(false);
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
        if(!tomatoHurt.isTomatoHurt && !tomatoControl.isGuard)
        {
            tomatocontrol.currentStamina += 5;
            if (tomatocontrol.currentStamina > tomatocontrol.maxStamina)
                tomatocontrol.currentStamina = tomatocontrol.maxStamina;
            
            staminaIcon.SetStamina(tomatocontrol.currentStamina);
        }
    }
    void beginSuffer()
    {
        Invoke("return_CounterToIdle", 2f);
        if(!Enemy_is_hurt.enemy_isPunched)     // when enemy is hurt at the exact frame transitioning to the suffer animation, 
            anim.Play(_base.Suffer_AnimationString,-1,0f);   // the 'if' statement makes it prioritize the hurt animation.
    }

    void beginStun()
    {
        anim.Play(_base.ParriedAft_AnimationString,-1,0f);
    }
    void return_CounterToIdle()
    {
        if(!Enemy_is_hurt.enemy_isDefeated){
            Enemy_countered.enemy_isCountered = false;
            Enemy_is_hurt.enemy_isPunched = false;
            
            action_afterSuffer = true;
            if(!Enemy_is_hurt.enemyIsHit){
                anim.Play(_base.Idle_AnimationString);
            }
        }
    }

    void upperRecover()
    {
        if(!Enemy_is_hurt.enemy_isDefeated)
            anim.Play(_base.UpperRecover_AnimationString,-1,0f);
    }

    void superedRecover()
    {
        if(!Enemy_is_hurt.enemy_isDefeated)
            anim.Play(_base.SuperedRecover_AnimationString,-1,0f);
    }

    void projectileSpawn()
    {
        Instantiate(_base.EnemyPjSelect(pjTag).EnemyProjectile);
    }

    void disableBools()
    {
        Enemy_is_hurt.enemy_isPunched = false;
        Enemy_is_hurt.enemy_isDefeated = false;
        Enemy_countered.enemy_isCountered = false;
        Enemy_parried.isParried = false;
        enemy_supered = false;
        
        action_afterSuffer = false;
    }

    void guardDown() // apply to all enemy attack animations' first frame
    {
        enemyHurt.guardUp = false;
        enemyHurt.hitct = 0;
    }

    void freezeAnimation() // when KO
    {
        Invoke("UnFreeze", 0.6f);
        anim.enabled = false;
    }
    private void UnFreeze()
    {
        anim.enabled = true;
        tomatoAnim.enabled = true;
        Instantiate(defeatedEffect_pop);
        textSpawn.spawn_KO_text();

        DOTween.Rewind("CameraShake");
        DOTween.Play("CameraShake");
    }

    private void super_upper_KO() // if supered or uppered -> KO
    {
        tomatoControl.isVictory = true;
        Enemy_is_hurt.enemy_isDefeated = true;
        Instantiate(defeatedEffect_beam);
    }

    public void ClearAnimation()
    {
        anim.runtimeAnimatorController = null;
    }

}
