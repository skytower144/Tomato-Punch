using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public event Action OnBattleOver;
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

}
