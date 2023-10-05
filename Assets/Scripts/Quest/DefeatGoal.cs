using UnityEngine;

[System.Serializable]
public class DefeatGoal : Goal
{
    [SerializeField] EnemyBase targetEnemy;
    [SerializeField, HideInInspector] public string TargetEnemyName;
    
    public override void Init()
    {
        GameManager.gm_instance.battle_system.OnEnemyDefeat -= EnemyDefeated;
        GameManager.gm_instance.battle_system.OnEnemyDefeat += EnemyDefeated;
    }

    public override void UnsubsribeEvent()
    {
        GameManager.gm_instance.battle_system.OnEnemyDefeat -= EnemyDefeated;
    }

    private void EnemyDefeated(EnemyBase enemy)
    {
        if (targetEnemy.EnemyName == enemy.EnemyName)
        {
            this.currentAmount++;
        }
    }
    public void SerializeEnemyName()
    {
        TargetEnemyName = targetEnemy.EnemyName;
    }

    public void DeSerializeEnemyName()
    {
        targetEnemy = EnemyBaseDB.ReturnEnemyOfName(TargetEnemyName);
    }
}
