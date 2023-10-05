using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyActDetail
{
    public string Name;
    public int percentage;
    // [TextArea(1,1)] public string LinkedMove;

    public bool PhysicalAttack => this is Enemy_AttackDetail;
}

[System.Serializable]
public class Enemy_IdleDetail : EnemyActDetail
{
    public Enemy_IdleDetail(string Name, int percentage)
    {
        this.Name = Name;
        this.percentage = percentage;
    }
}


[System.Serializable]
public class Enemy_NeutralDetail : EnemyActDetail
{
    public Enemy_NeutralDetail(string Name, int percentage)
    {
        this.Name = Name;
        this.percentage = percentage;
    }
}


[System.Serializable]
public class Enemy_AttackDetail : EnemyActDetail
{
    public List<PhysicalAttackFrame> PhysicalAttackFrames;
    public int LastDamageHitFrame => this.PhysicalAttackFrames[PhysicalAttackFrames.Count - 1].HitFrame;
    public Enemy_AttackDetail(string Name, int percentage, List<PhysicalAttackFrame> PhysicalAttackFrames = null)
    {
        this.Name = Name;
        this.percentage = percentage;
        this.PhysicalAttackFrames = PhysicalAttackFrames;
    }
}

[System.Serializable]
public class Enemy_ProjectileDetail : EnemyActDetail
{
    public List<ProjectileAttackFrame> ProjectileAttackFrames;
    public int LastDamageHitFrame => this.ProjectileAttackFrames[ProjectileAttackFrames.Count - 1].HitFrame;
    public Enemy_ProjectileDetail(string Name, int percentage, List<ProjectileAttackFrame> ProjectileAttackFrames = null)
    {
        this.Name = Name;
        this.percentage = percentage;
        this.ProjectileAttackFrames = ProjectileAttackFrames;
    }
}

[System.Serializable]
public class DamageFrame
{
    public float Damage;
    public AttackType EnemyAttackType;
    public HitType HitType;
    public int HitFrame;
}

[System.Serializable]
public class PhysicalAttackFrame : DamageFrame
{
    public int CounterStartFrame, CounterEndFrame;
}

[System.Serializable]
public class ProjectileAttackFrame : DamageFrame
{
    public int SpawnFrame;
    public GameObject Projectile;
}

public enum AttackType { LA, RA, DA, PJ }