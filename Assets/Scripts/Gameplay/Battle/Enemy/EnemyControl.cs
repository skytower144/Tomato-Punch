using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class EnemyControl : MonoBehaviour
{
    public EnemyBase _base { get; private set; }
    public Enemy_countered enemy_Countered;
    public EnemyAnimControl enemyAnimControl;
    public EnemyHitTypes enemyHitTypes;
    public DuplicateRenderer duplicate_r;

    public Enemy_is_hurt enemy_hurt => enemyHurt;
    public EnemyAIControl enemyAiControl => enemyAIControl;
    public Animator enemyAnim => anim;
    public Material mat_default => matDefault;
    public SpriteRenderer enemyRenderer => enemy_renderer;

    [System.NonSerialized] public List<DamageFrame> TotalDamageFrames = new List<DamageFrame>();
    private int _currentDamageFrameIndex = -1;
    private AttackType _attackType;

    public Transform AttackBoxSpawn => AttackBoxes;
    [SerializeField] private EnemyGreyEffect greyEffect;
    [SerializeField] private Animator anim; 
    [SerializeField] private SpriteRenderer enemy_renderer;
    [SerializeField] private GameObject counterBox;
    [SerializeField] private GameObject enemy_LA, enemy_RA, enemy_DA, enemy_PJ, enemy_Counter;
    [SerializeField] private GameObject defeatedEffect_pop, defeatedEffect_beam, defeatedEffect_flash, wallhitEffect, dunkSmoke, dunkSmoke2;
    [SerializeField] private Transform AttackBoxes;
    [SerializeField] private Animator tomatoAnim;
    [SerializeField] private tomatoGuard tomatoguard;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private EnemyAIControl enemyAIControl;
    [SerializeField] private Enemy_is_hurt enemyHurt;
    [SerializeField] private TextSpawn textSpawn;
    private Material matDefault;

    [System.NonSerialized] public static bool isPhysical = true;
    [System.NonSerialized] public bool action_afterSuffer = false;
    [System.NonSerialized] public bool enemy_supered = false;
    [System.NonSerialized] public bool canDunk = false;
    [System.NonSerialized] public bool isDunked = false;

    [System.NonSerialized] public AttackType attackType;
    [System.NonSerialized] public GameObject currentProjectile = null;

    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float flashDuration, hitFlashDuration;
    [System.NonSerialized] public float gangFightDmg = -1;
    
    public static int totalParry = 0;
    [System.NonSerialized] public int totalSuper = 0;

    void OnEnable()
    {
        disableBools();
        action_afterSuffer = false;
        GameManager.gm_instance.assistManager.SetIsBlast(false);

        matDefault = enemy_renderer.material;
        anim.runtimeAnimatorController = _base.AnimationController;
        enemyAnimControl.InitFrameDict(anim);
        InitEnemyPattern();

        enemyAIControl.InvokeRepeating("ProceedAction",1f,1f);
    }

    void OnDisable()
    {
        enemy_Countered.counter_is_initialized = false;
        ResetTotalDamageFrames();
        enemyAIControl.ResetEnemyPattern();

        CancelInvoke();
        enemyAIControl.CancelInvoke();
        enemyCounterEnd();

        gangFightDmg = -1;
    }
    
    void Update()
    {
        if(gatleCircleControl.failUppercut)
            RecoverAnimation();
        
        else if(tomatoControl.enemyUppered)
        {
            enemyAnimControl.Act(_base.Uppered_AnimationString, BattleActType.Uppered);
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
                enemyAnimControl.Act(_base.Dunk, BattleActType.Dunk);
        }
        else
        {
            if(tomatoControl.enemyFreeze)
            {
                tomatoControl.enemyFreeze = false;
    
                anim.enabled = false;
                EraseAllAttacks();
                enemyAnimControl.CancelScheduledInvokes();
                enemy_Countered.gameObject.SetActive(false);
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
        List<EnemyActDetail> deepCopiedPatterns = new List<EnemyActDetail>();
        int sumPercentage = 0;

        foreach (Enemy_NeutralDetail readingPattern in _base.EnemyNeutralPattern) {
            deepCopiedPatterns.Add(
                new Enemy_NeutralDetail(
                    readingPattern.Name,
                    readingPattern.percentage
            ));
            sumPercentage += readingPattern.percentage;
        }
        foreach (Enemy_AttackDetail readingPattern in _base.EnemyPattern) {
            deepCopiedPatterns.Add(
                new Enemy_AttackDetail(
                    readingPattern.Name,
                    readingPattern.percentage,
                    readingPattern.PhysicalAttackFrames
            ));
            sumPercentage += readingPattern.percentage;
        }
        foreach (Enemy_ProjectileDetail readingPattern in _base.EnemyProjectilePattern) {
            deepCopiedPatterns.Add(
                new Enemy_ProjectileDetail(
                    readingPattern.Name,
                    readingPattern.percentage,
                    readingPattern.ProjectileAttackFrames
            ));
            sumPercentage += readingPattern.percentage;
        }
        if (sumPercentage >= 100)
            Debug.LogError($"Enemy total action percentage error : {sumPercentage}");
        
        deepCopiedPatterns.Add(new Enemy_IdleDetail(_base.Idle_AnimationString, 100 - sumPercentage));
        
        enemyAIControl.LoadEnemyPattern(deepCopiedPatterns);
        GameManager.DoDebug($"Enemy idle percent : {100 - sumPercentage}%");
    }

    public void enemyIntroOver()
    {
        enemyAnimControl.Idle(_base.Idle_AnimationString);
    }
    public void actionOver()
    {
        enemy_supered = false;
        enemy_hurt.SetProjectileHit(false);

        if(!Enemy_countered.enemy_isCountered && !Enemy_is_hurt.enemy_isPunched)
            enemyAnimControl.Idle(_base.Idle_AnimationString);
    }
    public void enemy_isPunchedEnd()
    {
        Enemy_is_hurt.enemy_isPunched = false;
    }

    public void hurtOver()
    {
        Enemy_is_hurt.enemyIsHit = false;

        if(Enemy_parried.isParried && EnemyControl.isPhysical)                          // punching enemy when enemy is parried
            enemyAnimControl.Act(_base.Stun_AnimationString, BattleActType.Stun);
        
        else if(!Enemy_is_hurt.enemy_isPunched && Enemy_countered.enemy_isCountered)    // punching enemy when enemy is countered
            enemyAnimControl.Act(_base.Suffer_AnimationString, BattleActType.Suffer);
        
        else if(!Enemy_is_hurt.enemy_isPunched) {                                       // go back to idle when player did not attack
            enemyAnimControl.Idle(_base.Idle_AnimationString);
        }
    }

    public void enemyHurtFlash()
    {
        duplicate_r.InitFlash();
        duplicate_r.FlashEffect(hitFlashDuration, 0);
    }

    public void enemyCounterStart()
    {
        duplicate_r.InitFlash();
        duplicate_r.FlashEffect(flashDuration, 1);
        counterBox.SetActive(true);
    }
    public void enemyCounterEnd()
    {
        duplicate_r.InitFlash();
        counterBox.SetActive(false);
    }

    public void hitFrame() //depending on the animation, this function decides whether it should instantiate LA/RA/DA collider.            
    {
        _currentDamageFrameIndex++;
        _attackType = TotalDamageFrames[_currentDamageFrameIndex].EnemyAttackType;

        switch (_attackType) {
            case AttackType.LA:
                Instantiate (enemy_LA, AttackBoxes);
                break;
            
            case AttackType.RA:
                Instantiate (enemy_RA, AttackBoxes);
                break;
            
            case AttackType.DA:
                Instantiate (enemy_DA, AttackBoxes);
                break;
            
            case AttackType.PJ:
                Instantiate (enemy_PJ, AttackBoxes);
                break;
            
            default:
                break;
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
    public void DetermineCC()
    {
        Enemy_is_hurt.enemyIsHit = false;
        if (Enemy_countered.enemy_isCountered) beginSuffer();
        else if (Enemy_parried.isParried) beginStun();
        greyEffect.StartGreyEffect();
    }

    public void WallHitEffect()
    {
        Instantiate(wallhitEffect);
        StartCoroutine(WallHitCameraRumble());
    }

    IEnumerator WallHitCameraRumble()
    {
        yield return WaitForCache.GetWaitForSecondReal(0.02f);
        DOTween.Rewind("CameraBlast");
        DOTween.Play("CameraBlast");
    }

    public void EnableDunk()
    {
        canDunk = true;
        duplicate_r.FlashEffect(0.5f, 0);
    }

    public void BlastShrink()
    {
        transform.localPosition = new Vector3(216.77f, 121.93f, transform.localPosition.z);
        transform.localScale = new Vector3(21f, 21f, 21f);
    }

    public void RecoverShrink()
    {
        transform.localPosition = new Vector3(0f, 0f, transform.position.z);
        transform.localScale = new Vector3(45f, 45f, 45f);
    }
    
    public void DisableDunk()
    {
        canDunk = false;
    }

    public void Bounce()
    {
        enemyAnimControl.Act(_base.Bounce, BattleActType.Bounce);
    }

    public void DunkBounceSmoke2()
    {
        Instantiate(dunkSmoke2);
    }

    public void DunkBounceSmoke()
    {
        Instantiate(dunkSmoke);
    }

    void beginSuffer()
    {
        Invoke("return_CounterToIdle", 1.3f);
        if(!Enemy_is_hurt.enemy_isPunched)     // when enemy is hurt at the exact frame transitioning to the suffer animation, 
            enemyAnimControl.Act(_base.Suffer_AnimationString, BattleActType.Suffer);   // the 'if' statement makes it prioritize the hurt animation.
    }

    void beginStun()
    {
        enemyAnimControl.Act(_base.Stun_AnimationString, BattleActType.Stun);
    }
    void return_CounterToIdle()
    {
        greyEffect.StopGreyEffect();

        if(!Enemy_is_hurt.enemy_isDefeated){
            Enemy_countered.enemy_isCountered = false;
            Enemy_is_hurt.enemy_isPunched = false;
            
            action_afterSuffer = true;
            if(!Enemy_is_hurt.enemyIsHit){
                enemyAnimControl.Idle(_base.Idle_AnimationString);
            }
        }
    }

    public void CancelCounterState()
    {
        if (!Enemy_countered.enemy_isCountered) return;
        CancelInvoke("return_CounterToIdle");
        greyEffect.StopGreyEffect();
        Enemy_countered.enemy_isCountered = false;
    }

    public void RecoverAnimation()
    {
        if(Enemy_is_hurt.enemy_isDefeated) return;

        greyEffect.StopGreyEffect();
        enemyAnimControl.Act(_base.Recover_AnimationString, BattleActType.Recover);
    }

    public void projectileSpawn()
    {
        if (currentProjectile) Instantiate(currentProjectile, projectileSpawnPoint);
    }

    public void DestroyProjectiles()
    {
        foreach (Transform projectile in projectileSpawnPoint)
            Destroy(projectile.gameObject);
    }

    public void disableBools()
    {
        Enemy_is_hurt.enemy_isPunched = false;
        Enemy_is_hurt.enemy_isDefeated = false;
        Enemy_is_hurt.enemyIsHit = false;
        Enemy_countered.enemy_isCountered = false;
        Enemy_parried.isParried = false;
        tomatocontrol.enemy_supered = false;
        enemy_supered = false;
        canDunk = false;
        isDunked = false;
        enemy_hurt.SetProjectileHit(false);
    }

    public void guardDown() // apply to all enemy attack animations' first frame
    {
        enemyHurt.guardUp = false;
        enemyHurt.hitct = 0;
    }
    public void DisableIsGuarding()
    {
        enemyHurt.isGuarding = false;
    }

    public void freezeAnimation() // when KO // EnemyAnimControl
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

                yield return WaitForCache.GetWaitForSecondReal(0.8f);
                DOTween.Pause("ShakeEnemy");
                transform.localPosition = new Vector2(0, 0);
                anim.enabled = true;
                enemyAnimControl.Act(_base.Uppered_AnimationString, BattleActType.Uppered);
                break;

            default:
                break;

        }
        yield break;
    }
    public void UnFreeze()
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
        foreach (Transform attack in AttackBoxes)
        {
            Destroy(attack.gameObject);
        }
    }

    public void LoadEnemyBaseData(EnemyBase enemyBase)
    {
        _base = enemyBase;
    }

    public void SaveEnemyDmgFrames(List<PhysicalAttackFrame> damageFrames)
    {
        ResetTotalDamageFrames();
        _currentDamageFrameIndex = -1;
        
        foreach (PhysicalAttackFrame frame in damageFrames)
            TotalDamageFrames.Add(frame);
    }

    public void SaveEnemyDmgFrames(List<ProjectileAttackFrame> damageFrames)
    {
        ResetTotalDamageFrames();
        _currentDamageFrameIndex = -1;
        foreach (ProjectileAttackFrame frame in damageFrames)
            TotalDamageFrames.Add(frame);
    }
    public void ResetTotalDamageFrames()
    {
        TotalDamageFrames = new List<DamageFrame>();
    }

    public float GetCurrentAttackDamage()
    {
        if (_currentDamageFrameIndex >= 0 && _currentDamageFrameIndex < TotalDamageFrames.Count)
            return TotalDamageFrames[_currentDamageFrameIndex].Damage;
        
        else if (gangFightDmg != -1)
            return gangFightDmg;

        Debug.LogError($"Abnormal attack calculation detected. Current Damage Frame Index : {_currentDamageFrameIndex}");
        return 0;
    }
}