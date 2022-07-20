using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlScroll : MonoBehaviour, CanToggleIcon
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private RebindKey rebindKey;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Animator ToggleAnim;
    [SerializeField] private Image key_or_pad, roam_or_battle;

    [Header("ROAM or BATTLE")]
    [SerializeField] private GameObject display_roam; 
    [SerializeField] private GameObject display_battle;

    [SerializeField] private GameObject DisplayKeyboard_roam, DisplayKeyboard_battle, DisplayGamepad_roam, DisplayGamepad_battle;

    [SerializeField] private List<Sprite> icons;
    public List<Sprite> gamePadIcons;

    [SerializeField] private TextMeshProUGUI mode_text;
    [Header("ACTION NAME")]
    [SerializeField] private List<TextMeshProUGUI> roam_actionText;
    [SerializeField] private List<TextMeshProUGUI> battle_actionText;

    [Header("KEYBOARD")]
    [SerializeField] private List<TextMeshProUGUI> roam_bindingDisplayText_key;
    [SerializeField] private List<TextMeshProUGUI> battle_bindingDisplayText_key;

    [Header("GAMEPAD")]
    [SerializeField] private List<Image> roam_bindingDisplayText_pad;
    [SerializeField] private List<Image> battle_bindingDisplayText_pad;
    
    private int menuNumber;
    public int InputMenuNumber => menuNumber;

    private int showingNumber_top, showingNumber_bot;
    private float current_scroll_y;

    public bool isKeyBoard = true;
    public bool isModeRoam = true;

    void OnEnable()
    {
        UncolorMenu();
        menuNumber = -1;
        ColorMenu();

        showingNumber_top = 0;
        showingNumber_bot = 3;

        current_scroll_y = 0;
        contentTransform.localPosition = new Vector3(contentTransform.localPosition.x, 0);
    }
    void Update()
    {
        RefreshTopBot();

        if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
        {
           gameManager.DetectHolding(UINavigate);
        }
        else if (gameManager.WasHolding)
        {
            gameManager.holdStartTime = float.MaxValue;
        }
        else if(playerMovement.Press_Key("Interact"))
        {
            // Prevent Keyboard input rebind when viewing Controller input list.
            if(!(playerMovement.CheckKeyboardControl() && !isKeyBoard))
                ControlInteractMenu();
        }
    }
    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();

        UncolorMenu();
        if (direction == "UP")
            DecreaseNumber();
        
        else if (direction == "DOWN")
            IncreaseNumber();
        ColorMenu();
    }
    private void IncreaseNumber()
    {
        menuNumber += 1;
        menuNumber = Mathf.Clamp(menuNumber, -1, totalMenuNumber);

        if(menuNumber > showingNumber_bot)
            MoveScroll("+");  
    }
    private void DecreaseNumber()
    {
        menuNumber -= 1;
        menuNumber = Mathf.Clamp(menuNumber, -1, totalMenuNumber);

        if(menuNumber < showingNumber_top)
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
            if(isKeyBoard)
                bindingDisplayText_key[menuNumber].color = new Color32(97, 125, 97, 255);
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
            if(isKeyBoard)
                bindingDisplayText_key[menuNumber].color = new Color32(112, 82, 75, 255);
        }
    }
    private void MoveScroll(string direction)
    {
        if(direction == "+")
        {
            current_scroll_y += 100f;

            showingNumber_top += 1;
            showingNumber_bot += 1;
            showingNumber_top = Mathf.Clamp(showingNumber_top, 0, totalMenuNumber-4+1);
            showingNumber_bot = Mathf.Clamp(showingNumber_bot, 3, totalMenuNumber);
        }
        else if(direction == "-")
        {
            current_scroll_y -= 100f;

            showingNumber_top -= 1;
            showingNumber_bot -= 1;
            showingNumber_top = Mathf.Clamp(showingNumber_top, 0, totalMenuNumber-4+1);
            showingNumber_bot = Mathf.Clamp(showingNumber_bot, 3, totalMenuNumber);
        }

        // Must change clamp max value according to content size.
        current_scroll_y = Mathf.Clamp(current_scroll_y, 0, 499.9f); 

        contentTransform.localPosition = new Vector3(contentTransform.localPosition.x, current_scroll_y);
    }
    private void RefreshTopBot()
    {
        for (int i = 1; i <= totalMenuNumber - 3; i++)
        {
            if (contentTransform.localPosition.y < 100 * i - 0.2f)
            {
                showingNumber_top = (i-1);
                showingNumber_bot = showingNumber_top + 3;
                return;
            }
        }
        showingNumber_top = totalMenuNumber - 3;
        showingNumber_bot = showingNumber_top + 3;
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
    public void ToggleIcon()
    {
        if(isKeyBoard)
        {
            DisplayKeyboard_roam.SetActive(true);
            DisplayGamepad_roam.SetActive(false);
            
            DisplayKeyboard_battle.SetActive(true);
            DisplayGamepad_battle.SetActive(false);
        }
        else
        {
            DisplayKeyboard_roam.SetActive(false);
            DisplayGamepad_roam.SetActive(true);
            
            DisplayKeyboard_battle.SetActive(false);
            DisplayGamepad_battle.SetActive(true);
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
            Invoke("DelayRebind", 0.15f);
        }
    }
    private void DelayRebind()
    {
        rebindKey.StartRebinding();
    }
    public List<TextMeshProUGUI> bindingDisplayText_key
    {
        get { return isModeRoam ? roam_bindingDisplayText_key : battle_bindingDisplayText_key;}
    }
    public List<Image> bindingDisplayText_pad
    {
        get { return isModeRoam ? roam_bindingDisplayText_pad : battle_bindingDisplayText_pad;}
    }

    private List<TextMeshProUGUI> controlTextList
    {
        get { return isModeRoam ? roam_actionText : battle_actionText;}
    }
    private int totalMenuNumber
    {
        get { return isModeRoam ? roam_bindingDisplayText_key.Count - 1: battle_bindingDisplayText_key.Count - 1;}
    }

    private void SwitchModeText()
    {
        display_roam.SetActive(!display_roam.activeSelf);
        display_battle.SetActive(!display_battle.activeSelf);

        if(display_roam.activeSelf)
            mode_text.text = "FREEROAM";
        else
            mode_text.text = "BATTLE";
    }
}
