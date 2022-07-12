using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropDown : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private List<GameObject> toggle_list;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!resolutionMenu.dropPointer_pressed)
        {
            resolutionMenu.NormalizeMenu();
            resolutionMenu.graphicMenuNumber = 1;
            resolutionMenu.HighlightMenu();

            gameObject.GetComponent<Image>().color = new Color32(134, 166, 134, 255);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        resolutionMenu.dropPointer_pressed = true;

        resolutionMenu.NormalizeMenu();
        resolutionMenu.graphicMenuNumber = 1;
        resolutionMenu.HighlightMenu();
        
        gameObject.GetComponent<Image>().color = new Color32(134, 166, 134, 255);

        Invoke("GatherResolution", 0.15f);
    }

    public void GatherResolution()
    {
        toggle_list.Clear();
        GameObject temp_content = transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        for (int i = 1; i <= 8; i++)
        {
            toggle_list.Add(temp_content.transform.GetChild(i).gameObject);
        }
    }
    public void ClearResolutionList()
    {
        toggle_list.Clear();
        for (int i = 0; i < 8; i++)
        {
            toggle_list.Add(null);
        }
    }
}
