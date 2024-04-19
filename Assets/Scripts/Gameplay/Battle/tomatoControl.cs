using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class tomatoControl : MonoBehaviour
{
    public Animator tomatoAnim => tomatoAnimator;
    public GuardBar guard_bar => guardBar;
    public ParryBar parry_bar => parryBar;

    [SerializeField] private PlayerInput tomatoInput;
    [SerializeField] private Animator gatleButton_anim_L, gatleButton_anim_R, gaksung_objAnim, gaksung_anim;

    public GameObject deflectLaser;
    [SerializeField] private GameObject gaksung_OBJ, tomato_LP, tomato_RP, tomato_G, tomato_PRY, tomato_S;
    [SerializeField] private GameObject gatleSmoke_L, gatleSmoke_R, upperBg, upper_hitef, upper_hitef2, upperSmoke, superBanner, screenFlash, defeatedEffect_pop, faintStars, blastEffect, dunkEffect, dunkEffect2, sparkleEffect;
    [System.NonSerialized] public GameObject tempObj = null;
    [SerializeField] private Transform Parent, BattleCanvas_Parent;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GuardBar guardBar;
    [SerializeField] private ParryBar parryBar;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private CounterTrack counterTrack;
    [SerializeField] private tomatoHurt tomatohurt;
    [SerializeField] private TextSpawn textSpawn;
    [SerializeField] private FlashEffect flashEffect;

    private EnemyControl enemyControl;
    private Animator tomatoAnimator; 
    private GameObject _parryInstance;
    private BattleUI_Control battleUIControl;
    
    //========================================================================================================================

    [Header("EQUIP STATE")]
    public SuperEquip tomatoSuperEquip;
    public List <Equip> tomatoEquip;
    
    //Tomato Info: ============================================================================================================
    //[System.NonSerialized] 
    public float maxHealth, currentHealth;
    public float maxGuard, current_guardPt;
    public float tomatoAtk;
    public int maxStamina, currentStamina;
    [System.NonSerialized] public float dmg_normalPunch, dmg_gatlePunch, dmg_upperPunch, dmg_super, dmg_dunk;

    //Animation States: ======================================================================================================
    const string TOMATO_IDLE = "tomato_idle";
    const string TOMATO_LEVADE = "tomato_Levade"; const string TOMATO_REVADE = "tomato_Revade"; const string TOMATO_JUMP = "tomato_jump";
    const string TOMATO_LP = "tomato_LP"; const string TOMATO_RP = "tomato_RP";
    const string TOMATO_GUARD = "tomato_guard"; const string TOMATO_GUARDAFT = "tomato_guardAft";
    const string TOMATO_GATLING = "tomato_gatling"; const string TOMATO_GATLINGIDLE = "tomato_gatlingIdle"; const string TOMATO_GLP = "tomato_GLP"; const string TOMATO_GRP = "tomato_GRP";
    //========================================================================================================================
    public bool IsAction => isAction;
    public bool IsPunch => isPunch;
    public bool GuardRelease => guardRelease;
    public bool IsTired => isTired;

    private bool isAction = false;
    private bool isPunch = false;   // smoothen punch input (enabling cancel)
    private bool guardRelease = false;      // prevent multiple animations trying to play at a single frame (esp during animation transition)
    private bool smoothJump = false;
    private bool isTired = false;
    private bool isReviving = false;
    [System.NonSerialized] public static bool isGatle = false; 
    [System.NonSerialized] public static bool isGuard = false;
    [System.NonSerialized] public bool isMiss = false;

    [System.NonSerialized] public static bool isIntro = true;
    [System.NonSerialized] public static bool isVictory = false;
    [System.NonSerialized] public static bool isFainted = false;

    [System.NonSerialized] public static bool gatleButton_once = false;  // play a line once
    [System.NonSerialized] public static bool uppercutYes = false;       // player succeeded uppercut
    [System.NonSerialized] public static bool enemyUppered = false;
    [System.NonSerialized] public static bool enemyFreeze = false;
    [System.NonSerialized] public bool enemy_supered = false;

    [System.NonSerialized] public SkillType currentSkillType;
    [System.NonSerialized] public int tomatoes = 0;
    private int skillEffectIndex = -1;
    private int equipIndex = -1;

    void OnEnable()
    {
        isAction = isPunch = isGatle = isGuard = isMiss = false;
        guardRelease = false;

        isIntro = true;
        isVictory = isFainted = isTired = tomatoGuard.isParry = gatleButton_once = uppercutYes = enemyUppered = enemyFreeze = enemy_supered = false;

        dmg_normalPunch = tomatoDamage.NormalPunch(tomatoAtk);
        dmg_gatlePunch = tomatoDamage.GatlePunch(tomatoAtk);
        dmg_upperPunch = tomatoDamage.UpperPunch(tomatoAtk);
        if (tomatoSuperEquip) dmg_super = tomatoDamage.SkillAttack(tomatoAtk, tomatoSuperEquip.skillDamage);
        dmg_dunk = tomatoDamage.DunkAttack(tomatoAtk);

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        healthBar.setDamageFill();

        guardBar.SetMaxGuard(maxGuard);
        current_guardPt = maxGuard;
        guardBar.SetGuardbar(current_guardPt);

        ResetGaksung();

        staminaIcon.SetMaxStamina(maxStamina);
        currentStamina = maxStamina;
        staminaIcon.SetStamina(currentStamina);

        tomatoes = 0;
        counterTrack.CounterTracker();
        tomatohurt.SetHitBox(true);

        GameManager.gm_instance.SwitchActionMap("Battle");
    }
    void OnDisable()
    {
        GameManager.gm_instance.SwitchActionMap("Player");
    }
    void Start()
    {
        tomatoAnimator = GetComponent<Animator>();
        enemyControl = battleSystem.enemy_control;
        battleUIControl = battleSystem.battleUI_Control;
    }

    void ChangeAnimationState(string newState)
    {
        tomatoAnimator.Play(newState);
    }

    public IEnumerator ChangeAnimationState(string animName, float wait)
    {
        yield return WaitForCache.GetWaitForSecond(wait);
        tomatoAnimator.Play(animName, -1, 0f);
    }

    void Update()
    {
        if(!tomatoHurt.isTomatoHurt && !isIntro && !isFainted && !isVictory && !battleSystem.IsNextPhase)
        {
            if(!isAction)
            {
                if (PressKey("LeftEvade"))
                {
                    ChangeAnimationState(TOMATO_LEVADE);
                }
                else if (PressKey("RightEvade"))
                {
                    ChangeAnimationState(TOMATO_REVADE);
                }
                else if (PressKey("Jump"))
                {
                    ChangeAnimationState(TOMATO_JUMP);
                }
                else if (PressKey("LeftPunch"))
                {
                    if (enemyControl.canDunk) {
                        enemyControl.canDunk = false;
                        tomatoAnimator.Play("tomato_dunk", -1, 0f);
                    }

                    else if (!isTired)
                        ChangeAnimationState(TOMATO_LP);
                    
                    else
                        tomatoAnimator.Play("tomato_tiredPunch_L",-1,0f);
                }
                else if (PressKey("RightPunch"))
                {
                    if (enemyControl.canDunk) {
                        enemyControl.canDunk = false;
                        tomatoAnimator.Play("tomato_dunk", -1, 0f);
                    }

                    else if (!isTired)
                        ChangeAnimationState(TOMATO_RP);
                    
                    else 
                        tomatoAnimator.Play("tomato_tiredPunch_R",-1,0f);
                }
                else if (!isTired)
                {
                    if (PressKey("Guard"))
                    {
                        guardRelease = false;

                        ChangeAnimationState(TOMATO_GUARD);
                        tomato_G.SetActive(true);
                    }
                    else if (PressKey("SuperSkill"))
                    {
                        if((tomatoSuperEquip != null) && parryBar.gaksungOn)
                        {
                            ResetGaksung();

                            superBanner.SetActive(true);
                            enemyFreeze = true;
                            tomatoAnimator.enabled = false;
                        }
                    }
                    else if (PressKey("FirstEquip"))
                    {
                        if((tomatoEquip[0] != null) && tomatoes > 0){
                            currentSkillType = SkillType.Equip_Skill;
                            equipIndex = 0;

                            tomatoAnimator.Play(tomatoEquip[0].SkillAnimation,-1,0f);

                            tomatoes -= 1;
                            counterTrack.CounterTracker();
                        }
                    }
                    else if (PressKey("AssistSkill1"))
                    {
                        if (battleSystem.featherPointManager.feather_point >= 1)
                        {
                            currentSkillType = SkillType.Assist_Skill;
                            tomatoAnimator.Play(GameManager.gm_instance.assistManager.DecideSkill(1, battleSystem.featherPointManager.feather_point), -1, 0f);
                        }
                    }
                }
            }
            else if (isPunch && !isMiss)
            {
                if (PressKey("LeftPunch"))
                {
                    ChangeAnimationState(TOMATO_LP);
                }
                else if (PressKey("RightPunch"))
                {
                    ChangeAnimationState(TOMATO_RP);
                }
            }
            else if (tomatoInput.actions["Guard"].WasReleasedThisFrame())
                guardRelease = true;
            
            else if (smoothJump && PressKey("Jump"))
                tomatoAnimator.Play("tomato_jump", -1, 0f);

            if((!battleUIControl.stopGatle) && tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GATLINGIDLE))
                gatlePunch();

            if(isGatle)
            {
                if((!battleUIControl.stopGatle) && (tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GLP) || tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GRP)))
                    gatlePunch();
            }
            if (isGuard && guardRelease && !battleUIControl.IsGatleMode && !battleUIControl.stopGatle)
                ReleaseGuard();
        }
    }

    // FUNCTIONS ====================================================================================================================

    void IntroOver()
    {
        ChangeAnimationState(TOMATO_IDLE);
    }
    void actionStart()
    {
        isAction = true;
        isPunch = false;
        smoothJump = false;
    }
    void actionEnd()
    {
        if (tomatoHurt.isTomatoHurt)
            return;

        if (currentStamina == 0) {
            tomatoAnimator.Play("tomato_tired",-1,0f);
            isTired = true;
        }
        else if (!isFainted || isReviving) {
            ChangeAnimationState(TOMATO_IDLE);
            isTired = false;
        }
        isReviving = false;
        smoothJump = false;
        isPunch = false;
        isAction = false;
    }
    void smoothPunch() //allows to rapidly switch between LP and RP
    {
        isPunch = true;
    }
    void smoothGatle()
    {
        isGatle = true;
    }
    void SmoothJump()
    {
        smoothJump = true;
    }
    void gatlePunch()
    {
        if (PressKey("LeftPunch"))
        {
            if(gatleCircleControl.failUppercut == false)
            {
                tomatoAnimator.Play("tomato_GLP",-1,0f);
            }
        }
        if (PressKey("RightPunch"))
        {
            if(gatleCircleControl.uppercut_time)
            {
                uppercutYes = true;
            }
            else
                tomatoAnimator.Play("tomato_GRP",-1,0f);
        }
    }

    public void tomatoHurtStart()      //prevents from initiating action while hurt , reset all booleans except isAction
    {
        if (!tomatoHurt.isTomatoHurt)
            tomatoHurt.isTomatoHurt = true;
        
        isAction = true;
        isPunch = false;
        isGuard = false;

        if (tomatoGuard.isParry){
            tomatoGuard.isParry = false;
            tomatoes = 0;
            counterTrack.CounterTracker();
        }
        else if(tomatoes>0){
            tomatoes -= 1;
            counterTrack.CounterTracker();
        }
    }
    public void tomatoHurtOver()
    {
        tomatoGuard.preventDamageOverlap = false;
        tomatoHurt.isTomatoHurt = false;
        actionEnd();
    }

    void guardStart()
    {
        if(!guardRelease)
        {
            isAction = true;
            isGuard = true;
            isPunch = false;
            smoothJump = false;
            guardBar.regainGuardTimer = GuardBar.G_REGAINTIMER_MAX;
        }
    }
    void return_to_Guard()
    {
        if (Enemy_parried.isParried && EnemyControl.isPhysical)
        {
            isGuard = false;
            tomatoGuard.isParry = false;
            ChangeAnimationState(TOMATO_GATLING);
        }
        else
        {
            if (tomatoGuard.isParry)
            {
                tomatoGuard.isParry = false;
                tomato_G.SetActive(true);
            }
            if (isVictory || battleSystem.IsNextPhase)
                ReleaseGuard();

            else if (!guardRelease)
                ChangeAnimationState(TOMATO_GUARD);
        }
    }

    public void ReleaseGuard()
    {
        guardRelease = false;
        tomatoGuard.preventDamageOverlap = false;
        tomatoGuard.isParry = false;
        isGuard = false;
        isAction = false;

        Destroy(_parryInstance);
        tomatoAnimator.Play("tomato_idle",-1,0f);
        tomatohurt.SetHitBox(true);
    }

    void punchActivate()
    {
        if(tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_LP) || tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName("tomato_tiredPunch_L"))
        {
            Instantiate (tomato_LP, Parent);
        }
        else if(tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_RP) || tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName("tomato_tiredPunch_R"))
        {
            Instantiate (tomato_RP, Parent);
        }
    }
    void parryActivate()
    {
        if(!tomatoHurt.isTomatoHurt)
        {
            tomatohurt.SetHitBox(false);
            _parryInstance = Instantiate (tomato_PRY, Parent);
            Invoke("parryDeactivate",0.05f);
        }
    }

    void parryDeactivate()
    {
        Destroy(_parryInstance);
        if(!Enemy_parried.isParried) {
            tomatohurt.SetHitBox(true);
        }
    }

    void gatlingReady()
    {
        if(!battleUIControl.stopGatle && !gatleCircleControl.failUppercut && !uppercutYes)
        {
            isGatle = false;
            tomatoAnimator.Play("tomato_gatlingIdle",-1,0f);
        }
    }

    void endGatle()
    {
        battleUIControl.stopGatle = false;
        tomatohurt.SetHitBox(true);
    }

    void gatlingPunch()
    {
        if(!battleUIControl.stopGatle)
        {
            if(tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GLP))
            {
                gatleButton_anim_L.Play("gatleButton_L_keydown",-1,0f);
                if(!gatleButton_once)
                {
                    gatleButton_once = true;
                    gatleButton_anim_R.Play("gatleButton_R_keyup",-1,0f);
                }
                Instantiate (gatleSmoke_L, new Vector2 (transform.position.x + 0.2f, transform.position.y - 1.5f), Quaternion.identity);
                Instantiate (tomato_LP, Parent);
            }
            else if(tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GRP))
            {
                gatleButton_anim_R.Play("gatleButton_R_keydown",-1,0f);
                if(!gatleButton_once)
                {
                    gatleButton_once = true;
                    gatleButton_anim_L.Play("gatleButton_L_keyup",-1,0f);
                }
                Instantiate (gatleSmoke_R, new Vector2 (transform.position.x + 0.3f, transform.position.y - 1.6f), Quaternion.identity);
                Instantiate (tomato_RP, Parent);
            }
        }
    }

    void playUppercut()
    {
        tomatoAnimator.Play("tomato_uppercut",-1,0f);
    }

    void endUppercut()
    {
        battleUIControl.stopGatle = false;
        gatleButton_once = false;
        tomatohurt.SetHitBox(true);
    }
    void enemy_Uppered()
    {
        enemyUppered = true;
        Instantiate (upper_hitef2);
        Instantiate (upperBg);
    }
    void upper_hitEf()
    {
        Instantiate (upper_hitef);
        Instantiate (upperSmoke);
    }
    
    void enemy_Dunked()
    {
        enemyControl.CancelInvoke("Bounce");
        enemyControl.isDunked = true;
    }

    void revive_to_idle()
    {
        isReviving = true;
        textSpawn.spawn_FIGHT_text();

        enemyControl.enemyAnimControl.ReEngage();
        enemyControl.guardDown();

        battleSystem.resetPlayerHealth = true;
        enemyControl.enemy_hurt.EnemyHealthBar.SetIncreaseHealthAmount(-1, true);

        textSpawn.normalize_resultCard();
        battleSystem.featherPointManager.ResetFeather();
        battleSystem.tomato_control.guard_bar.RestoreGuardBar();
        
        currentStamina = maxStamina;
        staminaIcon.SetStamina(maxStamina);

        actionEnd();
    }

    void KO_effect()
    {
        if(Enemy_is_hurt.enemy_isDefeated){
            Instantiate(screenFlash, BattleCanvas_Parent);
            Instantiate(defeatedEffect_pop);
            textSpawn.spawn_KO_text();
            DOTween.Play("CameraShake");
        }
    }

    void playVictoryIdle()
    {
        tomatoAnimator.Play("tomato_victory_idle",-1,0f);
    }

    void DisableHitBox()
    {
        tomatohurt.SetHitBox(false);
    }

    void EnableHitBox()
    {
        tomatohurt.SetHitBox(true);
    }

    void spawnFaintStars()
    {
        Invoke("createStars", 0.8f);
    }

    private void createStars()
    {
        tempObj = Instantiate(faintStars, transform);
    }

    public void playTomatoKnockback()
    {
        Invoke("playKnockBack",0.1f);
    }
    void playKnockBack()
    {
        if (IsMatoAttacked()) return;

        if(isTired)
            tomatoAnimator.Play("tomato_tiredKnockback",-1,0f);
        else
            tomatoAnimator.Play("tomato_knockback",-1,0f);
    }

    void ScreenShake(string dotweenID)
    {
        DOTween.Rewind($"{dotweenID}");
        DOTween.Play($"{dotweenID}");
    }

    void SparkleEffect()
    {
        flashEffect.Flash(0.02f);
        GameManager.gm_instance.battle_system.battleTimeManager.SetSlowSetting(0.01f, 0.05f);
        GameManager.gm_instance.battle_system.battleTimeManager.DoSlowmotion();
        Instantiate (sparkleEffect);
    }

    void DunkEffect()
    {
        Instantiate (dunkEffect);
        Instantiate (dunkEffect2);
        // battleSystem.ShockWaveControl.CallShockWave(battleSystem.DunkShockWave.Duration, battleSystem.DunkShockWave.Size, battleSystem.DunkShockWave.Size);
    }

    public void BlastEffect()
    {
        Instantiate (blastEffect);
        Instantiate (upper_hitef, new Vector2 (transform.position.x + 0.7f, transform.position.y - 0.5f), Quaternion.identity);
        battleSystem.ShockWaveControl.CallShockWave(battleSystem.BlastShockWave.Duration, battleSystem.BlastShockWave.Size, battleSystem.BlastShockWave.Size);
    }

    public IEnumerator SetDeflectLaser(GameObject laser, bool state, float wait = 0f)
    {
        yield return WaitForCache.GetWaitForSecond(wait);
        laser.SetActive(state);
    }

    public void DestroyAllMatoPunches()
    {
        foreach (Transform punch in Parent)
            Destroy(punch.gameObject);
    }

    public bool IsMatoAttacked()
    {
        return (isFainted || tomatoHurt.isTomatoHurt);
    }

    public bool CheckAnimationState(string animation_string)
    {
        return  tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation_string);
    }

    public bool PressKey(string moveName)
    {
        return tomatoInput.actions[moveName].WasPressedThisFrame();
    }


// GAKSUNG CONTROLLER ===============================================================================================================
    void gaksung_def()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_objAnim.Play("Gaksung_ctrl_def",-1,0f);
        }
    }
    void gaksung_GT() 
    {
        if(parryBar.gaksungOn)
        {
            gaksung_objAnim.Play("Gaksung_ctrl_GT",-1,0f);
        }   
    }
    void gaksung_LE()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_objAnim.Play("Gaksung_ctrl_LE",-1,0f);
        }
    }
    void gaksung_RE()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_objAnim.Play("Gaksung_ctrl_RE",-1,0f);
        }
    }
    void gaksung_LP()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_objAnim.Play("Gaksung_ctrl_LP",-1,0f);
        }
    }
    void gaksung_RP()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_objAnim.Play("Gaksung_ctrl_RP",-1,0f);
        }
    }
    void gaksung_entry()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_anim.Play("Gaksung_entry",-1,0f);
        }
    }
    void gaksung_J()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_anim.Play("Gaksung_jump",-1,0f);
        }
    }
    public void gaksung_OFF()
    {
        if(parryBar.gaksungOn && gaksung_OBJ.activeSelf)
        {
            gaksung_OBJ.SetActive(false);
        }
    }
    public void gaksung_ON()
    {
        if(parryBar.gaksungOn && !gaksung_OBJ.activeSelf)
        {
            gaksung_OBJ.SetActive(true);
        }
    }

    public void ResetGaksung()
    {
        parryBar.gaksungOn = false;
        parryBar.parryFill.fillAmount = 0;
        parryBar.parry_fullCharge.SetActive(false);
        gaksung_OBJ.SetActive(false);
    }

// SKILL ATTACK =====================================================================================================================
    void skill(int effectIndex)
    {
        skillEffectIndex = effectIndex;
        Instantiate (tomato_S, Parent);
    }
    public void SkillEffect() {
        Instantiate (tomatoEquip[equipIndex].HitEffects[skillEffectIndex], Parent);
    }

// SUPER ATTACK =====================================================================================================================
    void superAttack()
    {
        enemy_supered = true;
    }
}

public enum SkillType { Equip_Skill, Assist_Skill, Deflect_Skill }