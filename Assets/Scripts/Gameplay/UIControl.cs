using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

[System.Serializable]
public class StringFontasset : SerializableDictionary<string, FontData>{}

[System.Serializable]
public class StringTextasset : SerializableDictionary<string, TextAsset>{}
public class UIControl : MonoBehaviour
{
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private List<GameObject> ui_bundle;
    
    [Header("LOCALIZATION")]
    public static string currentLangMode = "eng"; 
    public Dictionary<string, string> uiTextDict = new Dictionary<string, string>();
    [SerializeField] private StringTextasset inkLangDict = new StringTextasset();
    public List<TextAndFont> textDataList = new List<TextAndFont>();
    private Story UIData;

    public static UIControl instance { get; private set; }

    void Awake()
    {
        instance = this;
        resolutionMenu.LoadLanguageSetting();
        InitializeInkLangDict(currentLangMode);
        TitleScreen.instance.SetUILanguage();
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
        
        TextAsset inkJSON = inkLangDict[language];
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
    }

    public void SwitchLanguage(List<TextAndFont> textDataList, string language)
    {
        foreach(TextAndFont textData in textDataList)
        {
            TextMeshProUGUI targetText = textData.target_text;
            
            targetText.text = uiTextDict[targetText.name];
            targetText.font = textData.fontDict[language].font_type;
            targetText.fontSize = textData.fontDict[language].font_size;
            targetText.characterSpacing = textData.fontDict[language].character_space;
            targetText.wordSpacing = textData.fontDict[language].word_space;
            targetText.lineSpacing = textData.fontDict[language].line_space;
        }
    }

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
