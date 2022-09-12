using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TomatoLevel : MonoBehaviour
{
    public int playerLevel;
    public Slider expFill;
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable()
    {
        levelText.text = string.Format("Lv {0}", playerLevel);
    }
    
}
