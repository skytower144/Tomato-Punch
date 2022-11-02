using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

[System.Serializable]
public class StringFontasset : SerializableDictionary<string, TMP_FontAsset>{}

[System.Serializable]
public class StringTextasset : SerializableDictionary<string, TextAsset>{}
public class UIControl : MonoBehaviour
{
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private List<GameObject> ui_bundle;
    
    static public Dictionary<string, string> uiTextDict = new Dictionary<string, string>();
    public static string currentLangMode = "eng";
    [SerializeField] private StringTextasset inkLangDict = new StringTextasset();
    private Story UIData;

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("changing to korean");
            SetUILanguage("kor");
        }
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

    private void SetUILanguage(string langType)
    {
        currentLangMode = langType;
        TextAsset inkJSON = inkLangDict[langType];
        var UIJsonData = new Story(inkJSON.text);

        uiTextDict.Clear();
        
        while (UIJsonData.canContinue)
        {
            string dataLine = UIJsonData.Continue();
            string[] splitDataLine = dataLine.Split(':');

            string uiTextType = splitDataLine[0].Trim();
            string uiText = splitDataLine[1].Trim();

            uiTextDict[uiTextType] = uiText;
        }
    }
}

// [System.Serializable]
// public class FontData
// {
//     public TMP_FontAsset font_type;
//     public int font_size;
// }
