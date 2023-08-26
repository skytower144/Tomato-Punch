using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy_AttackDetail
{
    public string EnemyAttackName;
    public bool PhysicalAttack;
    public int percentage;
    public float EnemyAttackDmg;
    public AttackType EnemyAttackType;

    public Enemy_AttackDetail(string EnemyAttackName, bool PhysicalAttack, int percentage, float EnemyAttackDmg, AttackType EnemyAttackType)
    {
        this.EnemyAttackName = EnemyAttackName;
        this.PhysicalAttack = PhysicalAttack;
        this.percentage = percentage;
        this.EnemyAttackDmg = EnemyAttackDmg;
        this.EnemyAttackType = EnemyAttackType;
    }
}

public enum AttackType { LA, RA, DA, PJ, NEUTRAL }
