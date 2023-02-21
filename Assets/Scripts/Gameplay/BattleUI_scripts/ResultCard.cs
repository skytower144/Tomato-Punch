using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultCard : MonoBehaviour
{
    [System.NonSerialized] public BattleSystem battleSystem;
    [System.NonSerialized] public EnemyBase enemyBase;
    private TypeEffect typeEffect;
    private ResultCard_ExpBar resultCard_ExpBar;
    [System.NonSerialized] public int updateLevel;
    public float max_exp, current_exp;
    private int totalCounter_ct, totalParry_ct, totalSuper_ct, inputCount, textIndex;
    private float temp_ct;
    private float TEXTSPEED = 14f;

    private TextMeshProUGUI totalCounter_txt, totalParry_txt, totalSuper_txt;

    [SerializeField] TextMeshProUGUI CounterUI, ParryUI, SuperUI, BelowText;
    
    private List<string> resultTexts = new List<string>();
    private List<RewardDetail> droppedItems = new List<RewardDetail>();
    private ExpBundle expBundle;
    private bool start_textChange_counter, start_textChange_parry, start_textChange_super, data_isReady, isExit;
    private float totalExp;
    private int totalGold;

    void OnEnable()
    {
        typeEffect = transform.GetChild(4).gameObject.GetComponent<TypeEffect>();

        inputCount = 0;
        textIndex = -1;
        data_isReady = false;
        isExit = false;
    }

    private void SetLanguage()
    {
        CounterUI.text = UIControl.instance.uiTextDict["Resultcard_Counter_Text"];
        UIControl.instance.SetFontData(CounterUI, "Resultcard_Counter_Text");

        ParryUI.text = UIControl.instance.uiTextDict["Resultcard_Parry_Text"];
        UIControl.instance.SetFontData(ParryUI, "Resultcard_Parry_Text");

        SuperUI.text = UIControl.instance.uiTextDict["Resultcard_Super_Text"];
        UIControl.instance.SetFontData(SuperUI, "Resultcard_Super_Text");

        BelowText.text = "";
        UIControl.instance.SetFontData(BelowText, "BattleWon_ExpMessage");
    }

    void Update()
    {
        if (start_textChange_counter && temp_ct <= (float)totalCounter_ct)
        {
            temp_ct += Time.deltaTime * TEXTSPEED;
            if(CheckText(temp_ct, (float)totalCounter_ct))
            {
                start_textChange_counter = false;
                totalCounter_txt.color = new Color32(248, 131, 50, 255);
            }
            totalCounter_txt.text = temp_ct.ToString("F0");
        }
        else if (start_textChange_parry && temp_ct <= (float)totalParry_ct)
        {
            temp_ct += Time.deltaTime * TEXTSPEED;
            if(CheckText(temp_ct, (float)totalParry_ct))
            {
                start_textChange_parry = false;
                totalParry_txt.color = new Color32(248, 131, 50, 255);
            }
            totalParry_txt.text = temp_ct.ToString("F0");
        }
        else if (start_textChange_super && temp_ct <= (float)totalSuper_ct)
        {
            temp_ct += Time.deltaTime * TEXTSPEED;
            if(CheckText(temp_ct, (float)totalSuper_ct))
            {
                start_textChange_super = false;
                totalSuper_txt.color = new Color32(248, 131, 50, 255);
            }
            totalSuper_txt.text = temp_ct.ToString("F0");
        }

        if(data_isReady && !isExit)
        {
            if (battleSystem.Press_Key("LeftPunch"))
            {
                CancelInvoke();
                inputCount += 1;
                if (inputCount == 1){

                    start_textChange_counter = false;
                    start_textChange_parry = false;
                    start_textChange_super = false;

                    totalCounter_txt.text = totalCounter_ct.ToString("F0");
                    totalParry_txt.text = totalParry_ct.ToString("F0");
                    totalSuper_txt.text = totalSuper_ct.ToString("F0");

                    totalCounter_txt.color = new Color32(248, 131, 50, 255);
                    totalParry_txt.color = new Color32(248, 131, 50, 255);
                    totalSuper_txt.color = new Color32(248, 131, 50, 255);

                    resultCard_ExpBar.DisplayExp();
                }
                else if (inputCount >= 2 && resultCard_ExpBar.DisplayExp_isOver()){
                    CallText();
                }
            }
        }
    }
    public void ResultCard_Initialize(int counter_ct, int parry_ct, int super_ct, BattleSystem battle_system, EnemyBase enemy_base)
    {
        battle_system.OnEnemyDefeat?.Invoke(enemy_base);

        totalCounter_ct = counter_ct;
        totalParry_ct = parry_ct;
        totalSuper_ct = super_ct;

        battleSystem = battle_system;
        enemyBase = enemy_base;
        
        // Extra Reward Depending on Battle Result
        totalExp = enemyBase.BattleExp + Mathf.FloorToInt(enemyBase.BattleExp * 0.02f * counter_ct) + Mathf.FloorToInt(enemyBase.BattleExp * 0.05f * parry_ct) + Mathf.FloorToInt(enemyBase.BattleExp * 0.08f * super_ct);
        totalGold = enemyBase.BattleCoin + Mathf.FloorToInt(enemyBase.BattleCoin * 0.05f * counter_ct) + Mathf.FloorToInt(enemyBase.BattleCoin * 0.09f * parry_ct) + Mathf.FloorToInt(enemyBase.BattleCoin * 0.2f * super_ct);
        
        SetLanguage();

        string expMessage = UIControl.instance.uiTextDict["BattleWon_ExpMessage"];
        expMessage = expMessage.Replace("?", totalExp.ToString());

        string moneyMessage = UIControl.instance.uiTextDict["BattleWon_MoneyMessage"];
        moneyMessage = moneyMessage.Replace("?", totalGold.ToString());

        resultTexts.Add(expMessage);
        resultTexts.Add(moneyMessage);
        DropItems();

        expBundle = battleSystem.GetExp();
        resultCard_ExpBar = transform.GetChild(0).gameObject.GetComponent<ResultCard_ExpBar>();
        resultCard_ExpBar.InitializeExpBar(expBundle.player_level, expBundle.player_max_exp, expBundle.player_current_exp, totalExp);

        ResultCard_GetScore();
    }

    private void ResultCard_GetScore()
    {
        totalCounter_txt = transform.GetChild(1).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        totalParry_txt = transform.GetChild(2).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        totalSuper_txt = transform.GetChild(3).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

        Invoke("TextChange_counter", 2.5f);
        Invoke("TextChange_parry", 3.5f);
        Invoke("TextChange_super", 4.5f);
    }

    private void TextChange_counter()
    {
        data_isReady = true; // Enables Player to Exit any moment.

        temp_ct = 0;
        start_textChange_counter = true;
    }
    private void TextChange_parry()
    {
        temp_ct = 0;
        start_textChange_parry = true;
    }

    private void TextChange_super()
    {
        temp_ct = 0;
        start_textChange_super = true;
    }

    private bool CheckText(float ct, float max_ct)
    {
        if (ct > max_ct)
        {
            return true;
        }
        return false;
    }
    
    private void CallText()
    {
        if(!typeEffect.isPrinting){
            textIndex += 1;
        }
        
        if (textIndex == resultTexts.Count)
            ResultCard_Exit();
        
        else {
            typeEffect.SetMessage(resultTexts[textIndex]);
        }
    }

    private void DropItems()
    {
        for (int i=0; i<enemyBase.ItemReward.Count; i++)
        {
            float dropChance = enemyBase.ItemReward[i].DropChance * 0.01f;
            if(Random.Range(0f, 1f) <= dropChance)
            {
                string dropItemTag = enemyBase.ItemReward[i].RewardItem.ItemName;
                dropItemTag = UIControl.instance.uiTextDict[dropItemTag];

                string itemMessage = UIControl.instance.uiTextDict["BattleWon_ItemMessage"];
                itemMessage = itemMessage.Replace("?", dropItemTag);
                //string itemMessage = string.Format("Obtained {0}!", enemyBase.ItemReward[i].RewardItem.ItemName);
                resultTexts.Add(itemMessage);
                droppedItems.Add(enemyBase.ItemReward[i]);
            }
        }
    }

    private void ResultCard_Exit()
    {
        isExit = true;

        CancelInvoke();
        battleSystem.ExitBattle();
        battleSystem.UpdatePlayerStatus(updateLevel, max_exp, current_exp, totalExp, totalGold, droppedItems);
    }
    
}
