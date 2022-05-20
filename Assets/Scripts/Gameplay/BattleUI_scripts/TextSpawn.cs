using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TextSpawn : MonoBehaviour
{
    [SerializeField] private GameObject missEffect;
    [System.NonSerialized] static public bool isMiss = false;
    [SerializeField] private BattleSystem textSpawn_BattleSystem;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private EnemyControl enemyControl;
    [SerializeField] private Enemy_countered enemy_Countered;
    [SerializeField] private Animator tomatoAnim;
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private StaminaIcon staminaIcon;
    [SerializeField] private GameObject GetReadyText, KOText, YouWin_Text, dark_filter, resultCard;

    private Vector3 randomPosition;

    private void OnEnable()
    {
        Instantiate(GetReadyText, transform);
        Invoke("playIntro", 0.65f);
    }

    void Update()
    {
        if (isMiss)
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
        tomatoAnim.Play("tomato_intro",-1, 0f);
        enemyAnim.Play(enemyControl._base.Intro_AnimationString, -1, 0f);
    }

    public void spawn_KO_text()
    {
        GameObject KO = Instantiate(KOText, transform);
        KO.GetComponent<InitiateVictory>().script_textSpawn = gameObject.GetComponent<TextSpawn>();
    }

    public void PlayVictory_Player()
    {
        tomatoAnim.Play("tomato_victory", -1, 0f);
        Instantiate(YouWin_Text, transform);
        Instantiate(dark_filter, transform);
        GameObject resultCard_obj = Instantiate(resultCard, transform);

        resultCard_obj.GetComponent<ResultCard>().ResultCard_Initialize(enemy_Countered.totalCounter, Enemy_parried.totalParry, enemyControl.totalSuper);
        resultCard_obj.GetComponent<ResultCard>().battleSystem = textSpawn_BattleSystem;
    }

    public void normalize_resultCard()
    {
        enemy_Countered.totalCounter = 0;
        Enemy_parried.totalParry = 0;
        enemyControl.totalSuper = 0;
    }
}
