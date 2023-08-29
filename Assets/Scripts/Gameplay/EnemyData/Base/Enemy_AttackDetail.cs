using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyActDetail
{
    public string Name;
    public int percentage;
    public float Damage;
    public AttackType EnemyAttackType;
    public int HitFrame;
    public bool PhysicalAttack => ((EnemyAttackType != AttackType.PJ) && (EnemyAttackType != AttackType.NEUTRAL));
}

[System.Serializable]
public class Enemy_AttackDetail : EnemyActDetail
{
    public int CounterStartFrame, CounterEndFrame;

    public Enemy_AttackDetail(string Name, int percentage, float Damage, AttackType EnemyAttackType, int CounterStartFrame = 0, int CounterEndFrame = 0, int HitFrame = 0)
    {
        this.Name = Name;
        this.percentage = percentage;
        this.Damage = Damage;
        this.EnemyAttackType = EnemyAttackType;
        this.CounterStartFrame = CounterStartFrame;
        this.CounterEndFrame = CounterEndFrame;
        this.HitFrame = HitFrame;
    }
}

[System.Serializable]
public class Enemy_ProjectileDetail : EnemyActDetail
{
    public int SpawnFrame;
    public GameObject Projectile;

    public Enemy_ProjectileDetail(string Name, int percentage, float Damage, AttackType EnemyAttackType, int SpawnFrame, int HitFrame, GameObject Projectile)
    {
        this.Name = Name;
        this.percentage = percentage;
        this.Damage = Damage;
        this.EnemyAttackType = EnemyAttackType;
        this.SpawnFrame = SpawnFrame;
        this.HitFrame = HitFrame;
        this.Projectile = Projectile;
    }
}

public enum AttackType { LA, RA, DA, PJ, NEUTRAL }