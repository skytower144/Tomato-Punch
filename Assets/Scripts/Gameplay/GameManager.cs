using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState { FreeRoam, Battle }
public class GameManager : MonoBehaviour
{
    GameState gameState;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] ResolutionMenu resolutionMenu;
    [SerializeField] RebindKey rebindKey;
    [SerializeField] ControlScroll controlScroll;
    [SerializeField] UIControl uiControl;
    [SerializeField] Camera mainCamera;
    [SerializeField] private GameObject battleCircle, exclamation, fadeIn;
    public GameObject playerObject;
    private Animator playerAnimator;

    public float stickSensitivity;

    [System.NonSerialized] public float holdStartTime = float.MaxValue;
    [SerializeField] private float holdTimer;
    [SerializeField] private float intervalTime;
    private float delayTimer;
    public bool WasHolding => holdStartTime < Time.time;

    private float player_x, player_y;
    private void Start() //subscribing to an event
    {
        resolutionMenu.SetupGraphic();

        battleSystem.OnBattleOver += EndBattle;
        //playerMovement.BeginBattle += StartBattle;
    
        InvokeRepeating("DetectGamepad", 0f, 1f);
    }

    void StartBattle()
    {
        StartCoroutine(Wait());
        CancelInvoke("DetectGamepad");
    }

    void EndBattle()
    {
        StartCoroutine(BattleExit_Wait());
        InvokeRepeating("DetectGamepad", 0f, 1f);
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

    public void Initiate_Battle()
    {
        PlayerMovement.isBattle = true;

        playerAnimator = playerObject.GetComponent<Animator>();
        playerAnimator.SetBool("isWalking",false);

        player_x = GameObject.Find("Player").transform.position.x;
        player_y = GameObject.Find("Player").transform.position.y;

        Instantiate (exclamation, new Vector2 (player_x-0.05f, player_y+3f), Quaternion.identity);
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
    }

    IEnumerator BattleExit_Wait()
    {
        yield return new WaitForSeconds(1.5f);

        battleSystem.gameObject.SetActive(false);
        gameState = GameState.FreeRoam;

        GameObject fadeInstance = Instantiate(fadeIn, new Vector2 (player_x, player_y - 0.05f), Quaternion.identity);
        fadeInstance.transform.localScale = new Vector2(2f, 2f);
        
        mainCamera.gameObject.SetActive(true);

        PlayerMovement.isBattle = false;
    }

    private void DetectGamepad()
    {
        if (!rebindKey.isBinding)
        {
            string [] names = Input.GetJoystickNames();

            if (names.Length == 0){ // preventing index error
                uiControl.UI_Update(true); // true => Activate Keyboard
                return;
            }
            
            if(string.IsNullOrEmpty(names[0]))
                uiControl.UI_Update(true);
            
            else
                uiControl.UI_Update(false);
        }
    }

    public void DetectHolding(Action callback)
    {
        if (WasHolding)
        {
            if (Time.time - holdStartTime > holdTimer)
            {
                delayTimer -= Time.deltaTime;
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
            holdStartTime = Time.time;

            callback?.Invoke();
        }
    }
}
