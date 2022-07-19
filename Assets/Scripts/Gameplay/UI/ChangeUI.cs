using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeUI : MonoBehaviour, CanToggleIcon
{
    [SerializeField] private GameObject keyboard_display, gamepad_display;
    public void ToggleIcon()
    {
        keyboard_display.SetActive(!keyboard_display.activeSelf);
        gamepad_display.SetActive(!gamepad_display.activeSelf);
    }
}
