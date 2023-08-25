using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOption : MonoBehaviour
{
    [SerializeField] OptionScript optionScript;

    private void OffOption()
    {
        optionScript.TurnoffOption();
        
        if (TitleScreen.isTitleScreen)
            TitleScreen.busy_with_menu = false;
    }
    
    private void TurnOnTitle()
    {
        if (TitleScreen.isTitleScreen){
            TitleScreen.instance.gameObject.SetActive(true);
        }
    }

    private void AllowNavigation()
    {
        optionScript.EnableNavigation();
    }

    private void PushupFinish()
    {
        optionScript.optionDrawing.enabled = true;
    }
}
