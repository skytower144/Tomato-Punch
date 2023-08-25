using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDropDown : MonoBehaviour
{
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private DropDown dropDownControl;
    void OnDestroy()
    {
        dropDownControl.ClearResolutionList();
        resolutionMenu.drop_isActive = false;    
    }
}
