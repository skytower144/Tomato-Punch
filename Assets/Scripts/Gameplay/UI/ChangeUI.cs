using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeUI : MonoBehaviour, CanToggleIcon, RebindChangeUI
{
    [SerializeField] private string keyboard_linked_path, gamepad_linked_path;
    [SerializeField] private GameObject keyboard_display, gamepad_display;
    [SerializeField] private TextMeshProUGUI keyboard_rebind_text;
    [SerializeField] private Image gamepad_rebind_image;

    public void ToggleIcon()
    {
        keyboard_display.SetActive(!keyboard_display.activeSelf);
        gamepad_display.SetActive(!gamepad_display.activeSelf);
    }

    public void ToggleGamepadIcon()
    {
        gamepad_rebind_image.sprite = GameManager.gm_instance.rebind_key.LinkSprite(-1, gamepad_linked_path);
    }

    public void RebindUI_text(string changed_text, string old_path, string new_path)
    {
        if (old_path == keyboard_linked_path){
            keyboard_linked_path = new_path;
            keyboard_rebind_text.text = changed_text;
        }
        
        return;
    }
    public void RebindUI_sprite(Sprite changed_sprite, string old_path, string new_path)
    {
        if (old_path == gamepad_linked_path){
            gamepad_linked_path = new_path;
            gamepad_rebind_image.sprite = changed_sprite;
        }

        return;
    }
}
