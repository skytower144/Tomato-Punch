using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIControl : MonoBehaviour
{
    private const int MAXIDLECOUNT = 5;
    [SerializeField] private EnemyControl enemyCtrl;
    [SerializeField] private Animator battleAnim;
    [SerializeField] private tomatoGuard tomatoguard;
    [System.NonSerialized] public static bool enemy_isIntro = true;
    private List<EnemyActDetail> pattern_list;
    private int idleCount = 0;

    void OnEnable()
    {
        enemy_isIntro = true;
        idleCount = 0;
    }
    void EnemyMove(EnemyActDetail move)
    {
        EnemyControl.isPhysical = move.PhysicalAttack;

        if (move is Enemy_AttackDetail)
            enemyCtrl.SaveEnemyDmgFrames(((Enemy_AttackDetail)move).PhysicalAttackFrames);
        else if (move is Enemy_ProjectileDetail)
            enemyCtrl.SaveEnemyDmgFrames(((Enemy_ProjectileDetail)move).ProjectileAttackFrames);
        
        enemyCtrl.enemyAnimControl.Act(move);
    }

    private bool ShouldActivate()
    {
        return (
            !tomatoControl.isFainted &&
            !Enemy_is_hurt.enemy_isDefeated &&
            !Enemy_parried.isParried &&
            !Enemy_countered.enemy_isCountered &&
            !Enemy_is_hurt.enemy_isPunched &&
            !GameManager.gm_instance.assistManager.isBlast &&
            !enemyCtrl.enemy_supered &&
            enemyCtrl.enemyAnim.enabled
        );
    }

    public void ProceedAction()
    {
        if(!enemy_isIntro && IsIdle())
        {
            if(enemyCtrl.action_afterSuffer)
            {
                enemyCtrl.action_afterSuffer = false;
                return;
            }
            else if (ShouldActivate())
            {
                //Debug.Log($"Proceeding Action | Idle Count : {idleCount}");
                int randomPercent = Random.Range(1, 101);
                int sumPercent = 0;

                for (int i = 0; i < pattern_list.Count; i++)
                {
                    sumPercent += pattern_list[i].percentage;
                    if (sumPercent >= randomPercent)
                    {
                        EnemyActDetail selectedMove = pattern_list[i];
                        
                        if (OverMaxIdleCount(selectedMove)) {
                            //Debug.Log("=== Forcing Enemy Action. ===");
                            EnemyMove(pattern_list[(Random.Range(0, pattern_list.Count - 2))]);
                            return;
                        }
                        EnemyMove(selectedMove);
                        break;
                    }
                }
            }
        }
    }

    private bool IsIdle()
    {
        return battleAnim.GetCurrentAnimatorStateInfo(0).IsName(enemyCtrl._base.Idle_AnimationString);
    }

    private bool OverMaxIdleCount(EnemyActDetail selectedMove)
    {
        if (selectedMove.Name != enemyCtrl._base.Idle_AnimationString) {
            idleCount = 0;
            return false;
        }
        
        if (idleCount < MAXIDLECOUNT) {
            idleCount++;
            return false;
        }

        idleCount = 0;
        return true;
    }

    public void LoadEnemyPattern(List<EnemyActDetail> patternList)
    {
        pattern_list = patternList;
    }

    public void ResetEnemyPattern()
    {
        pattern_list = null;
    }
}
