using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private List<GameObject> ui_bundle;
    public void UI_Update(bool state)
    {
        // Check if the for loop update process is unnecessary.
        if (controlScroll.isKeyBoard == state){
            return;
        }
        
        controlScroll.isKeyBoard = state;
        
        for (int i = 0; i < ui_bundle.Count; i++)
        {
            ui_bundle[i].GetComponent<CanToggleIcon>()?.ToggleIcon();
        }
    }
}
