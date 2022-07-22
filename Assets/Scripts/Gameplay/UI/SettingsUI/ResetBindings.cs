using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ResetBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private RebindKey rebindKey;
    [SerializeField] private ControlScroll controlScroll;
    private List<string> pervious_paths = new List<string>();
    private List<int> bindingIndexList = new List<int>();
    [System.NonSerialized] public string demand_displayList = "none";
    
    public void ResetAllBindings(bool mode_roam)
    {
        InputAction target_action;
        int bindingIndex;

        // BACKUP PREVIOUS BINDING PATHS
        for (int i = 0; i <= controlScroll.totalMenuNumber; i++)
        {
            target_action = rebindKey.actionList[i].action;
            
            if ((i <= 3) && controlScroll.isModeRoam) // check if composite : applies only to "Player" because "Battle" does not contain composite inputs.
                bindingIndex = target_action.bindings.IndexOf(x => x.isPartOfComposite && x.path == rebindKey.InputPath(i, ""));
            else{
                bindingIndex = target_action.GetBindingIndexForControl(target_action.controls[0]);
            }
            
            
            pervious_paths.Add(target_action.bindings[bindingIndex].effectivePath);
            bindingIndexList.Add(bindingIndex);
        }
        
        // RESET CURRENT [CONTROL SCHEME, MODE] BINDINGS
        for (int i = 0; i <= controlScroll.totalMenuNumber; i++)
        {
            target_action = rebindKey.actionList[i].action;
            
            target_action.RemoveBindingOverride(bindingIndexList[i]);
        }
        
        // UPDATE UI DISPLAY [TEXTS or IMAGES]
        for (int i = 0; i <= controlScroll.totalMenuNumber; i++)
        {
            target_action = rebindKey.actionList[i].action;
            
            rebindKey.RebindUpdateText(target_action, bindingIndexList[i], i, pervious_paths[i], controlScroll.isKeyBoard);
        }
        
        pervious_paths.Clear();
        bindingIndexList.Clear();

        PlayerPrefs.DeleteKey("rebinds");
    }

    public void Load_UpdateUI()
    {
        InputAction target_action;
        int key_bindingIndex;
        int pad_bindingIndex;

        int total_menu = controlScroll.GetMenuNumbers("FREEROAM");
        List<InputActionReference> action_list = rebindKey.GetActionList("FREEROAM");
        
        
        for (int i = 0; i < total_menu; i++)
        {
            target_action = action_list[i].action;

            if (i <= 3) {
                key_bindingIndex = target_action.bindings.IndexOf(x => x.isPartOfComposite && x.path == rebindKey.InputPath(i, "KEY"));
                pad_bindingIndex = target_action.bindings.IndexOf(x => x.isPartOfComposite && x.path == rebindKey.InputPath(i, "PAD"));
            }
            else {
                key_bindingIndex = target_action.bindings.IndexOf(x => x.path.Contains("Keyboard"));
                pad_bindingIndex = target_action.bindings.IndexOf(x => x.path.Contains("Gamepad"));
            }
            
            demand_displayList = "FREEROAM";
            rebindKey.RebindUpdateText(target_action, key_bindingIndex, i, target_action.bindings[key_bindingIndex].path, true); // KEYBOARD
            rebindKey.RebindUpdateText(target_action, pad_bindingIndex, i, target_action.bindings[pad_bindingIndex].path, false); // GAMEPAD
        }
        
        // DO NOT CLEAR LIST ... with shallow copy it will effect the original list.
        
        total_menu = controlScroll.GetMenuNumbers("BATTLE");
        action_list = rebindKey.GetActionList("BATTLE");

        for (int i = 0; i < total_menu; i++)
        {
            target_action = action_list[i].action;

            key_bindingIndex = target_action.bindings.IndexOf(x => x.path.Contains("Keyboard"));
            pad_bindingIndex = target_action.bindings.IndexOf(x => x.path.Contains("Gamepad"));
            
            demand_displayList = "BATTLE";
            rebindKey.RebindUpdateText(target_action, key_bindingIndex, i, target_action.bindings[key_bindingIndex].path, true);
            rebindKey.RebindUpdateText(target_action, pad_bindingIndex, i, target_action.bindings[pad_bindingIndex].path, false);

            demand_displayList = "none";
        }
        
    }
}
