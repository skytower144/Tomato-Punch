using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public event Action OnBattleOver;
    [SerializeField] private tomatoStatus player_status;
    [SerializeField] private TomatoLevel tomatoLevel;
    [SerializeField] private EnemyControl enemyControl;
    [SerializeField] private Animator tomatoAnim, enemyAnim;
    [SerializeField] private Transform battleCanvas_transform, tomato_transform;
    [SerializeField] private GameObject battle_initiate_fade, darkScreen, coinFlip;
    [System.NonSerialized] public bool resetPlayerHealth, resetEnemyHealth;
    void Start()
    {
        resetPlayerHealth = false;
        resetEnemyHealth = false;
    }
    void OnEnable()
    {
        Instantiate(battle_initiate_fade);
    }
    // public void HandleUpdate()
    // {
        
        
    // }
    public void ExitBattle()
    {
        OnBattleOver();
        enemyControl.ClearAnimation();
    }

    public void UpdatePlayerStatus(int level, float maxExp, float currentExp, float gainExp, int coin, List<RewardDetail> dropList)
    {
        // Gain STATPOINTS if player leveled up.
        player_status.player_statPt += (level - tomatoLevel.playerLevel) * player_status.STATPOINT;

        tomatoLevel.playerLevel = level;
        tomatoLevel.levelText.text = string.Format("Lv {0}", level); 

        tomatoLevel.expFill.maxValue = maxExp;
        tomatoLevel.expFill.value = currentExp;

        player_status.player_totalExp += gainExp;
        player_status.player_leftExp = maxExp - currentExp;
        player_status.playerMoney += coin;

        for (int i=0; i < dropList.Count; i++)
        {
            Inventory.instance.AddItem(dropList[i].RewardItem);
        }
    }

    public ExpBundle GetExp()
    {
        ExpBundle expBundle = ScriptableObject.CreateInstance<ExpBundle>();
        expBundle.player_level = tomatoLevel.playerLevel;
        expBundle.player_max_exp = tomatoLevel.expFill.maxValue;
        expBundle.player_current_exp = tomatoLevel.expFill.value;

        return expBundle;
    }

    public int GetPlayerMoney()
    {
        return player_status.playerMoney;
    }

    public EnemyBase GetEnemyBase()
    {
        return enemyControl._base;
    }

    public void UpdatePlayerMoney(int lostCoins)
    {
        if (player_status.playerMoney < lostCoins)
            lostCoins = player_status.playerMoney;
        
        player_status.playerMoney -= lostCoins;
    }

    public Animator GetEnemyAnim()
    {
        return enemyAnim;
    }

    public Animator GetTomatoAnim()
    {
        return tomatoAnim;
    }

    public void CoinFlip()
    {
        Instantiate(darkScreen, battleCanvas_transform);
        Instantiate(coinFlip, tomato_transform);
    }

    public void ScreenFlash()
    {
        Destroy(tomato_transform.GetChild(3).gameObject);
        battleCanvas_transform.GetChild(4).gameObject.GetComponent<Animator>().Play("dark_screen");
    }

    public int ReviveCostFormula()
    {
        return Mathf.FloorToInt(GetEnemyBase().BattleCoin / 3);
    }
}
