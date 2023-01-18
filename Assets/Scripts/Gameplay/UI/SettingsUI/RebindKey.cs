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

    Dictionary<string, int> actionTagDict = new Dictionary<string, int>()
    {
        {"Jump", 0},
        {"Guard", 1},
        {"LeftEvade", 2},
        {"RightEvade", 3},
        {"LeftPunch", 4},
        {"RightPunch", 5},
        {"Skill1", 6},
        {"Skill2", 7},
        {"Super", 8}
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
    private string Key_Path(int menuNumber)
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
    private string Pad_Path(int menuNumber)
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

    public string ReturnKeyMapTag(string moveName)
    {
        int actionIndex = actionTagDict[moveName];
        return LinkKeyText(actionList_battle[actionIndex].action.bindings[bindingIndex].effectivePath, "");
    }

    public Sprite ReturnPadMapTag(string moveName)
    {
        int actionIndex = actionTagDict[moveName];
        return LinkSprite(actionIndex, actionList_battle[actionIndex].action.bindings[bindingIndex].effectivePath);
    }

    private string LinkKeyText(string path, string originalTag)
    {
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
    private Sprite LinkSprite(int menu_idx, string path)
    {
        string lowerCasePath = path.ToLower();

        if (path == "<Gamepad>/start")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[0];

        else if(path == "<Gamepad>/select")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[1];
        
        else if(path == "<Gamepad>/dpad/up")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[2];
        
        else if(path == "<Gamepad>/dpad/down")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[3];

        else if(path == "<Gamepad>/dpad/left")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[4];
        
        else if(path == "<Gamepad>/dpad/right")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[5];

        else if(path == "<Gamepad>/buttonEast")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[6];

        else if(path == "<Gamepad>/buttonNorth")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[7];
        
        else if(path == "<Gamepad>/buttonWest")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[8];
        
        else if(path == "<Gamepad>/buttonSouth")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[9];
        
        else if((path == "<Gamepad>/leftTrigger") || (lowerCasePath.Contains("trigger") && lowerCasePath.Contains("left")))
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[10];

        else if((path == "<Gamepad>/rightTrigger") || (lowerCasePath.Contains("trigger") && lowerCasePath.Contains("right")))
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[11];
        
        else if(path == "<Gamepad>/leftShoulder")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[12];
        
        else if(path == "<Gamepad>/rightShoulder")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[13];
        
        else if(path == "<Gamepad>/leftStickPress")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[14];
        
        else if(path == "<Gamepad>/rightStickPress")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[15];
        
        else if(path == "<Gamepad>/rightStick/left")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[16];
        
        else if(path == "<Gamepad>/rightStick/right")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[17];
        
        else if(path == "<Gamepad>/rightStick/down")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[18];
        
        else if(path == "<Gamepad>/rightStick/up")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[19];

        else if(path == "<Gamepad>/leftStick/left")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[20];
        
        else if(path == "<Gamepad>/leftStick/right")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[21];
        
        else if(path == "<Gamepad>/leftStick/down")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[22];
        
        else if(path == "<Gamepad>/leftStick/up")
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[23];
        
        else if (lowerCasePath.Contains("system") && lowerCasePath.Contains("button"))
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[24];
        
        else if (lowerCasePath.Contains("touchpad"))
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[25];
        
        else
            controlScroll.bindingDisplayText_pad()[menu_idx].sprite = controlScroll.gamePadIcons[26];
        
        return controlScroll.bindingDisplayText_pad()[menu_idx].sprite;
    }
}
