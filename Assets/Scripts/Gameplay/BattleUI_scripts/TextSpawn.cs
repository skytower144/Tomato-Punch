using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TextSpawn : MonoBehaviour
{
    [System.NonSerialized] static public bool isMiss = false;
    [SerializeField] private BattleSystem textSpawn_BattleSystem;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private EnemyControl enemyControl;
    [SerializeField] private Enemy_countered enemy_Countered;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private RebindKey rebindKey; public RebindKey rebind_key => rebindKey;
    [SerializeField] private GameObject GetReadyText;
    [SerializeField] private GameObject startingCartridge, KOText, YouWin_Text, YouLose_Text, FIGHT_Text, dark_filter, missEffect, resultCard, continueBundle;
    private Vector3 randomPosition;

    public void SwitchCartridge(GameObject inputCartridge)
    {
        startingCartridge = inputCartridge;
    }
    private void OnEnable()
    {
        Instantiate(startingCartridge, transform);
        Invoke("playIntro", 0.65f);
    }
    private void OnDisable()
    {
        normalize_resultCard();
        startingCartridge = GetReadyText;
    }

    void Update()
    {
        if (isMiss && !TutorialMode.isTutorial)
        {
            randomPosition = Random.insideUnitSphere * 1.5f + new Vector3(-30,0,0);
            GameObject miss = Instantiate(missEffect, transform);
            miss.transform.position = randomPosition;

            decreaseStamina();
            isMiss = false;
        }
    }

    void decreaseStamina()
    {
        tomatocontrol.currentStamina -= 1;
        if (tomatocontrol.currentStamina < 0)
            tomatocontrol.currentStamina = 0;

        staminaIcon.SetStamina(tomatocontrol.currentStamina);
    }

    private void playIntro()
    {
        tomatocontrol.tomatoAnim.Play("tomato_intro",-1, 0f);
        enemyControl.enemyAnim.Play(enemyControl._base.Intro_AnimationString, -1, 0f);
    }

    public void spawn_KO_text()
    {
        Instantiate(KOText, transform);
    }

    public void spawn_FIGHT_text()
    {
        Instantiate(FIGHT_Text, transform);
    }

    public void PlayVictory_Player()
    {
        tomatocontrol.tomatoAnim.Play("tomato_victory", -1, 0f);
        Instantiate(YouWin_Text, transform);
        Instantiate(dark_filter, transform);
        GameObject resultCard_obj = Instantiate(resultCard, transform);

        resultCard_obj.GetComponent<ResultCard>().ResultCard_Initialize(
            enemy_Countered.totalCounter, 
            EnemyControl.totalParry, 
            enemyControl.totalSuper, 
            textSpawn_BattleSystem, 
            enemyControl._base
            );
    }

    public void PlayDefeated_Player()
    {
        GameObject you_lose = Instantiate(YouLose_Text, transform);
        you_lose.GetComponent<DefeatedText>().battleSystem = textSpawn_BattleSystem;
    }

    public void normalize_resultCard()
    {
        enemy_Countered.totalCounter = 0;
        EnemyControl.totalParry = 0;
        enemyControl.totalSuper = 0;
    }

    public void AskContinue()
    {
        int revive_cost = textSpawn_BattleSystem.ReviveCostFormula();
        int player_money = textSpawn_BattleSystem.GetPlayerMoney();

        if (player_money >= revive_cost){
            GameObject battle_continue = Instantiate(continueBundle, transform);
            battle_continue.GetComponent<BattleContinue>().Continue_InitializeData(textSpawn_BattleSystem, KOText);
        }
        else{
            spawn_KO_text();
        }
    }
}
