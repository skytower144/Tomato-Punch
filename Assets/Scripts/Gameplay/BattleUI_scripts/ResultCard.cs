using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCard : MonoBehaviour
{
    public BattleSystem battleSystem;
    public EnemyBase enemyBase;
    private TypeEffect typeEffect;
    private ResultCard_ExpBar resultCard_ExpBar;
    private int totalCounter_ct, totalParry_ct, totalSuper_ct, inputCount, textIndex;
    private float temp_ct;
    private float TEXTSPEED = 14f;
    [SerializeField] private GameObject battle_end_circle;
    [SerializeField] private TextMeshProUGUI totalCounter_txt, totalParry_txt, totalSuper_txt;
    private string[] resultTexts;
    private ExpBundle expBundle;
    private bool start_textChange_counter, start_textChange_parry, start_textChange_super, data_isReady, isExit;

    void Start()
    {
        typeEffect = transform.GetChild(4).gameObject.GetComponent<TypeEffect>();
    }

    void OnEnable()
    {
        inputCount = 0;
        textIndex = -1;
        data_isReady = false;
        isExit = false;
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
            if(Input.GetKeyDown(KeyCode.O))
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

            if(Input.GetKeyDown(KeyCode.P))
            {
                ResultCard_Exit();
            }
        }
    }
    public void ResultCard_Initialize(int counter_ct, int parry_ct, int super_ct, BattleSystem battle_system, EnemyBase enemy_base)
    {
        totalCounter_ct = counter_ct;
        totalParry_ct = parry_ct;
        totalSuper_ct = super_ct;

        battleSystem = battle_system;
        enemyBase = enemy_base;

        string expMessage = string.Format("Gained Total {0} Exp.", enemyBase.BattleExp);
        string moneyMessage = string.Format("Obtained {0} Coins.", enemyBase.BattleCoin);
        resultTexts = new string[] {expMessage, moneyMessage};

        expBundle = battleSystem.GetExp();
        resultCard_ExpBar = transform.GetChild(0).gameObject.GetComponent<ResultCard_ExpBar>();
        resultCard_ExpBar.InitializeExpBar(expBundle.player_level, expBundle.player_max_exp, expBundle.player_current_exp, enemyBase.BattleExp);

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
        
        if (textIndex == resultTexts.Length)
            ResultCard_Exit();
        
        else {
            typeEffect.SetMessage(resultTexts[textIndex]);
        }
    }

    private void ResultCard_Exit()
    {
        isExit = true;

        CancelInvoke();
        Destroy(Instantiate(battle_end_circle), 2f);
        battleSystem.ExitBattle();
        battleSystem.UpdatePlayerStatus(enemyBase.BattleExp, enemyBase.BattleCoin);
    }
    
}
