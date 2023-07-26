using TMPro;
using System;
using UnityEngine;

[System.Serializable]
public class StringTextGUI : SerializableDictionary<string, TextMeshProUGUI>{} 

public class LocalizeUI : MonoBehaviour
{
    static public Action OnLocalizeUI;
    [SerializeField] private StringTextGUI uiDict = new StringTextGUI();

    void OnEnable()
    {
        OnLocalizeUI -= LocalizeText;
        OnLocalizeUI += LocalizeText;
        LocalizeText();
    }

    void OnDisable()
    {
        OnLocalizeUI -= LocalizeText;
    }

    private void LocalizeText()
    {
        if (!UIControl.instance) return;
        
        foreach (var bundle in uiDict) {
            bundle.Value.text = UIControl.instance.uiTextDict[bundle.Key];
            UIControl.instance.SetFontData(bundle.Value, bundle.Key);
        }
    }
}
