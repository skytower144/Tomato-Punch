using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class superSlotBox : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private List <TextMeshProUGUI> textField;
    [SerializeField] private Color highLightedColor;
    [SerializeField] private SuperSlotNavigation superSlotNavigation;
    [SerializeField] private InventoryUI inventoryUI;
    private int textNum, slotNum;
    private void OnEnable()
    {
        textNum = 0;
        HighLightText(textNum);
        slotNum = superSlotNavigation.s_invNumber;
    }
    private void Update()
    {   
        if(playerMovement.Press_Key("Interact"))
        {   
            if (textNum == 0){
                inventoryUI.AddColor_S(slotNum);
            }
            SlotNavigation.isBusy = false;
            gameObject.SetActive(false);
        }
        else if(playerMovement.Press_Key("Cancel"))
        {
            SlotNavigation.isBusy = false;
            gameObject.SetActive(false);
        }
        else if (playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
        {
            gameManager.DetectHolding(UINavigate);
        }
        else if (gameManager.WasHolding)
        {
            gameManager.holdStartTime = float.MaxValue;
        }
    }

    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();

        if(direction == "UP")
        {
            textNum += 1;
            if (textNum > 1)
                textNum = 0;
        }
        else if(direction == "DOWN")
        {
            textNum -= 1;
            if (textNum < 0)
                textNum = 1;
        }
        HighLightText(textNum);
    }
    private void HighLightText(int num)
    {
        for (int i=0; i<2; i++)
        {
            if (i==num){
                textField[i].color = highLightedColor;
            }
            else{
                textField[i].color = Color.black;
            }
        }
    }
}
