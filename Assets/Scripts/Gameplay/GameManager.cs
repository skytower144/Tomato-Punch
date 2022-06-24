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
    [SerializeField] private GameObject battleCircle, exclamation, fadeOut;
    public GameObject playerObject;
    private Animator playerAnimator;
    private float player_x, player_y;
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
        StartCoroutine(BattleExit_Wait());
    }

    private void Update()
    {
        if(gameState == GameState.FreeRoam)
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

        GameObject fadeInstance = Instantiate(fadeOut, new Vector2 (player_x, player_y - 0.05f), Quaternion.identity);
        fadeInstance.transform.localScale = new Vector2(2f, 2f);
        
        mainCamera.gameObject.SetActive(true);

        PlayerMovement.isBattle = false;
    }

}
