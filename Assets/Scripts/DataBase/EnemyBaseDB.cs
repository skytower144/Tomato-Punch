using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseDB
{
    static Dictionary<string, EnemyBase> EnemyCatalog;

    public static void Initialize()
    {
        EnemyCatalog = new Dictionary<string, EnemyBase>();

        var enemyArray = Resources.LoadAll<EnemyBase>("EnemyBase/");

        foreach (EnemyBase enemy in enemyArray)
        {
            if (EnemyCatalog.ContainsKey(enemy.EnemyName))
            {
                Debug.LogError($"Detected multiple enemy named : {enemy.EnemyName}");
                continue;
            }

            EnemyCatalog[enemy.EnemyName] = enemy;
        }
    }

    public static EnemyBase ReturnEnemyOfName(string name)
    {
        if (!EnemyCatalog.ContainsKey(name))
        {
            Debug.LogError($"Enemy named : {name} not found in the EnemyCatalog");
            return null;
        }

        return EnemyCatalog[name];
    }
}
