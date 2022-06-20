using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIControl : MonoBehaviour
{
    [SerializeField] private EnemyControl enemyCtrl;
    [SerializeField] private Animator battleAnim;
    [SerializeField] private tomatoGuard tomatoguard;
    [System.NonSerialized] public static bool enemy_isIntro = true;
    void OnEnable()
    {
        enemy_isIntro = true;
    }
    void EnemyMove(string movename)
    {
        Enemy_AttackDetail EM = enemyCtrl._base.EnemyAttack(movename);
        EnemyControl.isPhysical = EM.PhysicalAttack;
        tomatoguard.damage = EM.EnemyAttackDmg;
        enemyCtrl.attackType = EM.EnemyAttackType;
        battleAnim.Play(movename);
    }
    public void Jola() // function name must be exact as EnemyBase.EnemyName
    {
        if(!enemy_isIntro) {
            if(enemyCtrl.action_afterSuffer)
            {
                enemyCtrl.action_afterSuffer = false;
                return;
            }
            else if(!Enemy_is_hurt.enemy_isDefeated && !Enemy_parried.isParried && !Enemy_countered.enemy_isCountered && !Enemy_is_hurt.enemy_isPunched && !enemyCtrl.enemy_supered)
            {
                
                if((Random.value<= 0.1))
                {
                    battleAnim.Play("battleJola_idle");
                }
                else if((Random.value<= 0.5))
                {
                    EnemyMove("battleJola_DA");
                }
                else if((Random.value<= 0.45))
                {
                    EnemyMove("battleJola_LA");
                }
                else if((Random.value<= 0.3))
                {
                    EnemyMove("battleJola_RA");
                }
                
                
                else if((Random.value<= 0.2))
                {
                    enemyCtrl.pjTag = "battleJola_HatAttack";
                    EnemyMove("battleJola_HatAttack");
                }
            }
        }
    }

    public void Slime()
    {
        Debug.Log("slime is active!");
    }
}
