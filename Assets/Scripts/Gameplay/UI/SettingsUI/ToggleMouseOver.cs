using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleMouseOver : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private Image dropDownBox;

    public void OnPointerEnter(PointerEventData eventData)
    {
        resolutionMenu.drop_isActive = false;
        
        dropDownBox.color = new Color32(185, 179, 160, 255);

        resolutionMenu.NormalizeMenu();
        resolutionMenu.graphicMenuNumber = 0;
        resolutionMenu.HighlightMenu();
    }
}
