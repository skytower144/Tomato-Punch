using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class EnemyControl : MonoBehaviour
{
    [HideInInspector] public EnemyBase _base;

    [SerializeField] private Animator anim; public Animator enemyAnim => anim;
    [SerializeField] private SpriteRenderer enemy_renderer; public SpriteRenderer enemyRenderer => enemy_renderer;
    private Material matDefault; public Material mat_default => matDefault;

    [SerializeField] private EnemyGreyEffect greyEffect;
    [SerializeField] private DuplicateRenderer duplicate_r;
    [SerializeField] private GameObject counterBox;
    [SerializeField] private GameObject enemy_LA, enemy_RA, enemy_DA, enemy_PJ, enemy_Counter;
    [SerializeField] private GameObject defeatedEffect_pop, defeatedEffect_beam, defeatedEffect_flash, wallhitEffect, dunkSmoke, dunkSmoke2;
    [SerializeField] private Transform Parent;
    [SerializeField] private Animator tomatoAnim;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private EnemyAIControl enemyAIControl;
    [SerializeField] private Enemy_is_hurt enemyHurt;
    public Enemy_is_hurt enemy_hurt => enemyHurt;
    public Enemy_countered enemy_Countered;
    [SerializeField] private TextSpawn textSpawn;

    [System.NonSerialized] public static bool isPhysical = true;
    [System.NonSerialized] public bool action_afterSuffer = false;
    [System.NonSerialized] public bool enemy_supered = false;
    [System.NonSerialized] public bool canDunk = false;
    [System.NonSerialized] public bool isDunked = false;

    [System.NonSerialized] public AttackType attackType;
    [System.NonSerialized] public string pjTag;     // pj selection string

    [SerializeField] private float flashDuration, hitFlashDuration;
    
    public static int totalParry = 0;
    [System.NonSerialized] public int totalSuper = 0;

    void OnEnable()
    {
        disableBools();

        matDefault = enemy_renderer.material;
        anim.runtimeAnimatorController = _base.AnimationController;
        InitEnemyPattern();

        enemyAIControl.InvokeRepeating("ProceedAction",1f,1f);
    }

    void OnDisable()
    {
        enemy_Countered.counter_is_initialized = false;
        enemyAIControl.CancelInvoke();
    }
    
    void Update()
    {
        if(!Enemy_is_hurt.enemy_isDefeated && gatleCircleControl.failUppercut)
            RecoverAnimation();
        
        else if(tomatoControl.enemyUppered)
        {
            anim.Play(_base.Uppered_AnimationString,-1,0f);
            enemyHurt.ParryBonus();

            enemyHurt.enemyHurtDamage(tomatocontrol.dmg_upperPunch);
            if (enemyHurt.Enemy_currentHealth == 0){
                super_upper_KO();
            }

            greyEffect.StopGreyEffect();
        }
        else if (isDunked) {
            isDunked = false;
            enemyHurt.enemyHurtDamage(tomatocontrol.dmg_dunk);

            if (!enemyHurt.checkDefeat())
                anim.Play(_base.Dunk, -1, 0f);
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

                StartCoroutine(SuperAnimation(tomatocontrol.tomatoSuperEquip.ItemName));
                
                enemyHurt.enemyHurtDamage(tomatocontrol.dmg_super);
                if (enemyHurt.Enemy_currentHealth == 0){
                    super_upper_KO();
                }

                greyEffect.StopGreyEffect();
            }
        }
    }

    private void InitEnemyPattern()
    {
        List<Enemy_AttackDetail> deepCopiedPatterns = new List<Enemy_AttackDetail>();
        int sumPercentage = 0;

        foreach (Enemy_AttackDetail readingPattern in _base.EnemyPattern) {
            deepCopiedPatterns.Add(
                new Enemy_AttackDetail(
                    readingPattern.EnemyAttackName,
                    readingPattern.PhysicalAttack,
                    readingPattern.percentage,
                    readingPattern.EnemyAttackDmg,
                    readingPattern.EnemyAttackType
            ));
            sumPercentage += readingPattern.percentage;
        }
        if (sumPercentage >= 100)
            Debug.LogError($"Enemy total action percentage error : {sumPercentage}");
        
        deepCopiedPatterns.Add(
            new Enemy_AttackDetail(_base.Idle_AnimationString, false, 100 - sumPercentage, 0, AttackType.NEUTRAL)
        );
        enemyAIControl.LoadEnemyPattern(deepCopiedPatterns);
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
            anim.Play(_base.Stun_AnimationString,-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched && Enemy_countered.enemy_isCountered)    // punching enemy when enemy is countered
            anim.Play(_base.Suffer_AnimationString,-1,0f);
        else if(!Enemy_is_hurt.enemy_isPunched){                                            // go back to idle when player did not attack
            anim.Play(_base.Idle_AnimationString,-1,0f);
        }
    }

    public void enemyHurtFlash()
    {
        duplicate_r.FlashEffect(hitFlashDuration, 0);
    }

    void enemyCounterStart()
    {
        duplicate_r.FlashEffect(flashDuration, 1);
        counterBox.SetActive(true);
    }
    void enemyCounterEnd()
    {
        counterBox.SetActive(false);
    }

    void hitFrame() //depending on the animation, this function decides whether it should instantiate LA/RA/DA collider.            
    {
        if(attackType == AttackType.LA)
        {   
            Instantiate (enemy_LA, Parent);
        }
        else if(attackType == AttackType.RA)
        {
            Instantiate (enemy_RA, Parent);
        }
        else if(attackType == AttackType.DA)
        {
            Instantiate (enemy_DA, Parent);
        }
        else if(attackType == AttackType.PJ)
        {
            Instantiate (enemy_PJ, Parent);
        }
    }

    public void detectEvasion()
    {
        if(!tomatoHurt.isTomatoHurt && !tomatoControl.isGuard)
        {
            tomatocontrol.currentStamina += GameManager.gm_instance.battle_system.evadeStamina;
            if (tomatocontrol.currentStamina > tomatocontrol.maxStamina)
                tomatocontrol.currentStamina = tomatocontrol.maxStamina;
            
            staminaIcon.SetStamina(tomatocontrol.currentStamina);
            GameManager.gm_instance.battle_system.featherPointManager.AddFeatherPoint();
        }
    }
    void DetermineCC()
    {
        if (Enemy_countered.enemy_isCountered) beginSuffer();
        else if (Enemy_parried.isParried) beginStun();
        greyEffect.StartGreyEffect();
    }

    void WallHitEffect()
    {
        Instantiate(wallhitEffect);
        StartCoroutine(WallHitCameraRumble());
    }

    IEnumerator WallHitCameraRumble()
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(0.02f));
        DOTween.Rewind("CameraBlast");
        DOTween.Play("CameraBlast");
    }

    void EnableDunk()
    {
        canDunk = true;
        duplicate_r.FlashEffect(0.5f, 0);
    }
    
    void DisableDunk()
    {
        canDunk = false;
    }

    void Bounce()
    {
        if (!tomatoAnim.GetCurrentAnimatorStateInfo(0).IsName("tomato_dunk"))
            anim.Play(_base.Bounce, -1, 0f);
    }

    void Dunk2Bounce()
    {
        anim.Play(_base.Bounce, -1, 0f);
    }

    void DunkSmoke1_2(int number)
    {
        Instantiate(dunkSmoke);
        if (number == 2)
            Instantiate(dunkSmoke2);
    }

    void beginSuffer()
    {
        Invoke("return_CounterToIdle", 1.3f);
        if(!Enemy_is_hurt.enemy_isPunched)     // when enemy is hurt at the exact frame transitioning to the suffer animation, 
            anim.Play(_base.Suffer_AnimationString,-1,0f);   // the 'if' statement makes it prioritize the hurt animation.
    }

    void beginStun()
    {
        anim.Play(_base.Stun_AnimationString,-1,0f);
    }
    void return_CounterToIdle()
    {
        greyEffect.StopGreyEffect();

        if(!Enemy_is_hurt.enemy_isDefeated){
            Enemy_countered.enemy_isCountered = false;
            Enemy_is_hurt.enemy_isPunched = false;
            
            action_afterSuffer = true;
            if(!Enemy_is_hurt.enemyIsHit){
                anim.Play(_base.Idle_AnimationString);
            }
        }
    }

    private void RecoverAnimation()
    {
        if(Enemy_is_hurt.enemy_isDefeated) return;

        greyEffect.StopGreyEffect();
        anim.Play(_base.Recover_AnimationString,-1,0f);
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
        tomatocontrol.enemy_supered = false;
        enemy_supered = false;
        canDunk = false;
        isDunked = false;
        
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

    IEnumerator SuperAnimation(string equip_name)
    {
        switch (equip_name) {
            case "SuperChili":
                transform.localPosition = _base.chiliInfo.hitPosition;
                enemy_renderer.sprite = _base.chiliInfo.hitSprite;
                DOTween.Rewind("ShakeEnemy");
                DOTween.Play("ShakeEnemy");

                yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(0.8f));
                DOTween.Pause("ShakeEnemy");
                transform.localPosition = new Vector2(0, 0);
                anim.enabled = true;
                anim.Play(_base.Uppered_AnimationString);
                break;

            default:
                break;

        }
        yield break;
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

    public void EraseAllAttacks()
    {
        foreach (Transform attack in Parent.transform)
        {
            Destroy(attack.gameObject);
        }
    }
}