using UnityEngine;

[System.Serializable]
public class DefeatGoal : Goal
{
    [SerializeField] EnemyBase targetEnemy;

    public override void Init()
    {
        GameManager.gm_instance.battle_system.OnEnemyDefeat -= EnemyDefeated;
        GameManager.gm_instance.battle_system.OnEnemyDefeat += EnemyDefeated;
    }

    private void EnemyDefeated(EnemyBase enemy)
    {
        if (targetEnemy.EnemyName == enemy.EnemyName)
        {
            this.currentAmount++;
        }
    }
}
