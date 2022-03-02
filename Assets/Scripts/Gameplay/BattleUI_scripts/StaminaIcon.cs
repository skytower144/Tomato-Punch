using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StaminaIcon : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI staminaText;

    void Update()
    {
        if(tomatocontrol.currentStamina == 0)
            staminaText.color = new Color32(255, 100, 111,255);
        else
            staminaText.color = new Color32(255, 255, 160,255);
    }

    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
    }
    public void SetStamina(int stamina)
    {
        slider.value = stamina;
        staminaText.text = tomatocontrol.currentStamina.ToString();
    }
}
