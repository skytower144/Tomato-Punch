using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Create Enemy Base Data")]
public class EnemyBase : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Sprite frontSprite;

    // Base Stats
    [SerializeField] private List <Enemy_NameDmg_Pair> enemyAttack;


    public string EnemyName
    {
        get { return enemyName; }
    }
    
    public Enemy_NameDmg_Pair EnemyAttack(string attackName)
    {
        return enemyAttack.Find( x=> x.EnemyAttackName == attackName);
    }
    // LEFT: -1, RIGHT: 1, DOWN: 0

}
