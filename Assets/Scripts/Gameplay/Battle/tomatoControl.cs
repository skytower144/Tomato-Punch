using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* DEFAULT DEBUG CODE
if(Input.GetKeyDown(KeyCode.P))
    {
        Debug.Log("isAction : " + isAction);
        Debug.Log("isPunch : " + isPunch);
        Debug.Log("isGatle : " + isGatle);
        Debug.Log("isGuard : " + isGuard);
        Debug.Log("downGamepad : " + downGamepad);
        Debug.Log("isParry : " + tomatoGuard.isParry);
        Debug.Log("tomatoIsHurt : " + tomato_hurt.isTomatoHurt);
    }
*/
public class tomatoControl : MonoBehaviour
{
    private Animator tomatoAnimator;
    private GameObject _parryInstance;
    [SerializeField] tomatoHurt tomato_hurt;
    [SerializeField] Animator gatleButton_anim_L, gatleButton_anim_R;
    [SerializeField] private Animator gaksung_objAnim, gaksung_anim; [SerializeField] private GameObject gaksung_OBJ;
    [SerializeField] private BoxCollider2D hitbox;
    [SerializeField] private GameObject tomato_LP, tomato_RP, tomato_G, tomato_PRY, tomato_S;
    [SerializeField] private GameObject gatleSmoke_L, gatleSmoke_R, upperBg, upper_hitef, upper_hitef2, upperSmoke, superBanner;
    [SerializeField] private Transform Parent;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GuardBar guardBar;
    [SerializeField] private ParryBar parryBar;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] CounterTrack counterTrack;
    public SuperEquip tomatoSuperEquip;
    public List <Equip> tomatoEquip;
    
    //Tomato Info: ============================================================================================================
    //[System.NonSerialized] 
    public float maxHealth, currentHealth;
    public float maxGuard, current_guardPt;
    public float tomatoAtk;
    public int maxStamina, currentStamina;
    [System.NonSerialized] public float dmg_normalPunch, dmg_gatlePunch, dmg_upperPunch, dmg_super;

    //Animation States: ======================================================================================================
    const string TOMATO_IDLE = "tomato_idle";
    const string TOMATO_LEVADE = "tomato_Levade"; const string TOMATO_REVADE = "tomato_Revade"; const string TOMATO_JUMP = "tomato_jump";
    const string TOMATO_LP = "tomato_LP"; const string TOMATO_RP = "tomato_RP";
    const string TOMATO_GUARD = "tomato_guard"; const string TOMATO_GUARDAFT = "tomato_guardAft";
    const string TOMATO_GATLING = "tomato_gatling"; const string TOMATO_GATLINGIDLE = "tomato_gatlingIdle"; const string TOMATO_GLP = "tomato_GLP"; const string TOMATO_GRP = "tomato_GRP";
    //========================================================================================================================
    private bool isAction = false;
    
    private bool isPunch = false; // smoothen punch input (enabling cancel)
    [HideInInspector] public static bool isGatle = false; 

    [HideInInspector] public static bool isGuard = false;
    private bool guardRelease;               // prevent multiple animations trying to play at a single frame (esp during animation transition)
    private bool downGamepad = false;        // diffrentiate keyboard input and gamepad input
    private int x_GP = 0, y_GP = 0;          // making left joystick act like a button trigger

    [HideInInspector] public static bool gatleButton_once = false;  // play a line once
    [HideInInspector] public static bool uppercutYes = false;       // player succeeded uppercut
    [HideInInspector] public static bool enemyUppered = false;
    [HideInInspector] public static bool enemyFreeze = false;
    [HideInInspector] public bool enemy_supered = false;
    private bool isTired = false;

    [System.NonSerialized] public int tomatoes = 0;
    public int tomatoSuper;                  // which super indication

    void Awake()
    {
        dmg_normalPunch = tomatoAtk;
        dmg_gatlePunch = tomatoAtk * 0.2f + 0.1f;
        dmg_upperPunch = tomatoAtk + 4;
        dmg_super += tomatoAtk;
    }
    void Start()
    {
        tomatoAnimator = GetComponent<Animator>();

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        healthBar.setDamageFill();

        guardBar.SetMaxGuard(maxGuard);
        guardBar.SetGuardbar(current_guardPt);

        parryBar.SetParryBar();

        staminaIcon.SetMaxStamina(maxStamina);
        staminaIcon.SetStamina(currentStamina);

    }

    void ChangeAnimationState(string newState)
    {
        tomatoAnimator.Play(newState);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("isAction : " + isAction);
            Debug.Log("isPunch : " + isPunch);
            Debug.Log("isGatle : " + isGatle);
            Debug.Log("isGuard : " + isGuard);
            Debug.Log("downGamepad : " + downGamepad);
            Debug.Log("isParry : " + tomatoGuard.isParry);
            Debug.Log("isHurt : " + tomato_hurt.isTomatoHurt);
        }

        if((Input.GetAxisRaw("LeftJoystickHorizontal") == 0))
            x_GP = 0;
        if((Input.GetAxisRaw("LeftJoystickVertical") == 0))
            y_GP = 0;
        if(!tomato_hurt.isTomatoHurt)
        {
            if(!isAction)
            {
                if(Input.GetKeyDown(KeyCode.A) ||  ((Input.GetAxisRaw("LeftJoystickHorizontal") < 0) && x_GP == 0))
                {
                    x_GP = -1;
                    ChangeAnimationState(TOMATO_LEVADE);
                }
                else if(Input.GetKeyDown(KeyCode.D) || ((Input.GetAxisRaw("LeftJoystickHorizontal") > 0) && x_GP == 0))
                {
                    x_GP = 1;
                    ChangeAnimationState(TOMATO_REVADE);
                }
                else if(Input.GetKeyDown(KeyCode.W) || ((Input.GetAxisRaw("LeftJoystickVertical") < 0) && y_GP == 0))
                {
                    y_GP = 1;
                    ChangeAnimationState(TOMATO_JUMP);
                }
                else if(!isTired)
                {
                    if(Input.GetKeyDown(KeyCode.S) || (Input.GetAxisRaw("LeftJoystickVertical") > 0))
                    {
                        guardRelease = false;
                        if((Input.GetAxisRaw("LeftJoystickVertical") > 0))
                        {
                            downGamepad = true;
                        }
                        ChangeAnimationState(TOMATO_GUARD);
                        tomato_G.SetActive(true);
                    }
                    else if(Input.GetKeyDown(KeyCode.O) || (Input.GetKeyDown("joystick button 0")))
                    {
                        ChangeAnimationState(TOMATO_LP);
                    }
                    else if(Input.GetKeyDown(KeyCode.P) || (Input.GetKeyDown("joystick button 1")))
                    {
                        ChangeAnimationState(TOMATO_RP);
                    }
                    else if(Input.GetKeyDown(KeyCode.R))
                    {
                        if((tomatoSuperEquip != null) && parryBar.gaksungOn)
                        {
                            parryBar.gaksungOn = false;
                            parryBar.parryFill.fillAmount = 0;
                            parryBar.parryFillUp.SetActive(false);
                            gaksung_OBJ.SetActive(false);

                            superBanner.SetActive(true);
                            enemyFreeze = true;
                            tomatoAnimator.enabled = false;
                        }
                    }
                    else if(Input.GetKeyDown(KeyCode.Q))
                    {
                        if((tomatoEquip[0] != null) && tomatoes > 0){
                            tomatoAnimator.Play(tomatoEquip[0].SkillAnimation,-1,0f);
                            tomatoes -= 1;
                            counterTrack.CounterTracker();
                        }
                    }
                }
            }
        
            else if(isGuard)
            {
                if( !Enemy_parried.isParried  && !tomato_hurt.isTomatoHurt && (Input.GetKeyUp(KeyCode.S)) )
                {
                    Destroy(_parryInstance);
                    hitbox.enabled = true;

                    guardRelease = true;
                    
                    tomatoAnimator.Play("tomato_idle",-1,0f);
                    isGuard = false;
                    isAction = false;
                    downGamepad = false;
                    tomatoGuard.isParry = false;
                }
                else if(!Enemy_parried.isParried && !tomato_hurt.isTomatoHurt && (downGamepad == true) && (Input.GetAxisRaw("LeftJoystickVertical") == 0))
                {
                    Destroy(_parryInstance);
                    hitbox.enabled = true;

                    guardRelease = true;

                    tomatoAnimator.Play("tomato_idle",-1,0f);
                    isGuard = false;
                    isAction = false;
                    downGamepad = false;
                    tomatoGuard.isParry = false;
                }
            }
            
            else if(isPunch)
            {
                if(Input.GetKeyDown(KeyCode.O) || (Input.GetKeyDown("joystick button 0")))
                {
                    ChangeAnimationState(TOMATO_LP);
                }
                if(Input.GetKeyDown(KeyCode.P) || (Input.GetKeyDown("joystick button 1")))
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

    void IdleState()
    {
        isAction = false;
        tomato_hurt.isTomatoHurt = false;
    }
    void actionStart()
    {
        isPunch = false;
        isAction = false;

        if (currentStamina == 0){
            tomatoAnimator.Play("tomato_tired",-1,0f);
            isTired = true;
        }
        else{
            isAction = true; // cascade boolean value
                             // reason: isPunch interruption before action properly ending
        }
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

        if (currentStamina == 0){
            tomatoAnimator.Play("tomato_tired",-1,0f);
            isTired = true;
        }
        else{
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
        if(Input.GetKeyDown(KeyCode.O) || (Input.GetKeyDown("joystick button 0")))
        {
            if(gatleCircleControl.failUppercut == false)
            {
                tomatoAnimator.Play("tomato_GLP",-1,0f);
            }
        }
        if(Input.GetKeyDown(KeyCode.P) || (Input.GetKeyDown("joystick button 1")))
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
        downGamepad = false;

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
        tomato_hurt.isTomatoHurt = false;
        tomatoGuard.preventDamageOverlap = false;

        if (currentStamina == 0){
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
            if(Input.GetKey(KeyCode.S))
            {
                ChangeAnimationState(TOMATO_GUARD);
            }
        }
        
    }

    void punchActivate()
    {
        if(tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_LP))
        {
            Instantiate (tomato_LP, Parent);
        }
        else if(tomatoAnimator.GetCurrentAnimatorStateInfo(0).IsName(TOMATO_RP))
        {
            Instantiate (tomato_RP, Parent);
        }
    }

    void parryActivate()
    {
        if(!tomato_hurt.isTomatoHurt)
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
        if(!gatleCircleControl.failUppercut && !uppercutYes)
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