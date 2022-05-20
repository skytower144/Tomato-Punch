using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3rd_Controller : MonoBehaviour, Interactable
{
    [SerializeField] private EnemyControl enemyControl;
    public EnemyBase enemyStats;
    [SerializeField] private GameManager gameManager;

    public void Interact()
    {
        gameManager.Initiate_Battle();
        enemyControl._base = enemyStats;
    }

}

