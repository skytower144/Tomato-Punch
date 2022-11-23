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

}

public enum AttackType { LA, RA, DA, PJ, NEUTRAL }
