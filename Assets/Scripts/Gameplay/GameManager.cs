using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }
public class GameManager : MonoBehaviour
{
    GameState gameState;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCamera;

    private void Start() //subscribing to an event
    {
        battleSystem.OnBattleOver += EndBattle;
        playerMovement.BeginBattle += StartBattle;
    }
    void StartBattle()
    {
        StartCoroutine(Wait());
    }

    void EndBattle()
    {
        gameState = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        PlayerMovement.isBattle = false;
    }

    private void Update()
    {
        if(gameState == GameState.FreeRoam)
        {
            playerMovement.HandleUpdate();
        }
        else if(gameState == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);

        mainCamera.gameObject.SetActive(false);
        gameState = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        
    }
}
