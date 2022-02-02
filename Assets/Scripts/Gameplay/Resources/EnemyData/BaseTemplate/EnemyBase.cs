using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Create Enemy Base Data")]
public class EnemyBase : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private string counteredAnimationString;
    [SerializeField] private string parriedAnimationString;
    [SerializeField] private List <Enemy_ProjectileDetail> projectiles;
    [SerializeField] private List <Enemy_AttackDetail> enemyAttack;

    public string CounteredAnimationString
    {
        get { return counteredAnimationString; }
    }
    public string ParriedAnimationString
    {
        get { return parriedAnimationString; }
    }
    // ATTACKTYPE = LEFT: -1, RIGHT: 1, DOWN: 0
    public Enemy_AttackDetail EnemyAttack(string attackName)
    {
        return enemyAttack.Find( x=> x.EnemyAttackName == attackName);
    }
    public Enemy_ProjectileDetail EnemyPjSelect(string pjName)
    {
        return projectiles.Find( x=> x.EnemyPjName == pjName);
    }

}
