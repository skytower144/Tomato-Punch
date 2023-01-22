using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 
using System;

public enum GameState { FreeRoam, Battle }
public class GameManager : MonoBehaviour
{
    GameState gameState;

    [SerializeField] PlayerMovement playerMovement; public PlayerMovement player_movement => playerMovement;
    [SerializeField] BattleSystem battleSystem; public BattleSystem battle_system => battleSystem;
    [SerializeField] ResolutionMenu resolutionMenu;
    [SerializeField] RebindKey rebindKey; public RebindKey rebind_key => rebindKey;
    [SerializeField] ControlScroll controlScroll; public ControlScroll control_scroll => controlScroll;
    [SerializeField] UIControl uiControl;
    [SerializeField] SaveLoadMenu saveLoadMenu; public SaveLoadMenu save_load_menu => saveLoadMenu;
    [SerializeField] Camera mainCamera;
    [SerializeField] private GameObject battleCircle, exclamation, fadeIn;

    [System.NonSerialized] public GameObject[] levelHolder;
    [SerializeField] private Animator playerAnimator;

    public int gamepadType;
    public float stickSensitivity;

    [System.NonSerialized] public float holdStartTime = float.MaxValue;
    [SerializeField] private float holdTimer;
    [SerializeField] private float intervalTime;
    private float delayTimer;
    public bool WasHolding => holdStartTime < Time.unscaledTime;

    private float player_x, player_y;

// ================================================================
    public static GameManager gm_instance { get; private set; }
    void Awake()
    {
        if (gm_instance != null)
        {
            return;
        }

        gm_instance = this;
    }
// ================================================================
    private void Start() //subscribing to an event
    {
        resolutionMenu.SetupGraphic();
        resolutionMenu.LoadResolutionSetting();

        battleSystem.OnBattleOver += EndBattle;
        //playerMovement.BeginBattle += StartBattle;
    }

    void StartBattle()
    {
        StartCoroutine(Wait());
    }

    void EndBattle()
    {
        StartCoroutine(BattleExit_Wait());
    }

    private void Update()
    {
        if (gameState == GameState.FreeRoam)
        {
            playerMovement.HandleUpdate();
        }
        DetectGamepad();
        // else if(gameState == GameState.Battle)
        // {
        //     battleSystem.HandleUpdate();
        // }
    }

    // Initiate_Battle() => StartBattle() => Wait()
    public void Initiate_Battle(EnemyBase enemy_base)
    {
        PlayerMovement.isBattle = true;

        playerAnimator.SetBool("isWalking",false);

        battleSystem.enemy_control._base = enemy_base;
        battle_system.SetBg(enemy_base.bgName);

        player_x = playerMovement.transform.position.x;
        player_y = playerMovement.transform.position.y;

        Instantiate (exclamation, new Vector2 (player_x, player_y + 3.8f), Quaternion.identity);
        Invoke("battleStart_ef", 0.4f);
        StartBattle();
    }
    public void battleStart_ef()
    {
        Destroy(Instantiate (battleCircle, new Vector2 (player_x-2.6f, player_y), Quaternion.identity),2f);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);

        mainCamera.gameObject.SetActive(false);
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);

        DisableLevelHolder();
    }

    IEnumerator BattleExit_Wait()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (GameObject level_holder in levelHolder)
        {
            level_holder.SetActive(true);
        }

        battle_system.battleUI_Control.NormalizeBattleUI();
        battle_system.enemy_control.ClearAnimation();
        
        battleSystem.gameObject.SetActive(false);
        gameState = GameState.FreeRoam;

        GameObject fadeInstance = Instantiate(fadeIn, new Vector2 (player_x, player_y - 0.05f), Quaternion.identity);
        fadeInstance.transform.localScale = new Vector2(2f, 2f);
        
        mainCamera.gameObject.SetActive(true);

        PlayerMovement.isBattle = false;
    }

    private void DisableLevelHolder()
    {
        levelHolder = GameObject.FindGameObjectsWithTag("LevelHolder");
        foreach (GameObject level_holder in levelHolder)
        {
            level_holder.SetActive(false);
        }
    }

    public void UpdateGamepadType()
    {
        int prevType = gamepadType;
        var cur_gamepad = Gamepad.current;
        string gamepadName = "";

        if (cur_gamepad != null)
            gamepadName = cur_gamepad.name.ToLower();

        if (gamepadName.Contains("xinput") || gamepadName.Contains("xbox") )
            gamepadType = 1;
        else if (gamepadName.Contains("switch"))
            gamepadType = 3;
        else
            gamepadType = 2;

        if (prevType != gamepadType) {
            uiControl.UI_GamepadSwitch();
        }
    }

    private bool IsGamepad()
    {
        string [] names = Input.GetJoystickNames();

        foreach (string name in names)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return true;
            }
        }
        return false;
    }

    private void DetectGamepad()
    {
        if (!rebindKey.isBinding)
        {
            string [] names = Input.GetJoystickNames();

            if (names.Length == 0){ // preventing index error
                uiControl.UI_Update(true); // true => Activate Keyboard
                gamepadType = 0;
                return;
            }
            
            if (IsGamepad()) {
                UpdateGamepadType();
                uiControl.UI_Update(false);
            }
            else {
                uiControl.UI_Update(true);
                gamepadType = 0;
            }
        }
    }

    public void DetectHolding(Action callback)
    {
        if (WasHolding)
        {
            if (Time.unscaledTime - holdStartTime > holdTimer)
            {
                delayTimer -= Time.unscaledDeltaTime;
    
                if (delayTimer <= 0)
                {
                    delayTimer = intervalTime;

                    callback?.Invoke();
                }
            }
            
            // Allowing Button fast navigation
            else if(playerMovement.Press_Key("Move"))
            {
                string currentScheme = playerMovement.PlayerInput.actions["Move"].activeControl.ToString();

                if  (
                    (!currentScheme.Contains("leftStick") && !currentScheme.Contains("rightStick")) &&
                    (Mathf.Abs(playerMovement.ReturnMoveVector().x) == 1 || Mathf.Abs(playerMovement.ReturnMoveVector().y) == 1)
                    )
                {
                    callback?.Invoke();
                }
            }
            
        }
        else
        {
            delayTimer = intervalTime;
            holdStartTime = Time.unscaledTime;

            callback?.Invoke();
        }
    }
}