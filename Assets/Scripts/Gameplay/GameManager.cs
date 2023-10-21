using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState { FreeRoam, Battle }
public class GameManager : MonoBehaviour
{
    GameState gameState;
    PlayerReviveState expectedReviveState;
    public static GameManager gm_instance { get; private set; }

    public AssistManager assistManager;
    public PlayerKeyEventManager playerKeyEventManager;
    public PartyManager partyManager;
    public ItemManager itemManager;
    public PlayerMovement player_movement => playerMovement;
    public BattleSystem battle_system => battleSystem;
    public RebindKey rebind_key => rebindKey;
    public ControlScroll control_scroll => controlScroll;
    public UIControl ui_control => uiControl;
    public SaveLoadMenu save_load_menu => saveLoadMenu;
    public ItemMenuNavigation consumable_navigation => consumableNavigation;
    public ItemMenuNavigation other_item_navigation => otherItemNavigation;
    public equipControl equip_control => equipcontrol;

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] ResolutionMenu resolutionMenu;
    [SerializeField] RebindKey rebindKey;
    [SerializeField] ControlScroll controlScroll;
    [SerializeField] UIControl uiControl;
    [SerializeField] SaveLoadMenu saveLoadMenu;
    [SerializeField] ItemMenuNavigation consumableNavigation, otherItemNavigation;
    [SerializeField] equipControl equipcontrol;

    [SerializeField] Camera mainCamera;
    [SerializeField] private GameObject battleCircle, exclamation, fadeIn;

    [System.NonSerialized] public GameObject[] levelHolder;
    [SerializeField] private Animator playerAnimator;
    private float player_x, player_y;

    public int gamepadType;
    public float stickSensitivity;
    [System.NonSerialized] public float holdStartTime = float.MaxValue;
    [SerializeField] private float holdTimer;
    [SerializeField] private float intervalTime;
    private string[] joystickNames;
    private float delayTimer;

    public bool WasHolding => holdStartTime < Time.unscaledTime;
    public bool EnableDebug;

    void Awake()
    {
        if (gm_instance != null)
        {
            return;
        }

        gm_instance = this;
        EnableDebug = AppSettings.IsUnityEditor;

        resolutionMenu.SetupGraphic();
        resolutionMenu.LoadResolutionSetting();
        
        itemManager.HideAllItems();

        battleSystem.OnBattleOver -= EndBattle;
        battleSystem.OnBattleOver += EndBattle;

        InputSystem.onDeviceChange -= DetectGamepad;
        InputSystem.onDeviceChange += DetectGamepad;
        
        //playerMovement.BeginBattle += StartBattle;
    }

    void OnEnable()
    {
        DetermineKeyOrPad();
    }

    void OnDisable()
    {
        battleSystem.OnBattleOver -= EndBattle;
        InputSystem.onDeviceChange -= DetectGamepad;
    }

    void StartBattle()
    {
        StartCoroutine(Wait());
    }

    void EndBattle(bool isVictory)
    {
        StartCoroutine(BattleExit_Wait(isVictory));
    }

    private void Update()
    {
        if (gameState == GameState.FreeRoam)
        {
            playerMovement.HandleUpdate();
        }
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

        battleSystem.enemy_control.LoadEnemyBaseData(enemy_base);
        battle_system.SetBg(enemy_base);

        player_x = playerMovement.transform.position.x;
        player_y = playerMovement.transform.position.y;
        
        expectedReviveState = enemy_base.ReviveState;

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
        yield return WaitForCache.WaitSeconds1_5;

        mainCamera.gameObject.SetActive(false);
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);

        DisableLevelHolder();
    }
    IEnumerator BattleExit_Wait(bool isVictory)
    {
        yield return WaitForCache.WaitSeconds1_5;

        foreach (GameObject level_holder in levelHolder)
            level_holder.SetActive(true);

        battle_system.battleUI_Control.NormalizeBattleUI();
        battle_system.enemy_control.ClearAnimation();
        battleSystem.gameObject.SetActive(false);

        if (!isVictory) {
            // Load quicksave
            saveLoadMenu.isAutoSaving = true;
            saveLoadMenu.SetSlotNumber(3);
            StartCoroutine(saveLoadMenu.PrepareLoad());

            while (saveLoadMenu.isAutoSaving) {
                yield return null;
            }
            switch (expectedReviveState) {
                case PlayerReviveState.Cafe:
                    // Revive at Cafe
                    break;
                
                case PlayerReviveState.Bench:
                    ReviveFromBench();
                    break;
                
                default:
                    break;
            }
        }
        else if ((isVictory && DialogueManager.instance.is_continue_talk) || (!isVictory && (expectedReviveState == PlayerReviveState.LoseTalk)))
            Invoke("TalkToFacingNpc", 0.5f);
        
        Invoke("ReturnToFreeroam", 1f);
        playerKeyEventManager.ApplyCacheKeyEvents(isVictory);
        ShowScreen();
    }

    private void ReviveFromBench()
    {
        playerMovement.transform.position = GetBenchPostion();
        CutsceneHandler.FaceAdjustment(playerMovement.myAnim, "DOWN");
        playerAnimator.Play("Wakeup", -1, 0f);
        Instantiate(playerMovement.newspaper, playerAnimator.transform);
    }

    private void ShowScreen()
    {
        GameObject fadeInstance = Instantiate(fadeIn, new Vector2 (playerMovement.transform.position.x, playerMovement.transform.position.y - 0.05f), Quaternion.identity);
        fadeInstance.transform.localScale = new Vector2(2f, 2f);
        mainCamera.gameObject.SetActive(true);
    }

    private void TalkToFacingNpc()
    {
        if (DialogueManager.instance.ContinueTalkTarget != null)
            DialogueManager.instance.ContinueTalkTarget.InitiateTalk();
        else
            PlayerMovement.instance.PlayerInteract();
        
        DialogueManager.instance.SetIsContinueTalkBool(false);
        DialogueManager.instance.ContinueTalkTarget = null;
    }

    private void ReturnToFreeroam()
    {
        gameState = GameState.FreeRoam;
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

        if (cur_gamepad != null) gamepadName = cur_gamepad.name.ToLower();
        else return;

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

    private void DetectGamepad(InputDevice device, InputDeviceChange change)
    {
        if (rebind_key.isBinding) {
            rebind_key.ExitBind();
            return;
        }
        DetermineKeyOrPad();
    }

    public void DetermineKeyOrPad()
    {
        if (gameObject.activeSelf)
            StartCoroutine(DelayDetermine());
    }

    public IEnumerator DelayDetermine()
    {
        yield return WaitForCache.GetWaitForSecondReal(0.5f);
        joystickNames = Input.GetJoystickNames();

        if (joystickNames.Length == 0){ // preventing index error
            uiControl.UI_Update(true); // true => Activate Keyboard
            gamepadType = 0;
            yield break;
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

    private bool IsGamepad()
    {
        foreach (string name in joystickNames)
        {
            if (!string.IsNullOrEmpty(name))
                return true;
        }
        return false;
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
                string currentScheme = playerMovement.playerInput.actions["Move"].activeControl.ToString();

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

    public void UpdateConsumableSlots()
    {
        StartCoroutine(consumableNavigation.UpdateSlotValues());
    }

    public void UpdateOtherItemSlots()
    {
        StartCoroutine(otherItemNavigation.UpdateSlotValues());
    }

    private Vector3 GetBenchPostion()
    {
        GameObject[] benches = GameObject.FindGameObjectsWithTag("Bench");
        Vector3 benchPos = benches[Random.Range(0, benches.Length)].transform.position;
        return new Vector3(benchPos.x, benchPos.y - 1, benchPos.z);
    }

    public void SwitchActionMap(string mapName)
    {
        player_movement.playerInput.SwitchCurrentActionMap(mapName);
    }

    public static void DoDebug(string input)
    {
        if (!AppSettings.IsUnityEditor) return;
        Debug.Log(input);
    }
}

public static class AppSettings
{
#if UNITY_EDITOR
        public static bool IsUnityEditor = true;
#else
        public static bool IsUnityEditor = false;

#endif
}