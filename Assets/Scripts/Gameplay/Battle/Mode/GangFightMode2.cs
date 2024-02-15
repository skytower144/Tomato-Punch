using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GangFightMode2 : MonoBehaviour
{
    [SerializeField] private GangFightMode _gangFight; 
    [SerializeField] private Animator _anim;

    void DisableIsAttacking(string idle)
    {
        _gangFight.IsAction = false;
        _anim.Play(idle, -1, 0f);
    }

    void HitFrame(string type)
    {
        // LEFT_PH | LEFT_PJ
        string[] typeInfo = type.Split('_');

        _gangFight.CurrentGuy = gameObject;
        _gangFight.CurrentState = typeInfo[1] == "PH" ? GangFightMode.GangParry.Physical : GangFightMode.GangParry.Projectile;
        _gangFight.SpawnAttackBox(typeInfo[0]);
    }
    void HitDamage(float dmg)
    {
        GameManager.gm_instance.battle_system.enemy_control.gangFightDmg = dmg;
    }
}
