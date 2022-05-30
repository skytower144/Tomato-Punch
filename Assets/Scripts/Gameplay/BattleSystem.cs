using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public event Action OnBattleOver;
    [SerializeField] private tomatoStatus player_status;
    [SerializeField] private TomatoLevel tomatoLevel;
    [SerializeField] private GameObject battle_initiate_fade;
    
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
    }

    public void UpdatePlayerStatus(int level, float maxExp, float currentExp, int coin)
    {
        tomatoLevel.playerLevel = level;
        tomatoLevel.levelText.text = string.Format("Lv {0}", level); 

        tomatoLevel.expFill.maxValue = maxExp;
        tomatoLevel.expFill.value = currentExp;

        player_status.playerMoney += coin;
    }

    public ExpBundle GetExp()
    {
        ExpBundle expBundle = ScriptableObject.CreateInstance<ExpBundle>();
        expBundle.player_level = tomatoLevel.playerLevel;
        expBundle.player_max_exp = tomatoLevel.expFill.maxValue;
        expBundle.player_current_exp = tomatoLevel.expFill.value;

        return expBundle;
    }

}
