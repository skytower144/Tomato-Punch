using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlScroll : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Animator ToggleAnim;
    [SerializeField] private Image key_or_pad, roam_or_battle;
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private TextMeshProUGUI mode_text;
    [SerializeField] private List<TextMeshProUGUI> controlTextList;
    private int menuNumber;
    private float current_scroll_y;
    private bool isKeyBoard = true;
    private bool isModeRoam = true;
    void OnEnable()
    {
        UncolorMenu();
        menuNumber = -2;
        ColorMenu();

        current_scroll_y = 0;
        contentTransform.localPosition = new Vector3(contentTransform.localPosition.x, 0);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            UncolorMenu();
            IncreaseNumber();
            ColorMenu();
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            UncolorMenu();
            DecreaseNumber();
            ColorMenu();
        }
        else if(Input.GetKeyDown(KeyCode.O))
        {
            ControlInteractMenu();
        }
    }
    private void IncreaseNumber()
    {
        menuNumber += 1;
        menuNumber = Mathf.Clamp(menuNumber, -2, 9);
        MoveScroll("+");    
    }
    private void DecreaseNumber()
    {
        menuNumber -= 1;
        menuNumber = Mathf.Clamp(menuNumber, -2, 9);
        MoveScroll("-");
    }
    private void ColorMenu()
    {
        if (menuNumber == -2)
        {
            key_or_pad.color = new Color32(97, 125, 97, 255);
        }
        else if (menuNumber == -1)
        {
            roam_or_battle.color = new Color32(112, 255, 158, 194);
        }
        else if (menuNumber >= 0)
        {
            controlTextList[menuNumber].color = new Color32(97, 125, 97, 255);
        }
    }
    private void UncolorMenu()
    {
        if (menuNumber == -2)
        {
            key_or_pad.color = new Color32(96, 87, 84, 255);
        }
        else if (menuNumber == -1)
        {
            roam_or_battle.color = new Color32(255, 255, 255, 255);
        }
        else if (menuNumber >= 0)
        {
            controlTextList[menuNumber].color = new Color32(112, 82, 75, 255);
        }
    }
    private void MoveScroll(string direction)
    {
        if(direction == "+")
        {
            if(menuNumber >= 4)
                current_scroll_y += 105f;
        }
        else if(direction == "-")
        {
            if(menuNumber >= 0 && menuNumber <= 5)
                current_scroll_y -= 105f;
        }
        current_scroll_y = Mathf.Clamp(current_scroll_y, 0, 599.9f);

        contentTransform.localPosition = new Vector3(contentTransform.localPosition.x, current_scroll_y);
    }

    public void ControlMouseSelect(int index)
    {
        current_scroll_y = contentTransform.localPosition.y;

        UncolorMenu();
        menuNumber = index;
        ColorMenu();
    }

    public void ControlInteractMenu()
    {
        if(menuNumber == -2)
        {
            if(isKeyBoard){
                isKeyBoard = false;
                key_or_pad.sprite = icons[1];
            }
            else {
                isKeyBoard = true;
                key_or_pad.sprite = icons[0];
            }
        }
        else if(menuNumber == -1)
        {
            if(isModeRoam)
            {
                isModeRoam = false;
                ToggleAnim.Play("ui_controlToggle_battle");
            }
            else {
                isModeRoam = true;
                ToggleAnim.Play("ui_controlToggle_roam");
            }
            SwitchModeText();
        }
    }

    private void SwitchModeText()
    {
        if(isModeRoam){
            mode_text.text = "FREEROAM";

            controlTextList[0].text = "* Move Up";
            controlTextList[1].text = "* Move Down";
            controlTextList[2].text = "* Move Left";
            controlTextList[3].text = "* Move Right";
            controlTextList[4].text = "* Interact";
            controlTextList[5].text = "* Cancel";
            controlTextList[6].text = "* Left Page";
            controlTextList[7].text = "* Right Page";
            controlTextList[8].text = "* Status";
            controlTextList[9].text = "* Pause Menu";
        }
        else {
            mode_text.text = "BATTLE";

            controlTextList[0].text = "* Jump";
            controlTextList[1].text = "* Guard";
            controlTextList[2].text = "* Evade Left";
            controlTextList[3].text = "* Evade Right";
            controlTextList[4].text = "* Left Punch";
            controlTextList[5].text = "* Right Punch";
            controlTextList[6].text = "* 1st Equip";
            controlTextList[7].text = "* 2nd Equip";
            controlTextList[8].text = "* Super";
            controlTextList[9].text = "-";
        }
    }
}
