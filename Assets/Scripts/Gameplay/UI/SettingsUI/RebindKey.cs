using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindKey : MonoBehaviour
{
    [SerializeField] UIControl uIControl;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private OptionScript optionScript;
    [SerializeField] private List<InputActionReference> actionList_roam;
    [SerializeField] private List<InputActionReference> actionList_battle;
    [SerializeField] private GameObject waitCover, bindFail;
    [SerializeField] private Transform listenTransform;
    [SerializeField] private TextMeshProUGUI bindFailText;
    private InputAction current_action = null;
    private int bindingIndex, sameIndex;
    private string cachePath;
    private string sameActionName;
    private bool isComposite = false;
    [System.NonSerialized] public bool isBinding = false;
    
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private string[] gamepadInputTags = {
        "<Gamepad>/start",          "<Gamepad>/select",             "<Gamepad>/dpad/up",        "<Gamepad>/dpad/down",
        "<Gamepad>/dpad/left",      "<Gamepad>/dpad/right",         "<Gamepad>/buttonEast",     "<Gamepad>/buttonNorth",
        "<Gamepad>/buttonWest",     "<Gamepad>/buttonSouth",        "<Gamepad>/leftTrigger",    "<Gamepad>/rightTrigger",
        "<Gamepad>/leftShoulder",   "<Gamepad>/rightShoulder",      "<Gamepad>/leftStickPress", "<Gamepad>/rightStickPress",
        "<Gamepad>/rightStick/left","<Gamepad>/rightStick/right",   "<Gamepad>/rightStick/down","<Gamepad>/rightStick/up",
        "<Gamepad>/leftStick/left", "<Gamepad>/leftStick/right",    "<Gamepad>/leftStick/down", "<Gamepad>/leftStick/up",
    };

    private Dictionary<string, string> keyboardShortTags = new Dictionary<string, string>(){
        {"<Keyboard>/backspace", "BACK"},   {"<Keyboard>/insert", "INSR"},      {"<Keyboard>/pageUp", "PGUP"},
        {"<Keyboard>/delete", "DEL"},       {"<Keyboard>/pageDown", "PGDN"},    {"<Keyboard>/capsLock", "CAPS"},
        {"<Keyboard>/enter", "ENTR"},       {"<Keyboard>/leftShift", "LSHF"},   {"<Keyboard>/rightShift", "RSHF"},
        {"<Keyboard>/leftCtrl", "LCTRL"},   {"<Keyboard>/leftMeta", "LSYS"},    {"<Keyboard>/leftAlt", "LALT"},
        {"<Keyboard>/space", "SPC"},        {"<Keyboard>/rightAlt", "RALT"},    {"<Keyboard>/contextMenu", "MENU"},
        {"<Keyboard>/rightCtrl", "RCTR"},   {"<Keyboard>/upArrow", "UP"},       {"<Keyboard>/downArrow", "DOWN"},
        {"<Keyboard>/leftArrow", "LEFT"},   {"<Keyboard>/rightArrow", "RGHT"},  {"<Keyboard>/numLock", "NMLK"},
        {"<Keyboard>/numpadDivide", "N/"},  {"<Keyboard>/numpadMultiply", "N*"},{"<Keyboard>/numpadMinus", "N-"},
        {"<Keyboard>/numpadPlus", "N+"},    {"<Keyboard>/numpadEnter", "NENT"}, {"<Keyboard>/numpadPeriod", "N."},
        {"<Keyboard>/numpad0", "N0"},       {"<Keyboard>/numpad1", "N1"},       {"<Keyboard>/numpad2", "N2"},
        {"<Keyboard>/numpad3", "N3"},       {"<Keyboard>/numpad4", "N4"},       {"<Keyboard>/numpad5", "N5"},
        {"<Keyboard>/numpad6", "N6"},       {"<Keyboard>/numpad7", "N7"},       {"<Keyboard>/numpad8", "N8"},
        {"<Keyboard>/numpad9", "N9"},       {"<Mouse>/leftButton", "LCLK"},     {"<Mouse>/rightButton", "RCLK"}
    };

    private void OnDisable()
    {
        if(rebindingOperation != null)
            rebindingOperation.Dispose();
    }
    public List<InputActionReference> actionList
    {
        get { return controlScroll.isModeRoam ? actionList_roam : actionList_battle;}
    }

    public List<InputActionReference> GetActionList(string mode_name)
    {
        if (mode_name == "FREEROAM")
            return actionList_roam;
        else if (mode_name == "BATTLE")
            return actionList_battle;
        
        return null;
    }
    public void StartRebinding()
    {
        isBinding = true;
        playerMovement.PlayerInput.actions.Disable();

        MoveCover();
        waitCover.SetActive(true);

        GameManager.gm_instance.SwitchActionMap("Menu");

        current_action = actionList[controlScroll.InputMenuNumber].action;

        optionScript.RebindPushUp();

        // WASD composite input // Only applied for Freeroam controls
        if (controlScroll.InputMenuNumber <= 3 && controlScroll.isModeRoam)
        {
            isComposite = true;

            bindingIndex = current_action.bindings.IndexOf(x => x.isPartOfComposite && x.path == InputPath(controlScroll.InputMenuNumber, ""));
            cachePath = current_action.bindings[bindingIndex].effectivePath;

            // KEYBOARD
            if(controlScroll.isKeyBoard){
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsHavingToMatchPath("<Keyboard>")

                    .WithControlsExcluding("<Keyboard>/anyKey")
                    .WithControlsExcluding("<Keyboard>/escape")
                    .WithControlsExcluding("<Keyboard>/printScreen")
                    .WithControlsExcluding("<Keyboard>/scrollLock")
                    .WithControlsExcluding("<Keyboard>/pause")

                    .WithTimeout(5f)
                    .WithExpectedControlType("Button")
                    .WithTargetBinding(bindingIndex)
                    .OnMatchWaitForAnother(.1f) // delay

                    .OnCancel(operation => {ExitBind();})
                    .OnComplete(operation => RebindComplete())
                    .Start();
            }
            // GAMEPAD
            else {
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsHavingToMatchPath("<Gamepad>")

                    .WithControlsExcluding("<Gamepad>/leftTriggerButton")
                    .WithControlsExcluding("<Gamepad>/rightTriggerButton")

                    .WithTimeout(5f)
                    .WithExpectedControlType("Button")
                    .WithTargetBinding(bindingIndex)
                    .OnMatchWaitForAnother(.1f)

                    .OnCancel(operation => {ExitBind();})
                    .OnComplete(operation => RebindComplete())
                    .Start();
            }
        }

        // Not composite, single button
        else {
            isComposite = false;

            if(controlScroll.isKeyBoard){
                bindingIndex = current_action.GetBindingIndexForControl(current_action.controls[0]);
                cachePath = current_action.bindings[bindingIndex].effectivePath;
                
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsHavingToMatchPath("<Keyboard>")
                    
                    .WithControlsExcluding("<Keyboard>/anyKey")
                    .WithControlsExcluding("<Keyboard>/escape")
                    .WithControlsExcluding("<Keyboard>/printScreen")
                    .WithControlsExcluding("<Keyboard>/scrollLock")
                    .WithControlsExcluding("<Keyboard>/pause")

                    .WithCancelingThrough("<Keyboard>/escape")
                    
                    .WithTimeout(5f)
                    .OnMatchWaitForAnother(0.1f)

                    .OnCancel(operation => {ExitBind();})
                    .OnComplete(operation => RebindComplete())
                    .Start();
            }
            else {
                bindingIndex = 1;
                cachePath = current_action.bindings[bindingIndex].effectivePath;

                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsHavingToMatchPath("<Gamepad>")

                    .WithControlsExcluding("<Gamepad>/leftTriggerButton")
                    .WithControlsExcluding("<Gamepad>/rightTriggerButton")

                    .WithTimeout(5f)
                    .OnMatchWaitForAnother(0.1f)

                    .OnCancel(operation => {ExitBind();})
                    .OnComplete(operation => RebindComplete())
                    .Start();
            }
        }
    }

    private void RebindComplete()
    {
        if (CheckDuplicateBind(current_action, bindingIndex, isComposite))
        {
            //REVERT BINDING
            current_action.RemoveBindingOverride(bindingIndex);
            current_action.ApplyBindingOverride(bindingIndex, cachePath);
            
            RebindUpdateText(current_action, bindingIndex, controlScroll.InputMenuNumber, cachePath, controlScroll.isKeyBoard);
            ExitBind();
            return;
        } 
        RebindUpdateText(current_action, bindingIndex, controlScroll.InputMenuNumber, cachePath, controlScroll.isKeyBoard);
        ExitBind();
    }

    private bool CheckDuplicateBind(InputAction action, int binding_idx, bool is_composite)
    {
        InputBinding newBinding = action.bindings[binding_idx];

        foreach (InputBinding binding in action.actionMap.bindings){
            if (binding.action == newBinding.action){
                continue;
            }
            if (binding.effectivePath == newBinding.effectivePath){
                ShootCaution(newBinding.effectivePath);
                return true;
            }
        }

        // Check for duplicate composite bindings
        if (is_composite) {
            for (int i = 0; i < ActionArrayLength(action); i++) {
                if (i == binding_idx)
                    continue;
                
                if (action.bindings[i].effectivePath == newBinding.effectivePath) {
                    ShootCaution(newBinding.effectivePath);
                    return true;
                }
            }
        }

        return false;
    }
    //===============================================================================
    public string InputPath(int menuNumber, string current_scheme)
    {
        if (current_scheme == "KEY")
            return Key_Path(menuNumber);

        else if (current_scheme == "PAD")
            return Pad_Path(menuNumber);

        else if (controlScroll.isKeyBoard)
            return Key_Path(menuNumber);

        else
            return Pad_Path(menuNumber);
    }
    private string Key_Path(int menuNumber) // Initial Setting.
    {
        if (menuNumber == 0)
            return "<Keyboard>/w";
        else if(menuNumber == 1)
            return "<Keyboard>/s";
        else if(menuNumber == 2)
            return "<Keyboard>/a";
        else if(menuNumber == 3)
            return "<Keyboard>/d";
        return "";
    }
    private string Pad_Path(int menuNumber) // Initial Setting.
    {
        if (menuNumber == 0)
            return "<Gamepad>/dpad/up";
        else if(menuNumber == 1)
            return "<Gamepad>/dpad/down";
        else if(menuNumber == 2)
            return "<Gamepad>/dpad/left";
        else if(menuNumber == 3)
            return "<Gamepad>/dpad/right";
        return "";
    }
    //===============================================================================

    public void ExitBind()
    {
        rebindingOperation.Dispose(); // stop allocating memory space
        playerMovement.PlayerInput.actions.Enable();

        if (!bindFail.activeSelf)
            waitCover.SetActive(false);
        optionScript.RebindPushupFinish();

        StartCoroutine(ReleaseBind(0.25f));
    }

    IEnumerator ReleaseBind(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        isBinding = false;
        
        GameManager.gm_instance.DetermineKeyOrPad();
        GameManager.gm_instance.SwitchActionMap("Player");
    }

    private void MoveCover()
    {
        float move_y = (-100f * (controlScroll.InputMenuNumber)) - 50f; // local position.x starts form -50.0f

        waitCover.transform.localPosition = new Vector3(waitCover.transform.localPosition.x, move_y);
    }

    private void ShootCaution(string duplicatePath)
    {
        string duplicateName = duplicatePath.Split("/"[0])[1].ToUpper();

        bindFailText.text = UIControl.instance.uiTextDict["Detect_DuplicateBind"];
        bindFailText.text += " : " + duplicateName;
        bindFail.SetActive(true);
    }

    private int ActionArrayLength(InputAction action)
    {
        int count = 0;
        foreach (InputBinding binding in action.bindings)
        {
            count += 1;
        }
        return count;
    }
    public void RebindUpdateText(InputAction targetAction, int binding_idx, int menu_idx, string cached_path, bool is_keyboard, bool isUpdatingRoam = true)
    {
        string changed_path = targetAction.bindings[binding_idx].effectivePath;

        if(is_keyboard) // KEYBOARD
        {
            controlScroll.bindingDisplayText_key()[menu_idx].text = InputControlPath.ToHumanReadableString(
                changed_path,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            string changed_text = LinkKeyText(changed_path, controlScroll.bindingDisplayText_key()[menu_idx].text);

            if (controlScroll.isModeRoam && isUpdatingRoam)
                uIControl.UI_Update_Text(changed_text, cached_path, changed_path);
        }
        else // GAMEPAD
        {
            Sprite changed_sprite = LinkSprite(menu_idx, changed_path);
            if (controlScroll.isModeRoam && isUpdatingRoam)
                uIControl.UI_Update_Sprite(changed_sprite, cached_path, changed_path);
        }
    }

    private string WASDPath(InputAction inputAction, int menu_idx, int key_or_pad, string mode)
    {
        // menu_idx - binding index
        Dictionary<int, string> wasdDict_key = new Dictionary<int, string>();
        Dictionary<int, string> wasdDict_pad = new Dictionary<int, string>();

        if (mode == "FREEROAM")
        {
            wasdDict_key[0] = inputAction.bindings[1].effectivePath; // Move Up
            wasdDict_key[1] = inputAction.bindings[3].effectivePath; // Move Down
            wasdDict_key[2] = inputAction.bindings[2].effectivePath; // Move Left
            wasdDict_key[3] = inputAction.bindings[4].effectivePath; // Move Right

            wasdDict_pad[0] = inputAction.bindings[6].effectivePath; // Move Up
            wasdDict_pad[1] = inputAction.bindings[7].effectivePath; // Move Down
            wasdDict_pad[2] = inputAction.bindings[8].effectivePath; // Move Left
            wasdDict_pad[3] = inputAction.bindings[9].effectivePath; // Move Right
        }
        else if (mode == "BATTLE")
        {
            for (int i = 0; i < 4; i++)
            {
                wasdDict_key[i] = inputAction.bindings[0].effectivePath; 
                wasdDict_pad[i] = inputAction.bindings[1].effectivePath;
            }
        }
        
        return (key_or_pad == 0) ? wasdDict_key[menu_idx] : wasdDict_pad[menu_idx];
    }

    public string ReturnMapPath(int menu_idx, int key_or_pad, string mode)
    {
        // key : 0
        // pad : 1
        InputAction curr_action = null;

        if (mode == "FREEROAM")
            curr_action = actionList_roam[menu_idx].action;
        else if (mode == "BATTLE")
            curr_action = actionList_battle[menu_idx].action;
            
        if (menu_idx <= 3) {
            return WASDPath(curr_action, menu_idx, key_or_pad, mode);
        }
    
        return curr_action.bindings[key_or_pad].effectivePath;
    }

    private string LinkKeyText(string path, string originalTag) {

        if (keyboardShortTags.ContainsKey(path))
            return keyboardShortTags[path];
        
        else if (originalTag.Length > 4)
            return originalTag.Substring(0, 4).ToUpper();
        
        else
            return originalTag.ToUpper();
    }

    public Sprite LinkSprite(int menu_idx, string path, string pad_type = null) {
        
        Sprite matchingSprite = null;
        int spriteIndex = 26;
        string lowerCasePath = path.ToLower();

        for (int i = 0; i < gamepadInputTags.Length; i++)
        {
            if (path == gamepadInputTags[i]) {
                spriteIndex = i;
                break;
            }
        }

        if((lowerCasePath.Contains("trigger") && lowerCasePath.Contains("left")))
            spriteIndex = 10;

        else if ((lowerCasePath.Contains("trigger") && lowerCasePath.Contains("right")))
            spriteIndex = 11;
        
        else if (lowerCasePath.Contains("system") && lowerCasePath.Contains("button"))
            spriteIndex = 24;
        
        else if (lowerCasePath.Contains("touchpad"))
            spriteIndex = 25;
        
        matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[spriteIndex];
        
        if (menu_idx >= 0)
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = matchingSprite;
        
        return matchingSprite;
    }

    public string ShortenKeyDisplay(int menuNumber, string originalTag, string mode)
    {
        string target_path = ReturnMapPath(menuNumber, 0, mode);

        return LinkKeyText(target_path, originalTag);
    }
}
