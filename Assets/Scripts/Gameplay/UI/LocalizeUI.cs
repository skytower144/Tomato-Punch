using TMPro;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringTextGUI : SerializableDictionary<string, TextMeshProUGUI>{} 

public class LocalizeUI : MonoBehaviour
{
    [SerializeField] private StringTextGUI uiDict = new StringTextGUI();

    public void OnEnable()
    {
        foreach (var bundle in uiDict) {
            bundle.Value.text = UIControl.instance.uiTextDict[bundle.Key];
            UIControl.instance.SetFontData(bundle.Value, bundle.Key);
        }
    }
}
