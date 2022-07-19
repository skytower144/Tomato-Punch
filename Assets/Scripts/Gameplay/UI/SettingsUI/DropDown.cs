using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropDown : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private List<Toggle> toggle_list;
    private Transform temp_content;
    private int listNumber = 0;
    void Update()
    {
        if(resolutionMenu.drop_isActive)
        {
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
                resolutionMenu.resolutionDropdown.value = listNumber;
                resolutionMenu.SetResolution(resolutionMenu.resolutionDropdown.value);
                ExitDropDown();
            }
            else if(playerMovement.Press_Key("Cancel"))
            {
                ExitDropDown();
            }
        }
    }
    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();

        if(direction == "DOWN")
            IncreaseNumber();
        
        else if(direction == "UP")
            DecreaseNumber();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!resolutionMenu.drop_isActive)
        {
            resolutionMenu.NormalizeMenu();
            resolutionMenu.graphicMenuNumber = 1;
            resolutionMenu.HighlightMenu();

            gameObject.GetComponent<Image>().color = new Color32(134, 166, 134, 255);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        resolutionMenu.drop_isActive = true;

        resolutionMenu.NormalizeMenu();
        resolutionMenu.graphicMenuNumber = 1;
        resolutionMenu.HighlightMenu();
        
        gameObject.GetComponent<Image>().color = new Color32(134, 166, 134, 255);

        Invoke("GatherResolution", 0.15f);
    }

    public void GatherResolution()
    {
        toggle_list.Clear();

        GameObject tempObj = transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        temp_content = tempObj.transform;

        for (int i = 1; i <= 8; i++)
        {
            toggle_list.Add(temp_content.GetChild(i).gameObject.GetComponent<Toggle>());
        }

        listNumber = resolutionMenu.resolutionDropdown.value;
        if (listNumber >= 6)
            temp_content.localPosition = new Vector3(temp_content.localPosition.x, 25.16801f);

        Invoke("ActivateDropDown", 0.15f);
    }
    private void ActivateDropDown()
    {
        resolutionMenu.drop_isActive = true;
    }
    public void ClearResolutionList()
    {
        toggle_list.Clear();
        temp_content = null;
        resolutionMenu.resolutionDropdown.Hide();
    }
    private void IncreaseNumber()
    {
        listNumber += 1;

        if (listNumber < 8)
        {
            toggle_list[listNumber].Select();

            if (listNumber == 6)
                temp_content.localPosition = new Vector3(temp_content.localPosition.x, 25.16801f);
        }
        else
            listNumber -= 1;
    }
    private void DecreaseNumber()
    {
        listNumber -= 1;

        if (listNumber > -1)
        {
            toggle_list[listNumber].Select();

            if (listNumber == 1)
                temp_content.localPosition = new Vector3(temp_content.localPosition.x, 0);
        }
        else
            listNumber += 1;
    }

    private void ExitDropDown()
    {
        resolutionMenu.drop_isActive = false;
        ClearResolutionList();
    }
}
