using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringStringDictionary : SerializableDictionary<string, string>{}

[CreateAssetMenu(fileName = "Enemy", menuName = "Create Enemy Base Data")]
public class EnemyBase : ScriptableObject
{   // ATTACKTYPE = LEFT: -1, RIGHT: 1, DOWN: -101, CENTER: 0
    [SerializeField] private string enemyName;
    [SerializeField] private List<Sprite> frontSprite;
    public BgName bgName;
    [SerializeField] private float enemyMaxHealth, enemyCurrentHealth;
    public int min_hitct, max_hitct;
    [SerializeField] private RuntimeAnimatorController animationController;
    [SerializeField] private string idle, intro, defeated, countered, suffer, stun, uppered, recover, guard, hurtL, hurtR, blasted, bounce, dunk;
    [SerializeField] private string wait, reEngage, victory;
    [SerializeField] List<string> otherHurtAnim;

    public ChiliAnimInfo chiliInfo;
    // Add more super info

    [SerializeField] private List <Enemy_ProjectileDetail> projectiles;
    [SerializeField] private List <Enemy_AttackDetail> enemyPattern;
    [SerializeField] private List <RewardDetail> itemReward;
    [SerializeField] private float battleExp;
    [SerializeField] private int battleCoin;


    public string EnemyName
    {
        get { return enemyName; }
    }
    public Sprite EnemyFace(int number)
    {
        if (number == 2)
            return frontSprite[1];
        else if(number == 3)
            return frontSprite[2];
        
        return frontSprite[0];
    }
    public float EnemyMaxHealth
    {
        get { return enemyMaxHealth; }
    }
    public float EnemyCurrentHealth
    {
        get { return enemyCurrentHealth; }
    }
    public RuntimeAnimatorController AnimationController
    {
        get { return animationController; }
    }
    public string Idle_AnimationString
    {
        get { return idle; }
    }
    public string Intro_AnimationString
    {
        get { return intro; }
    }
    public string Defeated_AnimationString
    {
        get { return defeated; }
    }
    public string Countered_AnimationString
    {
        get { return countered; }
    }
    public string Suffer_AnimationString
    {
        get { return suffer; }
    }
    public string Stun_AnimationString
    {
        get { return stun; }
    }
    public string Uppered_AnimationString
    {
        get { return uppered; }
    }
    public string Recover_AnimationString
    {
        get { return recover; }
    }
    public string Guard_AnimationString
    {
        get { return guard; }
    }
    public string HurtL_AnimationString
    {
        get { return hurtL; }
    }
    public string HurtR_AnimationString
    {
        get { return hurtR; }
    }
    public string Wait
    {
        get { return wait; }
    }
    public string ReEngage
    {
        get { return reEngage; }
    }
    public string Victory
    {
        get { return victory; }
    }
    public string Blasted
    {
        get { return blasted; }
    }
    public string Bounce
    {
        get { return bounce; }
    }
    public string Dunk
    {
        get { return dunk; }
    }

    public List<string> HurtAnimList
    {
        get { return otherHurtAnim.Concat(new List<string> {hurtL, hurtR}).ToList(); }
    }

    public Enemy_ProjectileDetail EnemyPjSelect(string pjName)
    {
        return projectiles.Find( x=> x.EnemyPjName == pjName);
    }
    public Enemy_AttackDetail EnemyAttack(string attackName)
    {
        return enemyPattern.Find( x=> x.EnemyAttackName == attackName);
    }

    public List<Enemy_AttackDetail> EnemyPattern
    {
        get { return enemyPattern; }
    }

    public List<RewardDetail> ItemReward
    {
        get { return itemReward; }
    }
    public float BattleExp
    {
        get { return battleExp; }
    }
    public int BattleCoin
    {
        get { return battleCoin; }
    }

}

public enum BgName { bg_tutorial }

[System.Serializable]
public class ChiliAnimInfo
{
    public Vector2 hitPosition;
    public Sprite hitSprite;
}
