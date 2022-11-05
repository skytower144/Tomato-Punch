using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatedText : MonoBehaviour
{
    public BattleSystem battleSystem;
    private TypeEffect typeEffect;
    private GameObject battle_text, text_box, cursor;
    [SerializeField] private GameObject fadeOut;
    private List<string> textList = new List<string>();
    private int textIndex, giveUpCost;
    private bool startText = false;
    [SerializeField] private List<TextAndFont> textDataList = new List<TextAndFont>();
    void Start()
    {
        battle_text = transform.GetChild(0).gameObject;
        text_box = battle_text.transform.GetChild(0).gameObject;
        cursor = text_box.transform.GetChild(0).gameObject;

        typeEffect = text_box.GetComponent<TypeEffect>();
        textIndex = -1;

        giveUpCost = battleSystem.GetEnemyBase().BattleCoin;
        
        string victoryAnimation = battleSystem.GetEnemyBase().Victory;
        battleSystem.GetEnemyAnim().Play(victoryAnimation, -1, 0f);

        Invoke("SpawnTextBox", 1.5f);
    }

    void Update()
    {
        if (startText){
            if (battleSystem.Press_Key("LeftPunch")){
                CallText();
            }
        }
    }

    void OnDisable()
    {
        Destroy(gameObject);
    }
    private void SpawnTextBox()
    {
        battle_text.SetActive(true);
        InitializeText();
        
        CallText();
        startText = true;
    }

    private void InitializeText()
    {
        if (UIControl.currentLangMode != "eng")
            UIControl.instance.SwitchLanguage(textDataList, UIControl.currentLangMode);

        string moneyMessage = UIControl.instance.uiTextDict["BattleLost_MoneyMessage"];
        moneyMessage = moneyMessage.Replace("?", giveUpCost.ToString());

        string ExitMessage = UIControl.instance.uiTextDict["BattleLost_ExitMessage"];

        textList.Add(moneyMessage);
        textList.Add(ExitMessage);
    }

    private void CallText()
    {
        if(!typeEffect.isPrinting){
            textIndex += 1;
        }
        
        if (textIndex == textList.Count-1){
            startText = false;

            SlowText();
            typeEffect.SetMessage(textList[textIndex]);

            Invoke("ScreenFadeOut", 1.1f);
            Invoke("ExitBattle", 2f);
        }
        
        else {
            typeEffect.SetMessage(textList[textIndex]);
        }
    }
    private void ScreenFadeOut()
    {
        Instantiate(fadeOut, transform.parent);
    }

    private void SlowText()
    {
        text_box.GetComponent<TypeEffect>().CharPerSeconds = 13;
    }
    private void ExitBattle()
    {
        typeEffect.CancelInvoke();
        battleSystem.UpdatePlayerMoney(giveUpCost);
        battleSystem.ExitBattle();
    }
}
