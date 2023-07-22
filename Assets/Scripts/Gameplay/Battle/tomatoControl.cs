using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

/* DEFAULT DEBUG CODE
    if(Input.GetKeyDown(KeyCode.T))
    {
        Debug.Log("isAction : " + isAction);
        Debug.Log("isPunch : " + isPunch);
        Debug.Log("isGatle : " + isGatle);
        Debug.Log("isGuard : " + isGuard);

        Debug.Log("isParry : " + tomatoGuard.isParry);
        Debug.Log("isHurt : " + tomatoHurt.isTomatoHurt);
        Debug.Log("isFainted : " + isFainted);

        Debug.Log("enemy_isDefeated : " + Enemy_is_hurt.enemy_isDefeated);
        Debug.Log("enemy_isParried : " + Enemy_parried.isParried);
        Debug.Log("enemy_isCountered : " + Enemy_countered.enemy_isCountered);
        Debug.Log("enemy_isPunched : " + Enemy_is_hurt.enemy_isPunched);
        Debug.Log("enemy_isHit : " + Enemy_is_hurt.enemyIsHit);
        Debug.Log("enemy_isDefeated : " + Enemy_is_hurt.enemy_isDefeated);
    }
*/
public class tomatoControl : MonoBehaviour
{
    [SerializeField] private PlayerInput tomatoInput;
    private Animator tomatoAnimator; public Animator tomatoAnim => tomatoAnimator;
    private GameObject _parryInstance;
    [SerializeField] Animator gatleButton_anim_L, gatleButton_anim_R;
    [SerializeField] private Animator gaksung_objAnim, gaksung_anim; [SerializeField] private GameObject gaksung_OBJ;
    [SerializeField] private BoxCollider2D hitbox;
    [SerializeField] private GameObject tomato_LP, tomato_RP, tomato_G, tomato_PRY, tomato_S;
    [SerializeField] private GameObject gatleSmoke_L, gatleSmoke_R, upperBg, upper_hitef, upper_hitef2, upperSmoke, superBanner, screenFlash, defeatedEffect_pop, faintStars, blastEffect, dunkEffect, dunkEffect2, sparkleEffect;
    [System.NonSerialized] public GameObject tempObj = null;
    [SerializeField] private Transform Parent, BattleCanvas_Parent;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GuardBar guardBar; public GuardBar guard_bar => guardBar;
    [SerializeField] private ParryBar parryBar;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private CounterTrack counterTrack;
    [SerializeField] private tomatoDamage tomatodamage;
    [SerializeField] private TextSpawn textSpawn;
    [SerializeField] private FlashEffect flashEffect;
    
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
    private bool isAction = false;
    
    private bool isPunch = false; // smoothen punch input (enabling cancel)
    [System.NonSerialized] public static bool isGatle = false; 

    [System.NonSerialized] public static bool isGuard = false;

    [System.NonSerialized] public bool isMiss = false;
    
    private bool guardRelease;               // prevent multiple animations trying to play at a single frame (esp during animation transition)

    [System.NonSerialized] public static bool isIntro = true;
    [System.NonSerialized] public static bool isVictory = false;
    [System.NonSerialized] public static bool isFainted = false;

    [System.NonSerialized] public static bool gatleButton_once = false;  // play a line once
    [System.NonSerialized] public static bool uppercutYes = false;       // player succeeded uppercut
    [System.NonSerialized] public static bool enemyUppered = false;
    [System.NonSerialized] public static bool enemyFreeze = false;
    [System.NonSerialized] public bool enemy_supered = false;
    private bool isTired = false;

    [System.NonSerialized] public SkillType currentSkillType;
    [System.NonSerialized] public int tomatoes = 0;

    void OnEnable()
    {
        isAction = isPunch = isGatle = isGuard = isMiss = false;
        guardRelease = true;

        isIntro = true;
        isVictory = isFainted = isTired = tomatoGuard.isParry = gatleButton_once = uppercutYes = enemyUppered = enemyFreeze = enemy_supered = false;

        dmg_normalPunch = tomatodamage.NormalPunch(tomatoAtk);
        dmg_gatlePunch = tomatodamage.GatlePunch(tomatoAtk);
        dmg_upperPunch = tomatodamage.UpperPunch(tomatoAtk);
        dmg_super = tomatodamage.SkillAttack(tomatoAtk, tomatoSuperEquip.skillDamage);
        dmg_dunk = tomatodamage.DunkAttack(tomatoAtk);

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        healthBar.setDamageFill();

        guardBar.SetMaxGuard(maxGuard);
        current_guardPt = maxGuard;
        guardBar.SetGuardbar(current_guardPt);

        parryBar.SetParryBar();

        staminaIcon.SetMaxStamina(maxStamina);
        currentStamina = maxStamina;
        staminaIcon.SetStamina(currentStamina);

        tomatoes = 0;
        counterTrack.CounterTracker();

        tomatoInput.SwitchCurrentActionMap("Battle");
    }
    void OnDisable()
    {
        tomatoInput.SwitchCurrentActionMap("Player");
    }
    void Start()
    {
        tomatoAnimator = GetComponent<Animator>();
    }

    void ChangeAnimationState(string newState)
    {
        tomatoAnimator.Play(newState);
    }
    void Update()
    {
        if(!tomatoHurt.isTomatoHurt && !isIntro && !isFainted && !isVictory)
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
                    if (battleSystem.enemy_control.canDunk) {
                        battleSystem.enemy_control.canDunk = false;
                        tomatoAnimator.Play("tomato_dunk", -1, 0f);
                    }

                    else if (!isTired)
                        ChangeAnimationState(TOMATO_LP);
                    
                    else
                        tomatoAnimator.Play("tomato_tiredPunch_L",-1,0f);
                }
                else if (PressKey("RightPunch"))
                {
                    if (battleSystem.enemy_control.canDunk) {
                        battleSystem.enemy_control.canDunk = false;
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
        
            else if(isGuard)
            {
                if (!Enemy_parried.isParried  && !tomatoHurt.isTomatoHurt && tomatoInput.actions["Guard"].WasReleasedThisFrame())
                {
                    ReleaseGuard();
                }
            }
            
            else if (isPunch && !isMiss)
            {
                if (PressKey("LeftPunch"))
                {
                    ChangeAnimationState(TOMATO_LP);
                }
                if (PressKey("RightPunch"))
                {
                    ChangeAnimationState(TOMATO_RP);
                }
            }
            
            if((!BattleUI_Control.stopGatle) && tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GATLINGIDLE))
            {
                gatlePunch();
            }
            if(isGatle)
            {
                if((!BattleUI_Control.stopGatle) && (tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GLP) || tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_GRP)))
                {
                    gatlePunch();
                }
            }
            
        }
    }

    // FUNCTIONS ====================================================================================================================

    void IntroOver()
    {
        ChangeAnimationState(TOMATO_IDLE);
    }
    void IdleState()
    {
        isAction = false;
        tomatoHurt.isTomatoHurt = false;
    }

    void actionStart()
    {
        isPunch = false;
        isAction = true;
    }
    void evadeStart()
    {
        isPunch = false;
        isAction = true;
    }
    void actionEnd()
    {
        isAction = false;
        isPunch = false;

        if (currentStamina == 0 && !tomatoHurt.isTomatoHurt){
            tomatoAnimator.Play("tomato_tired",-1,0f);
            isTired = true;
        }
        else if (!tomatoHurt.isTomatoHurt && !isFainted){
            ChangeAnimationState(TOMATO_IDLE);
            isTired = false;
        }
    }
    void smoothPunch()          //allows to rapidly switch between LP and RP
    {
        isPunch = true;
    }
    void smoothGatle()
    {
        isGatle = true;
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

    void tomatoHurtStart()      //prevents from initiating action while hurt , reset all booleans except isAction
    {
        isPunch = false;
        isAction = true;

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
    void tomatoHurtOver()
    {
        isPunch = false;
        isAction = false;
        tomatoHurt.isTomatoHurt = false;
        tomatoGuard.preventDamageOverlap = false;

        if (currentStamina == 0 && !tomatoHurt.isTomatoHurt){
            tomatoAnimator.Play("tomato_tired",-1,0f);
            isTired = true;
        }
        else{
            ChangeAnimationState(TOMATO_IDLE);
            isTired = false;
        }
    }

    void guardStart()
    {
        if(!guardRelease)
        {
            isPunch = false;
            isAction = true;
            isGuard = true;
            guardBar.regainGuardTimer = GuardBar.G_REGAINTIMER_MAX;
        }
    }
    void return_to_Guard()
    {
        if(Enemy_parried.isParried && EnemyControl.isPhysical)
        {
            ChangeAnimationState(TOMATO_GATLING);
        }
        else
        {
            if(tomatoGuard.isParry)
            {
                tomatoGuard.isParry = false;
                tomato_G.SetActive(true);
            }
            if(tomatoInput.actions["Guard"].IsPressed())
            {
                ChangeAnimationState(TOMATO_GUARD);
            }
        }
    }

    public void ReleaseGuard()
    {
        Destroy(_parryInstance);
        hitbox.enabled = true;

        guardRelease = true;
        
        tomatoAnimator.Play("tomato_idle",-1,0f);
        isGuard = false;
        isAction = false;

        tomatoGuard.isParry = false;
        tomatoGuard.preventDamageOverlap = false;
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
            hitbox.enabled = false;
            _parryInstance = Instantiate (tomato_PRY, Parent);
            Invoke("parryDeactivate",0.05f);
        }
    }

    void parryDeactivate()
    {
        Destroy(_parryInstance);
        if(!Enemy_parried.isParried) {
            hitbox.enabled = true;
        }
    }

    void gatlingReady()
    {
        if(!BattleUI_Control.stopGatle && !gatleCircleControl.failUppercut && !uppercutYes)
        {
            isGatle = false;
            tomatoAnimator.Play("tomato_gatlingIdle",-1,0f);
        }
    }

    void endGatle()
    {
        BattleUI_Control.stopGatle = false;
        hitbox.enabled = true;
    }

    void gatlingPunch()
    {
        if(!BattleUI_Control.stopGatle)
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
        isGuard = false;
        tomatoGuard.isParry = false;
        /**/ Enemy_parried.isParried = false; /**/

        BattleUI_Control.gatleCircle_once = false;
        BattleUI_Control.stopGatle = false;
        gatleButton_once = false;

        hitbox.enabled = true;
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
        //Instantiate (dunk_hitef);
        battleSystem.enemy_control.isDunked = true;
    }

    void revive_to_idle()
    {
        ChangeAnimationState(TOMATO_IDLE);
        textSpawn.spawn_FIGHT_text();

        string enemyReEngage = battleSystem.GetEnemyBase().ReEngage;
        battleSystem.GetEnemyAnim().Play(enemyReEngage);

        battleSystem.resetPlayerHealth = true;
        battleSystem.resetEnemyHealth = true;

        battleSystem.enemy_control.enemy_Countered.ResetCounterPoints();
        battleSystem.featherPointManager.ResetFeather();
        battleSystem.tomato_control.guard_bar.RestoreGuardBar();
        ResetGaksung();
        
        currentStamina = maxStamina;
        staminaIcon.SetStamina(maxStamina);
        isTired = false;
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
        battleSystem.shockWaveEffect.CallShockWave(0.3f, 0.1f);
    }

    public void BlastEffect()
    {
        Instantiate (blastEffect);
        Instantiate (upper_hitef, new Vector2 (transform.position.x + 0.7f, transform.position.y - 0.5f), Quaternion.identity);
        battleSystem.shockWaveEffect.CallShockWave(0.4f, 0.3f);
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
    void gaksung_OFF()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_OBJ.SetActive(false);
        }
    }
    void gaksung_ON()
    {
        if(parryBar.gaksungOn)
        {
            gaksung_OBJ.SetActive(true);
        }
    }

    private void ResetGaksung()
    {
        parryBar.gaksungOn = false;
        parryBar.parryFill.fillAmount = 0;
        parryBar.parry_fullCharge.SetActive(false);
        gaksung_OBJ.SetActive(false);
    }

// SKILL ATTACK =====================================================================================================================
    void skill()
    {
        Instantiate (tomato_S, Parent);
    }
    void effect0()
    {
        Instantiate (tomatoEquip[0].HitEffects[0], Parent);
    }
    void effect1()
    {
        Instantiate (tomatoEquip[0].HitEffects[1], Parent);
    }

// SUPER ATTACK =====================================================================================================================
    void superAttack()
    {
        enemy_supered = true;
    }

}

public enum SkillType { Equip_Skill, Assist_Skill }