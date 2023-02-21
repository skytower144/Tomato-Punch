using UnityEngine;

[System.Serializable]
public class DefeatGoal : Goal
{
    [SerializeField] private string targetEnemy;

    public override void Init()
    {
        GameManager.gm_instance.battle_system.OnEnemyDefeat -= EnemyDefeated;
        GameManager.gm_instance.battle_system.OnEnemyDefeat += EnemyDefeated;
    }

    private void EnemyDefeated(EnemyBase enemy)
    {
        if (targetEnemy == enemy.EnemyName)
        {
            this.currentAmount++;
        }
    }
}
