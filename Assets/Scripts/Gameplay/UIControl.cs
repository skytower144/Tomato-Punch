using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

[System.Serializable]
public class StringFontdata : SerializableDictionary<string, TMP_FontAsset>{}

public class UIControl : MonoBehaviour
{
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private ControlScroll controlScroll;
    public ShopSystem ui_shop;
    [SerializeField] private List<GameObject> ui_bundle;
    
    [Header("LOCALIZATION")]
    public static LanguageType currentLang = LanguageType.ENGLISH;
    public StringFontdata fontDict = new StringFontdata(); // dictionary of font types

    public static UIControl instance { get; private set; }
    public static int TotalLanguages => System.Enum.GetValues(typeof(LanguageType)).Length;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one UI Control in the scene.");
            return;
        }
        instance = this;

        TextDB.Initialize();
        resolutionMenu.LoadLanguageSetting();
    }

    public void UI_Update(bool state)
    {
        // Check if the for loop update process is unnecessary.
        if (controlScroll.isKeyBoard == state){
            return;
        }
        
        controlScroll.isKeyBoard = state; // update the state of current control scheme
        
        for (int i = 0; i < ui_bundle.Count; i++)
        {
            ui_bundle[i].GetComponent<CanToggleIcon>()?.ToggleIcon();
        }
    }

    public void UI_GamepadSwitch()
    {
        for (int i = 0; i < ui_bundle.Count; i++)
        {
            ui_bundle[i].GetComponent<CanToggleIcon>()?.ToggleGamepadIcon();
        }
        controlScroll.ToggleGamepadIcon();
    }

    public void UI_Update_Text(string changedText, string oldPath,string newPath)
    {
        for (int i = 0; i < ui_bundle.Count; i++)
        {
            ui_bundle[i].GetComponent<RebindChangeUI>()?.RebindUI_text(changedText, oldPath, newPath);
        }
    }

    public void UI_Update_Sprite(Sprite changedSprite, string oldPath, string newPath)
    {
        for (int i = 0; i < ui_bundle.Count; i++)
        {
            ui_bundle[i].GetComponent<RebindChangeUI>()?.RebindUI_sprite(changedSprite, oldPath, newPath);
        }
    }
    public void SetFontData(TextMeshProUGUI targetText, string uiTag)
    {
        string fontData = TextDB.Translate(uiTag, TranslationType.FONT);
        string[] fontInfos = fontData.Split('_');

        targetText.font = fontDict[fontInfos[0]];
        targetText.fontSize = int.Parse(fontInfos[1]);
        targetText.characterSpacing = float.Parse(fontInfos[2]);
        targetText.wordSpacing = float.Parse(fontInfos[3]);
        targetText.lineSpacing = float.Parse(fontInfos[4]);
    }
    public static void SetLanguage(int amount)
    {
        currentLang += amount;

        if ((int)currentLang >= TotalLanguages)
            currentLang = LanguageType.ENGLISH;
        
        else if ((int)currentLang < 0)
            currentLang = (LanguageType)(TotalLanguages - 1);
    }
}

// Column order MUST match with Dialogue Sheet.csv file table.
public enum LanguageType { ENGLISH, KOREAN, JAPANESE, CHINESE }
public enum TranslationType { DIALOGUE, UI, FONT }