using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

[System.Serializable]
public class StringFontasset : SerializableDictionary<string, FontData>{} // xxxxxx

[System.Serializable]
public class StringFontdata : SerializableDictionary<string, TMP_FontAsset>{}

[System.Serializable]
public class StringLocalizationData : SerializableDictionary<string, LocalizationData>{}
public class UIControl : MonoBehaviour
{
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private List<GameObject> ui_bundle;
    
    [Header("LOCALIZATION")]
    public static string currentLangMode = "eng";
    public Dictionary<string, string> uiTextDict = new Dictionary<string, string>(); // uitext - translation
    private Dictionary<string, string[]> uiFontdataDict = new Dictionary<string, string[]>(); // uitext - fontdata
    [SerializeField] private StringLocalizationData inkLangDict = new StringLocalizationData();
    public StringFontdata fontDict = new StringFontdata(); // dictionary of font types
    public List<TextAndFont> textDataList = new List<TextAndFont>(); // text - fontdata // xxxxxxx
    private Story UIData;

    public static UIControl instance { get; private set; }

    void Awake()
    {
        instance = this;
        resolutionMenu.LoadLanguageSetting();
        InitializeInkLangDict(currentLangMode);
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

    public void InitializeInkLangDict(string language = "eng")
    {
        currentLangMode = language;
        uiTextDict.Clear();
        
        TextAsset inkJSON = inkLangDict[language].translationData;
        var UIJsonData = new Story(inkJSON.text);
        
        while (UIJsonData.canContinue)
        {
            string dataLine = UIJsonData.Continue();
            string[] splitDataLine = dataLine.Split(':');

            if (splitDataLine.Length != 2)
                continue;

            string uiTextType = splitDataLine[0].Trim();
            string uiText = splitDataLine[1].Trim();

            uiTextDict[uiTextType] = uiText;
        }

        uiFontdataDict.Clear();
        inkJSON = inkLangDict[language].fontData;
        UIJsonData = new Story(inkJSON.text);

        while (UIJsonData.canContinue)
        {
            string dataLine = UIJsonData.Continue();
            string[] splitDataLine = dataLine.Split(':');

            if (splitDataLine.Length != 2)
                continue;

            string uiTextType = splitDataLine[0].Trim();
            string uiFontdata = splitDataLine[1].Trim();
            string[] dataList = uiFontdata.Split('_');

            if (dataList.Length != 5)
                continue;

            uiFontdataDict[uiTextType] = dataList;
        }
    }

    public void SwitchLanguage(List<TextAndFont> textDataList, string language)
    {
        foreach(TextAndFont textData in textDataList)
        {
            TextMeshProUGUI targetText = textData.target_text;
            
            targetText.text = uiTextDict[targetText.name];

            if (textData.fontDict[language].font_type == null)
            {
                FontData defaultSetting = textDataList[0].fontDict[language];
                targetText.font = defaultSetting.font_type;
                targetText.fontSize = defaultSetting.font_size;
                targetText.characterSpacing = defaultSetting.character_space;
                targetText.wordSpacing = defaultSetting.word_space;
                targetText.lineSpacing = defaultSetting.line_space;
            }
            else
            {
                targetText.font = textData.fontDict[language].font_type;
                targetText.fontSize = textData.fontDict[language].font_size;
                targetText.characterSpacing = textData.fontDict[language].character_space;
                targetText.wordSpacing = textData.fontDict[language].word_space;
                targetText.lineSpacing = textData.fontDict[language].line_space;
            }
        }
    }

    public void SetFontData(TextMeshProUGUI targetText, string uiTag)
    {
        targetText.font = fontDict[uiFontdataDict[uiTag][0]];
        targetText.fontSize = int.Parse(uiFontdataDict[uiTag][1]);
        targetText.characterSpacing = float.Parse(uiFontdataDict[uiTag][2]);
        targetText.wordSpacing = float.Parse(uiFontdataDict[uiTag][3]);
        targetText.lineSpacing = float.Parse(uiFontdataDict[uiTag][4]);
    }
}
[System.Serializable]
public class LocalizationData
{
    public TextAsset translationData;
    public TextAsset fontData;
}

[System.Serializable]
public class FontData
{
    public TMP_FontAsset font_type;
    public int font_size;
    public float character_space;
    public float word_space;
    public float line_space;
}

[System.Serializable]
public class TextAndFont
{
    public TextMeshProUGUI target_text;
    public StringFontasset fontDict;
}

[System.Serializable]
public class LanguageSetting
{
    public TextMeshProUGUI target_text;
    public string display_name;
    public string language_mode;
    public FontData font_detail;
}
