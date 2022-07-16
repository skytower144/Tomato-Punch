using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlScroll : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private RebindKey rebindKey;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Animator ToggleAnim;
    [SerializeField] private Image key_or_pad, roam_or_battle;
    [SerializeField] private GameObject DisplayKeyboard, DisplayGamepad;
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private TextMeshProUGUI mode_text;
    [SerializeField] private List<TextMeshProUGUI> controlTextList;
    [SerializeField] private List<TextMeshProUGUI> bindingDisplayText_key;
    [SerializeField] private List<TextMeshProUGUI> bindingDisplayText_pad;
    
    private int menuNumber, totalMenuNumber;
    public int InputMenuNumber => menuNumber;
    private float current_scroll_y;
    public bool isKeyBoard = true;
    private bool isModeRoam = true;
    void Start()
    {
        totalMenuNumber = controlTextList.Count - 1;
    }
    void OnEnable()
    {
        UncolorMenu();
        menuNumber = -1;
        ColorMenu();

        current_scroll_y = 0;
        contentTransform.localPosition = new Vector3(contentTransform.localPosition.x, 0);
    }
    void Update()
    {
        if(playerMovement.Press_Direction("DOWN"))
        {
            UncolorMenu();
            IncreaseNumber();
            ColorMenu();
        }
        else if(playerMovement.Press_Direction("UP"))
        {
            UncolorMenu();
            DecreaseNumber();
            ColorMenu();
        }
        else if(playerMovement.Press_Key("Interact"))
        {
            ControlInteractMenu();
        }
    }
    private void IncreaseNumber()
    {
        menuNumber += 1;
        menuNumber = Mathf.Clamp(menuNumber, -1, totalMenuNumber);
        MoveScroll("+");    
    }
    private void DecreaseNumber()
    {
        menuNumber -= 1;
        menuNumber = Mathf.Clamp(menuNumber, -1, totalMenuNumber);
        MoveScroll("-");
    }
    private void ColorMenu()
    {
        if (menuNumber == -1)
        {
            roam_or_battle.color = new Color32(112, 255, 158, 194);
        }
        else if (menuNumber >= 0)
        {
            controlTextList[menuNumber].color = new Color32(97, 125, 97, 255);
            bindingDisplayText_key[menuNumber].color = new Color32(97, 125, 97, 255);
            bindingDisplayText_pad[menuNumber].color = new Color32(97, 125, 97, 255);
        }
    }
    private void UncolorMenu()
    {
        if (menuNumber == -1)
        {
            roam_or_battle.color = new Color32(255, 255, 255, 255);
        }
        else if (menuNumber >= 0)
        {
            controlTextList[menuNumber].color = new Color32(112, 82, 75, 255);
            bindingDisplayText_key[menuNumber].color = new Color32(112, 82, 75, 255);
            bindingDisplayText_pad[menuNumber].color = new Color32(112, 82, 75, 255);
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
        if(!rebindKey.isBinding){
            current_scroll_y = contentTransform.localPosition.y;

            UncolorMenu();
            menuNumber = index;
            ColorMenu();
        }
    }
    public void ChangeControlScheme(bool state)
    {
        isKeyBoard = state;
        if(isKeyBoard){
            key_or_pad.sprite = icons[0];
            DisplayKeyboard.SetActive(true);
            DisplayGamepad.SetActive(false);
        }
        else{
            key_or_pad.sprite = icons[1];
            DisplayKeyboard.SetActive(false);
            DisplayGamepad.SetActive(true);
        }
    }
    public void ControlInteractMenu()
    {
        if(menuNumber == -1)
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
        else if(menuNumber >= 0)
        {
            rebindKey.StartRebinding();
        }
    }
    public List<TextMeshProUGUI> bindingDisplayText
    {
        get { return isKeyBoard ? bindingDisplayText_key : bindingDisplayText_pad; }
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
        }
    }
}
