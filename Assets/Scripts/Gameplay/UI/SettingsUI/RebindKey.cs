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

        MoveCover();
        waitCover.SetActive(true);

        playerMovement.PlayerInput.SwitchCurrentActionMap("Menu");

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
                    .WithControlsExcluding("<Gamepad>")
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
                    .WithControlsExcluding("<Keyboard>")
                    .WithControlsExcluding("<Gamepad>/start")

                    .WithCancelingThrough("<Gamepad>/start")

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

            bindingIndex = current_action.GetBindingIndexForControl(current_action.controls[0]);

            cachePath = current_action.bindings[bindingIndex].effectivePath;

            if(controlScroll.isKeyBoard){
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsExcluding("<Gamepad>")
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
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsExcluding("<Keyboard>")
                    .WithControlsExcluding("<Gamepad>/start")

                    .WithCancelingThrough("<Gamepad>/start")

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
            return "<Gamepad>/leftStick/up";
        else if(menuNumber == 1)
            return "<Gamepad>/leftStick/down";
        else if(menuNumber == 2)
            return "<Gamepad>/leftStick/left";
        else if(menuNumber == 3)
            return "<Gamepad>/leftStick/right";
        return "";
    }
    //===============================================================================

    private void ExitBind()
    {
        rebindingOperation.Dispose(); // stop allocating memory space

        if (!bindFail.activeSelf)
            waitCover.SetActive(false);
        optionScript.RebindPushupFinish();

        playerMovement.PlayerInput.SwitchCurrentActionMap("Player");

        StartCoroutine(ReleaseBind(0.25f));
    }

    IEnumerator ReleaseBind(float waitTime)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(waitTime));
        isBinding = false;
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
    public void RebindUpdateText(InputAction targetAction, int binding_idx, int menu_idx, string cached_path, bool is_keyboard)
    {
        string changed_path = targetAction.bindings[binding_idx].effectivePath;

        if(is_keyboard) // KEYBOARD
        {
            controlScroll.bindingDisplayText_key()[menu_idx].text = InputControlPath.ToHumanReadableString(
                changed_path,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
            
            string changed_text = LinkKeyText(changed_path, controlScroll.bindingDisplayText_key()[menu_idx].text);

            if (controlScroll.isModeRoam)
                uIControl.UI_Update_Text(changed_text, cached_path, changed_path);
        }
        else // GAMEPAD
        {
            Sprite changed_sprite = LinkSprite(menu_idx, changed_path);
            if (controlScroll.isModeRoam)
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
        switch (path) {
            case "<Keyboard>/backspace":
                return "BACK";

            case "<Keyboard>/insert":
                return "INSR";

            case "<Keyboard>/pageUp":
                return "PGUP";
            
            case "<Keyboard>/delete":
                return "DEL";
            
            case "<Keyboard>/pageDown":
                return "PGDN";
            
            case "<Keyboard>/capsLock":
                return "CAPS";
            
            case "<Keyboard>/enter":
                return "ENTR";

            case "<Keyboard>/leftShift":
                return "LSHF";
            
            case "<Keyboard>/rightShift":
                return "RSHF";
            
            case "<Keyboard>/leftCtrl":
                return "LCTR";
            
            case "<Keyboard>/leftMeta":
                return "LSYS";
            
            case "<Keyboard>/leftAlt":
                return "LALT";
            
            case "<Keyboard>/space":
                return "SPC";
            
            case "<Keyboard>/rightAlt":
                return "RALT";
            
            case "<Keyboard>/contextMenu":
                return "MENU";

            case "<Keyboard>/rightCtrl":
                return "RCTR";
            
            case "<Keyboard>/upArrow":
                return "UP";
            
            case "<Keyboard>/downArrow":
                return "DOWN";
            
            case "<Keyboard>/leftArrow":
                return "LEFT";
            
            case "<Keyboard>/rightArrow":
                return "RGHT";
            
            case "<Keyboard>/numLock":
                return "NMLK";
            
            case "<Keyboard>/numpadDivide":
                return "N/";

            case "<Keyboard>/numpadMultiply":
                return "N*";
            
            case "<Keyboard>/numpadMinus":
                return "N-";
            
            case "<Keyboard>/numpadPlus":
                return "N+";
            
            case "<Keyboard>/numpadEnter":
                return "NENT";
            
            case "<Keyboard>/numpadPeriod":
                return "N.";
            
            case "<Keyboard>/numpad0":
                return "N0";
            
            case "<Keyboard>/numpad1":
                return "N1";
            
            case "<Keyboard>/numpad2":
                return "N2";
            
            case "<Keyboard>/numpad3":
                return "N3";
            
            case "<Keyboard>/numpad4":
                return "N4";
            
            case "<Keyboard>/numpad5":
                return "N5";
            
            case "<Keyboard>/numpad6":
                return "N6";
            
            case "<Keyboard>/numpad7":
                return "N7";
            
            case "<Keyboard>/numpad8":
                return "N8";
            
            case "<Keyboard>/numpad9":
                return "N9";
            
            case "<Mouse>/leftButton":
                return "LCLK";
            
            case "<Mouse>/rightButton":
                return "RCLK";
            
            default:
                if (originalTag.Length > 4)
                    return originalTag.Substring(0, 4).ToUpper();
                else
                    return originalTag.ToUpper();
        }
    }
    public Sprite LinkSprite(int menu_idx, string path, string pad_type = null) {

        string lowerCasePath = path.ToLower();
        Sprite matchingSprite = null;
        
        if (path == "<Gamepad>/start")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[0];

        else if(path == "<Gamepad>/select")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[1];
        
        else if(path == "<Gamepad>/dpad/up")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[2];
        
        else if(path == "<Gamepad>/dpad/down")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[3];

        else if(path == "<Gamepad>/dpad/left")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[4];
        
        else if(path == "<Gamepad>/dpad/right")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[5];

        else if(path == "<Gamepad>/buttonEast")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[6];

        else if(path == "<Gamepad>/buttonNorth")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[7];
        
        else if(path == "<Gamepad>/buttonWest")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[8];
        
        else if(path == "<Gamepad>/buttonSouth")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[9];
        
        else if((path == "<Gamepad>/leftTrigger") || (lowerCasePath.Contains("trigger") && lowerCasePath.Contains("left")))
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[10];

        else if((path == "<Gamepad>/rightTrigger") || (lowerCasePath.Contains("trigger") && lowerCasePath.Contains("right")))
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[11];
        
        else if(path == "<Gamepad>/leftShoulder")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[12];
        
        else if(path == "<Gamepad>/rightShoulder")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[13];
        
        else if(path == "<Gamepad>/leftStickPress")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[14];
        
        else if(path == "<Gamepad>/rightStickPress")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[15];
        
        else if(path == "<Gamepad>/rightStick/left")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[16];
        
        else if(path == "<Gamepad>/rightStick/right")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[17];
        
        else if(path == "<Gamepad>/rightStick/down")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[18];
        
        else if(path == "<Gamepad>/rightStick/up")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[19];

        else if(path == "<Gamepad>/leftStick/left")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[20];
        
        else if(path == "<Gamepad>/leftStick/right")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[21];
        
        else if(path == "<Gamepad>/leftStick/down")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[22];
        
        else if(path == "<Gamepad>/leftStick/up")
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[23];
        
        else if (lowerCasePath.Contains("system") && lowerCasePath.Contains("button"))
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[24];
        
        else if (lowerCasePath.Contains("touchpad"))
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[25];
        
        else
            matchingSprite = controlScroll.ReturnGamepadIcon(pad_type)[26];
        
        if (menu_idx >= 0)
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = matchingSprite;
        
        return matchingSprite;
    }

    public string ShortenKeyDisplay(string inputName) {
        string lower = inputName.ToLower();

        if (lower.Contains("back") && lower.Contains("space"))
            return "BACK";

        else if (lower.Contains("insert"))
            return "INSR";

        else if (lower.Contains("page") && lower.Contains("up"))
            return "PGUP";
        
        else if (lower.Contains("delete"))
            return "DEL";
        
        else if (lower.Contains("page") && lower.Contains("down"))
            return "PGDN";
        
        else if (lower.Contains("cap"))
            return "CAPS";
        
        else if (lower.Contains("enter"))
            return "ENTR";

        else if (lower.Contains("left") && lower.Contains("shift"))
            return "LSHF";
        
        else if (lower.Contains("right") && lower.Contains("shift"))
            return "RSHF";
        
        else if (lower.Contains("left") && lower.Contains("ctrl"))
            return "LCTR";
        
        else if (lower.Contains("left") && (lower.Contains("meta") || lower.Contains("system")))
            return "LSYS";
        
        else if (lower.Contains("left") && lower.Contains("alt"))
            return "LALT";
        
        else if (lower.Contains("space"))
            return "SPC";
        
        else if (lower.Contains("right") && lower.Contains("alt"))
            return "RALT";
        
        else if (lower.Contains("context") && lower.Contains("menu"))
            return "MENU";

        else if (lower.Contains("right") && lower.Contains("ctrl"))
            return "RCTR";
        
        else if (lower.Contains("up") && lower.Contains("arrow"))
            return "UP";
        
        else if (lower.Contains("down") && lower.Contains("arrow"))
            return "DOWN";
        
        else if (lower.Contains("left") && lower.Contains("arrow"))
            return "LEFT";
        
        else if (lower.Contains("right") && lower.Contains("arrow"))
            return "RGHT";
        
        else if (lower.Contains("num") && lower.Contains("lock"))
            return "NMLK";
        
        else if (lower.Contains("numpad") && lower.Contains("divide"))
            return "N/";

        else if (lower.Contains("numpad") && lower.Contains("multiply"))
            return "N*";
        
        else if (lower.Contains("numpad") && lower.Contains("minus"))
            return "N-";
        
        else if (lower.Contains("numpad") && lower.Contains("plus"))
            return "N+";
        
        else if (lower.Contains("numpad") && lower.Contains("enter"))
            return "NENT";
        
        else if (lower.Contains("numpad") && lower.Contains("period"))
            return "N.";
        
        else if (lower.Contains("numpad") && lower.Contains("0"))
            return "N0";
        
        else if (lower.Contains("numpad") && lower.Contains("1"))
            return "N1";
        
        else if (lower.Contains("numpad") && lower.Contains("2"))
            return "N2";
        
        else if (lower.Contains("numpad") && lower.Contains("3"))
            return "N3";
        
        else if (lower.Contains("numpad") && lower.Contains("4"))
            return "N4";
        
        else if (lower.Contains("numpad") && lower.Contains("5"))
            return "N5";
        
        else if (lower.Contains("numpad") && lower.Contains("6"))
            return "N6";
        
        else if (lower.Contains("numpad") && lower.Contains("7"))
            return "N7";
        
        else if (lower.Contains("numpad") && lower.Contains("8"))
            return "N8";
        
        else if (lower.Contains("numpad") && lower.Contains("9"))
            return "N9";
        
        else if (lower.Contains("left") && lower.Contains("button"))
            return "LCLK";
        
         else if (lower.Contains("right") && lower.Contains("button"))
            return "RCLK";
        
        
        if (inputName.Length > 4)
            return inputName.Substring(0, 4).ToUpper();
        else
            return inputName.ToUpper();
    }

}
