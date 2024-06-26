using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIControl : MonoBehaviour
{
    private const int MAXIDLECOUNT = 3;
    [SerializeField] private EnemyControl enemyCtrl;
    [SerializeField] private Animator battleAnim;
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

    public bool ShouldActivate()
    {
        return (
            !enemy_isIntro &&
            !tomatoControl.isFainted &&
            !Enemy_parried.isParried &&
            !Enemy_countered.enemy_isCountered &&
            !Enemy_is_hurt.enemy_isDefeated &&
            !Enemy_is_hurt.enemy_isPunched &&
            !Enemy_is_hurt.enemyIsHit &&
            !GameManager.gm_instance.assistManager.isBlast &&
            !GameManager.gm_instance.battle_system.IsNextPhase &&
            !enemyCtrl.enemy_supered &&
            !enemyCtrl.enemy_hurt.isGuarding &&
            !enemyCtrl.isRecovering &&
            enemyCtrl.enemyAnim.enabled
        );
    }

    public void ProceedAction()
    {
        if (IsIdle())
        {
            if (enemyCtrl.action_afterSuffer)
            {
                enemyCtrl.action_afterSuffer = false;
                return;
            }
            else if (ShouldActivate())
            {
                EnemyActDetail selectedMove = GetRandomEnemyAct();
                        
                if (OverMaxIdleCount(selectedMove)) {
                    GameManager.DoDebug("=== Forcing Enemy Action. ===");
                    EnemyMove(GetRandomEnemyAct(true));
                    return;
                }
                EnemyMove(selectedMove);
            }
        }
    }

    private EnemyActDetail GetRandomEnemyAct(bool excludeIdleAct = false)
    {
        int idlePercent = pattern_list[pattern_list.Count - 1].percentage;
        int maxPercent = excludeIdleAct ? 100 - idlePercent : 100;

        int sumPercent = 0;
        int randomPercent = Random.Range(1, maxPercent + 1);
        int totalActCount = excludeIdleAct ? pattern_list.Count - 1 : pattern_list.Count;

        for (int i = 0; i < totalActCount; i++)
        {
            sumPercent += pattern_list[i].percentage;
            if (sumPercent >= randomPercent) {
                // GameManager.DoDebug($"{pattern_list[i].Name} - {randomPercent}% | sum : {sumPercent}");
                return pattern_list[i];
            }
        }
        return pattern_list[pattern_list.Count - 1];
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
    public void ChangeEnemyPattern(int phaseIndex)
    {
        int sumPercent = 0;
        List<EnemyActDetail> phaseActs = enemyCtrl._base.Phases[phaseIndex].attacks;

        if (phaseActs.Count != pattern_list.Count - 1)
            Debug.LogWarning("Phase 2 act count does not match with Phase 1.");
        
        foreach (EnemyActDetail refAct in phaseActs) {
            bool flag = false;

            foreach (EnemyActDetail act in pattern_list) {
                if (act.Name == refAct.Name) {
                    act.percentage = refAct.percentage;
                    sumPercent += refAct.percentage;
                    flag = true;
                    break;
                }
            }
            if (!flag)
                Debug.LogWarning($"{refAct.Name} : Could not change enemy pattern.");
        }
        pattern_list[pattern_list.Count - 1].percentage = 100 - sumPercent;
        GameManager.DoDebug($"Enemy idle percent : {100 - sumPercent}%");
    }
    public EnemyActDetail ReturnEnemyPattern(string name)
    {
        foreach (EnemyActDetail act in pattern_list) {
            if (act.Name == name) return act;
        }
        Debug.LogError($"{name} : Enemy Act not found.");
        return null;
    }
}
