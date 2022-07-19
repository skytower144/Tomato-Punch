using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindKey : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private List<InputActionReference> actionList;
    [SerializeField] private List<GameObject> waitCover;
    private InputAction current_action = null;
    private int bindingIndex, sameIndex;
    private string cachePath;
    private string sameActionName;
    private bool isComposite = false;
    [System.NonSerialized] public bool isBinding = false;
    
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    public void StartRebinding()
    {
        isBinding = true;

        waitCover[controlScroll.InputMenuNumber].SetActive(true);
        playerMovement.PlayerInput.SwitchCurrentActionMap("Menu");

        current_action = actionList[controlScroll.InputMenuNumber].action;

        // WASD composite input //
        if (controlScroll.InputMenuNumber <= 3)
        {
            isComposite = true;

            bindingIndex = current_action.bindings.IndexOf(x => x.isPartOfComposite && x.path == InputPath(controlScroll.InputMenuNumber));
            cachePath = current_action.bindings[bindingIndex].effectivePath;

            if(controlScroll.isKeyBoard){
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsExcluding("<Gamepad>")
                    .WithControlsExcluding("<keyboard>/anyKey")
                    .WithControlsExcluding("<Keyboard>/escape")
                    .WithControlsExcluding("<Keyboard>/printScreen")
                    .WithControlsExcluding("<Keyboard>/scrollLock")
                    .WithControlsExcluding("<Keyboard>/pause")

                    .WithExpectedControlType("Button")
                    .WithTargetBinding(bindingIndex)
                    .OnMatchWaitForAnother(.1f) // delay
                    .OnCancel(operation => {rebindingOperation.Dispose(); StartRebinding();})
                    .OnComplete(operation => RebindComplete())
                    .Start();
            }
            else {
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsExcluding("<Keyboard>")

                    .WithExpectedControlType("Button")
                    .WithTargetBinding(bindingIndex)
                    .OnMatchWaitForAnother(.1f)
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
                    .WithControlsExcluding("<keyboard>/anyKey")
                    .WithControlsExcluding("<Keyboard>/escape")
                    .WithControlsExcluding("<Keyboard>/printScreen")
                    .WithControlsExcluding("<Keyboard>/scrollLock")
                    .WithControlsExcluding("<Keyboard>/pause")
                    
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(operation => RebindComplete())
                    .Start();
            }
            else {
                rebindingOperation = current_action.PerformInteractiveRebinding(bindingIndex)
                    .WithControlsExcluding("<Keyboard>")

                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(operation => RebindComplete())
                    .Start();
            }
        }
    }

    private void RebindComplete()
    {
        if (CheckDuplicateBind(current_action, bindingIndex, isComposite)){

            //REVERT BINDING
            current_action.RemoveBindingOverride(bindingIndex);
            current_action.ApplyBindingOverride(bindingIndex, cachePath);
            
            UpdateText(current_action, bindingIndex, controlScroll.InputMenuNumber);
            ExitBind();
            return;
        } 
        UpdateText(current_action, bindingIndex, controlScroll.InputMenuNumber);
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
                Debug.Log("Duplicate binding found: " + newBinding.effectivePath);
                return true;
            }
        }

        // Check for duplicate composite bindings
        if (is_composite) {
            for (int i = 0; i < ActionArrayLength(action); i++) {
                if (i == binding_idx)
                    continue;
                
                if (action.bindings[i].effectivePath == newBinding.effectivePath) {
                    Debug.Log("Duplicate binding found: " + newBinding.effectivePath);
                    return true;
                }
            }
        }

        return false;
    }
    private string InputPath(int menuNumber)
    {
        if (controlScroll.isKeyBoard){
            if (menuNumber == 0)
                return "<Keyboard>/w";
            else if(menuNumber == 1)
                return "<Keyboard>/s";
            else if(menuNumber == 2)
                return "<Keyboard>/a";
            else if(menuNumber == 3)
                return "<Keyboard>/d";
        }
        else {
            if (menuNumber == 0)
                return "<Gamepad>/leftStick/up";
            else if(menuNumber == 1)
                return "<Gamepad>/leftStick/down";
            else if(menuNumber == 2)
                return "<Gamepad>/leftStick/left";
            else if(menuNumber == 3)
                return "<Gamepad>/leftStick/right";
        }
        return "";
    }

    private void ExitBind()
    {
        rebindingOperation.Dispose(); // stop allocating memory space

        waitCover[controlScroll.InputMenuNumber].SetActive(false);
        playerMovement.PlayerInput.SwitchCurrentActionMap("Player");

        isBinding = false;
    }

    private void ShootCaution()
    {
        Debug.Log("Duplicate binding found");
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
    private void UpdateText(InputAction targetAction, int binding_idx, int menu_idx)
    {
        if(controlScroll.isKeyBoard)
        {
            controlScroll.bindingDisplayText_key[menu_idx].text = InputControlPath.ToHumanReadableString(
                targetAction.bindings[binding_idx].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        else
        {
            LinkSprite(menu_idx, targetAction.bindings[binding_idx].effectivePath);
        }
    }
    private void LinkSprite(int menu_idx, string path)
    {
        if (path == "<Gamepad>/start")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[0];

        else if(path == "<Gamepad>/select")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[1];
        
        else if(path == "<Gamepad>/dpad/up")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[2];
        
        else if(path == "<Gamepad>/dpad/down")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[3];

        else if(path == "<Gamepad>/dpad/left")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[4];
        
        else if(path == "<Gamepad>/dpad/right")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[5];

        else if(path == "<Gamepad>/buttonEast")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[6];

        else if(path == "<Gamepad>/buttonNorth")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[7];
        
        else if(path == "<Gamepad>/buttonWest")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[8];
        
        else if(path == "<Gamepad>/buttonSouth")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[9];
        
        else if(path == "<Gamepad>/leftTrigger")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[10];

        else if(path == "<Gamepad>/rightTrigger")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[11];
        
        else if(path == "<Gamepad>/leftShoulder")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[12];
        
        else if(path == "<Gamepad>/rightShoulder")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[13];
        
        else if(path == "<Gamepad>/leftStickPress")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[14];
        
        else if(path == "<Gamepad>/rightStickPress")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[15];
        
        else if(path == "<Gamepad>/rightStick/left")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[16];
        
        else if(path == "<Gamepad>/rightStick/right")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[17];
        
        else if(path == "<Gamepad>/rightStick/down")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[18];
        
        else if(path == "<Gamepad>/rightStick/up")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[19];

        else if(path == "<Gamepad>/leftStick/left")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[20];
        
        else if(path == "<Gamepad>/leftStick/right")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[21];
        
        else if(path == "<Gamepad>/leftStick/down")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[22];
        
        else if(path == "<Gamepad>/leftStick/up")
            controlScroll.bindingDisplayText_pad[menu_idx].sprite = controlScroll.gamePadIcons[23];
    }
}
