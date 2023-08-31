using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;
    public Action<EnemyBase> OnEnemyDefeat;
    public TextSpawn textSpawn;
    public BattleUI_Control battleUI_Control;

    [SerializeField] private tomatoControl tomatocontrol; public tomatoControl tomato_control => tomatocontrol;
    [SerializeField] private tomatoStatus player_status; public tomatoStatus tomatostatus => player_status;
    [SerializeField] private TomatoLevel tomatoLevel;
    [SerializeField] private EnemyControl enemyControl; public EnemyControl enemy_control => enemyControl;
    public tomatoHurt tomato_hurt;
    public BattleTimeManager battleTimeManager;
    public FeatherPoints featherPointManager;
    public BackgroundAnimator backgroundAnim;
    public ShockWaveEffect ShockWaveControl;
    public BackgroundParallax parallax;
    
    [SerializeField] private Animator tomatoAnim, enemyAnim;
    [SerializeField] private Transform battleCanvas_transform, tomato_transform;
    [SerializeField] private GameObject battle_initiate_fade, darkScreen, coinFlip, battle_end_circle;

    [SerializeField] private Image fixedBg;
    [SerializeField] private RawImage parallaxBg;

    [SerializeField] private float parryBounusPoints; public float parryBonus => parryBounusPoints;
    [SerializeField] private int missPenaltyStamina; public int missStamina => missPenaltyStamina;
    [SerializeField] private int blockPenaltyStamina; public int blockStamina => blockPenaltyStamina;
    [SerializeField] private int evadeBonusStamina; public int evadeStamina => evadeBonusStamina;

    [field: SerializeField] public ShockWaveInfo BlastShockWave { get; private set; }
    [field: SerializeField] public ShockWaveInfo DunkShockWave { get; private set; }

    [System.NonSerialized] public bool resetPlayerHealth, resetEnemyHealth;
    private GameObject tempObj;

    void Start()
    {
        resetPlayerHealth = false;
        resetEnemyHealth = false;
    }

    void OnEnable()
    {
        Instantiate(battle_initiate_fade);
    }

    void OnDisable()
    {
        fixedBg.sprite = null;
        parallaxBg.texture = null;
    }

    void Update()
    {
        DebugBools.DebugFunctions();
    }

    public void ExitBattle(bool isVictory)
    {
        if (!isVictory && (tomato_control.currentHealth == 0))
            tomato_control.currentHealth = 1;
        
        Destroy(Instantiate(battle_end_circle), 2f);
        OnBattleOver?.Invoke(isVictory);
    }

    public void SetBg(EnemyBase enemyBase)
    {
        fixedBg.gameObject.SetActive(false);
        parallaxBg.gameObject.SetActive(false);

        if (enemyBase.isFixedBg) {
            backgroundAnim.SetBackground(enemyBase.bgSprites);
            fixedBg.gameObject.SetActive(true);
        }
        if (enemyBase.isParallaxBg) {
            parallaxBg.texture = enemyBase.bgTexture;
            parallax.SetParallaxDirection(enemyBase.parallaxDirection);
            parallaxBg.gameObject.SetActive(true);
        }
    }

    public void UpdatePlayerStatus(int updated_level, float maxExp, float currentExp, float gainExp, int coin, List<RewardDetail> dropList)
    {
        // Gain STATPOINTS if player leveled up.
        player_status.player_statPt += (updated_level - tomatoLevel.playerLevel) * player_status.STATPOINT;

        tomatoLevel.playerLevel = updated_level;

        tomatoLevel.expFill.maxValue = maxExp;
        tomatoLevel.expFill.value = currentExp;

        player_status.player_totalExp += gainExp;
        player_status.player_leftExp = maxExp - currentExp;
        player_status.playerMoney += coin;

        for (int i=0; i < dropList.Count; i++)
        {
            Inventory.instance.AddItem(dropList[i].RewardItem);
        }
    }

    public ExpBundle GetExp()
    {
        ExpBundle expBundle = ScriptableObject.CreateInstance<ExpBundle>();
        expBundle.player_level = tomatoLevel.playerLevel;
        expBundle.player_max_exp = tomatoLevel.expFill.maxValue;
        expBundle.player_current_exp = tomatoLevel.expFill.value;

        return expBundle;
    }

    public int GetPlayerMoney()
    {
        return player_status.playerMoney;
    }

    public EnemyBase GetEnemyBase()
    {
        return enemyControl._base;
    }

    public void UpdatePlayerMoney(int lostCoins)
    {
        if (player_status.playerMoney < lostCoins)
            lostCoins = player_status.playerMoney;
        
        player_status.playerMoney -= lostCoins;
    }

    public Animator GetEnemyAnim()
    {
        return enemyAnim;
    }

    public Animator GetTomatoAnim()
    {
        return tomatoAnim;
    }

    public void CoinFlip()
    {
        tempObj = Instantiate(darkScreen, battleCanvas_transform);
        Instantiate(coinFlip, tomato_transform);
    }

    public void ScreenFlash()
    {
        Destroy(tomato_control.tempObj);
        tempObj.GetComponent<Animator>().Play("dark_screen");
    }

    public int ReviveCostFormula()
    {
        return Mathf.FloorToInt(GetEnemyBase().BattleCoin / 3);
    }

    public bool Press_Key(string moveName)
    {
        return tomatocontrol.PressKey(moveName);
    }
}
