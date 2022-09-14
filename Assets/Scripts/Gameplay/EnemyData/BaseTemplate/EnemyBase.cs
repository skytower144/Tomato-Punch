using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringStringDictionary : SerializableDictionary<string, string>{}

[CreateAssetMenu(fileName = "Enemy", menuName = "Create Enemy Base Data")]
public class EnemyBase : ScriptableObject
{   // ATTACKTYPE = LEFT: -1, RIGHT: 1, DOWN: -101, CENTER: 0
    [SerializeField] private string enemyName;
    [SerializeField] private List<Sprite> frontSprite;
    [SerializeField] private float enemyMaxHealth, enemyCurrentHealth;
    public int min_hitct, max_hitct;
    [SerializeField] private RuntimeAnimatorController animationController;
    [SerializeField] private string idle, intro, defeated, countered, suffer, parried, parriedToIdle, parriedAft, uppered, upperRecover, superedRecover, guard, hurtL, hurtR;
    [SerializeField] private string wait, reEngage, victory;

    [Header("Key: ItemName | Value: AnimationString name")]
    public StringStringDictionary EnemySuperedAnim;

    [SerializeField] private List <Enemy_ProjectileDetail> projectiles;
    [SerializeField] private List <Enemy_AttackDetail> enemyAttack;
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
    public string Parried_AnimationString
    {
        get { return parried; }
    }
    public string ParriedToIdle_AnimationString
    {
        get { return parriedToIdle; }
    }
    public string ParriedAft_AnimationString
    {
        get { return parriedAft; }
    }
    public string Uppered_AnimationString
    {
        get { return uppered; }
    }
    public string UpperRecover_AnimationString
    {
        get { return upperRecover; }
    }
    public string SuperedRecover_AnimationString
    {
        get { return superedRecover; }
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
    public Enemy_AttackDetail EnemyAttack(string attackName)
    {
        return enemyAttack.Find( x=> x.EnemyAttackName == attackName);
    }
    public Enemy_ProjectileDetail EnemyPjSelect(string pjName)
    {
        return projectiles.Find( x=> x.EnemyPjName == pjName);
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
