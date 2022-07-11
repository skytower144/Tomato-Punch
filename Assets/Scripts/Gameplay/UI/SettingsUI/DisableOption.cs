using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOption : MonoBehaviour
{
    [SerializeField] OptionScript optionScript;

    private void OffOption()
    {
        optionScript.TurnoffOption();
    }

    private void AllowNavigation()
    {
        optionScript.EnableNavigation();
    }
}
