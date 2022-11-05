using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlScroll : MonoBehaviour, CanToggleIcon
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private OptionScript optionScript;
    [SerializeField] private RebindKey rebindKey;
    [SerializeField] private ResetBindings resetBindings;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Animator ToggleAnim;
    [SerializeField] private Image key_or_pad, roam_or_battle, reset_arrow;

    [Header("PROMPT")]
    [SerializeField] private GameObject reset_prompt;
    [SerializeField] private Transform promptTransform;

    [Header("ROAM or BATTLE")]
    [SerializeField] private GameObject title_roam;
    [SerializeField] private GameObject title_battle;
    [SerializeField] private GameObject display_roam, display_battle;

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
    public bool isPrompt = false;

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
        if (!isPrompt)
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
    }
    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();

        UncolorMenu();

        if ((menuNumber == -1) && (direction == "LEFT")) // in reset button
            DecreaseNumber();
        
        else if ((menuNumber == -2) && (direction == "RIGHT")) // out reset button
            IncreaseNumber();

        else if ((menuNumber != -1) && direction == "UP")
            DecreaseNumber();
        
        else if ((menuNumber != -2) && direction == "DOWN")
            IncreaseNumber();
        
        ColorMenu();
    }
    private void IncreaseNumber()
    {
        menuNumber += 1;
        menuNumber = Mathf.Clamp(menuNumber, -2, totalMenuNumber);

        if(menuNumber > showingNumber_bot)
            MoveScroll("+");  
    }
    private void DecreaseNumber()
    {
        menuNumber -= 1;
        menuNumber = Mathf.Clamp(menuNumber, -2, totalMenuNumber);

        if(menuNumber < showingNumber_top)
            MoveScroll("-");
    }
    private void ColorMenu()
    {
        if (menuNumber == -2)
        {
            reset_arrow.color = new Color32(97, 125, 97, 255);
        }
        else if (menuNumber == -1)
        {
            roam_or_battle.color = new Color32(112, 255, 158, 194);
        }
        else if (menuNumber >= 0)
        {
            controlTextList[menuNumber].color = new Color32(97, 125, 97, 255);
            if(isKeyBoard)
                bindingDisplayText_key()[menuNumber].color = new Color32(97, 125, 97, 255);
        }
    }
    private void UncolorMenu()
    {
        if (menuNumber == -2)
        {
            reset_arrow.color = new Color32(106, 94, 91, 255);
        }
        if (menuNumber == -1)
        {
            roam_or_battle.color = new Color32(255, 255, 255, 255);
        }
        else if (menuNumber >= 0)
        {
            controlTextList[menuNumber].color = new Color32(112, 82, 75, 255);
            if(isKeyBoard)
                bindingDisplayText_key()[menuNumber].color = new Color32(112, 82, 75, 255);
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
            key_or_pad.sprite = icons[0];
            optionScript.OptionToggleDrawing("KEY");

            DisplayKeyboard_roam.SetActive(true);
            DisplayGamepad_roam.SetActive(false);
            
            DisplayKeyboard_battle.SetActive(true);
            DisplayGamepad_battle.SetActive(false);
        }
        else
        {
            key_or_pad.sprite = icons[1];
            optionScript.OptionToggleDrawing("PAD");

            DisplayKeyboard_roam.SetActive(false);
            DisplayGamepad_roam.SetActive(true);
            
            DisplayKeyboard_battle.SetActive(false);
            DisplayGamepad_battle.SetActive(true);
        }
    }
    public void ControlInteractMenu()
    {
        if (menuNumber == -2)
        {
            isPrompt = true;
            GameObject prompt = Instantiate(reset_prompt, promptTransform);
            prompt.GetComponent<BindingResetPrompt>().InitalizeBindPrompt(gameManager, playerMovement, resetBindings, gameObject.GetComponent<ControlScroll>());
        }

        else if (menuNumber == -1)
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
            StartCoroutine(DelayRebind(0.15f));
        }
    }

    IEnumerator DelayRebind(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        rebindKey.StartRebinding();
    }
    
    public List<TextMeshProUGUI> bindingDisplayText_key()
    {
        if (resetBindings.demand_displayList == "FREEROAM")
            return roam_bindingDisplayText_key;

        else if (resetBindings.demand_displayList == "BATTLE")
            return battle_bindingDisplayText_key;
        
        else if (isModeRoam)
            return roam_bindingDisplayText_key;
        
        else
            return battle_bindingDisplayText_key;
    }
    public List<Image> bindingDisplayText_pad()
    {
        if (resetBindings.demand_displayList == "FREEROAM")
            return roam_bindingDisplayText_pad;
        
        else if (resetBindings.demand_displayList == "BATTLE")
            return battle_bindingDisplayText_pad;
        
        else if (isModeRoam)
            return  roam_bindingDisplayText_pad;
        
        else
            return battle_bindingDisplayText_pad;
    }
    

    private List<TextMeshProUGUI> controlTextList
    {
        get { return isModeRoam ? roam_actionText : battle_actionText;}
    }
    public int totalMenuNumber
    {
        get { return isModeRoam ? roam_bindingDisplayText_key.Count - 1: battle_bindingDisplayText_key.Count - 1;}
    }

    public int GetMenuNumbers(string mode_name)
    {
        if (mode_name == "FREEROAM")
            return roam_actionText.Count;
        else if (mode_name == "BATTLE")
            return battle_actionText.Count;
        
        return 0;
    }

    private void SwitchModeText()
    {
        display_roam.SetActive(!display_roam.activeSelf);
        display_battle.SetActive(!display_battle.activeSelf);

        if(display_roam.activeSelf) {
            title_roam.SetActive(true);
            title_battle.SetActive(false);
        }
        else {
            title_roam.SetActive(false);
            title_battle.SetActive(true);
        }
    }
}
