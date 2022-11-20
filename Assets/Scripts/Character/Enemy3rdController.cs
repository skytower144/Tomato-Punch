using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3rdController : MonoBehaviour, Interactable
{
    public EnemyBase enemyStats;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.gm_instance;
    }

    public void Interact()
    {
        gameManager.Initiate_Battle(enemyStats);
    }

}

