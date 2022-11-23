using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIControl : MonoBehaviour
{
    [SerializeField] private EnemyControl enemyCtrl;
    [SerializeField] private Animator battleAnim;
    [SerializeField] private tomatoGuard tomatoguard;
    [System.NonSerialized] public static bool enemy_isIntro = true;
    [System.NonSerialized] public List<Enemy_AttackDetail> pattern_list = new List<Enemy_AttackDetail>();

    void OnEnable()
    {
        enemy_isIntro = true;
    }
    void EnemyMove(Enemy_AttackDetail move)
    {
        EnemyControl.isPhysical = move.PhysicalAttack;
        tomatoguard.damage = move.EnemyAttackDmg;
        enemyCtrl.attackType = move.EnemyAttackType;
        enemyCtrl.pjTag = move.EnemyAttackName;
        battleAnim.Play(move.EnemyAttackName);
    }

    private bool ShouldActivate()
    {
        return (!tomatoControl.isFainted && !Enemy_is_hurt.enemy_isDefeated && !Enemy_parried.isParried && !Enemy_countered.enemy_isCountered && !Enemy_is_hurt.enemy_isPunched && !enemyCtrl.enemy_supered);
    }

    public void ProceedAction()
    {
        if(!enemy_isIntro)
        {
            if(enemyCtrl.action_afterSuffer)
            {
                enemyCtrl.action_afterSuffer = false;
                return;
            }
            else if (ShouldActivate())
            {
                int randomPercent = Random.Range(0, 101);
                int sumPercent = 0;

                for (int i = 0; i < pattern_list.Count; i++)
                {
                    sumPercent += pattern_list[i].percentage;
                    if (sumPercent >= randomPercent)
                    {
                        Enemy_AttackDetail selectedMove = pattern_list[i];
                        EnemyMove(selectedMove);
                        break;
                    }
                }
            }
        }
    }
}
