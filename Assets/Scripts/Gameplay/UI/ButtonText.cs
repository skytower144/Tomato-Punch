using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;

    public void buttonHighlightText()
    {
        textField.color = new Color(197/255f , 255/255f, 234/255f);
    }
    public void buttonNormalizeText()
    {
        textField.color = new Color(229/255f , 168/255f, 161/255f);
    }

}
