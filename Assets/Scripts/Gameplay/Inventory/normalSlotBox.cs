using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class normalSlotBox : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private List <TextMeshProUGUI> textField;
    [SerializeField] private List <Color> highLightedColor;
    [SerializeField] private SlotNavigation slotNavigation;
    [SerializeField] private InventoryUI inventoryUI;
    private int textNum, slotNum;
    private void OnEnable()
    {
        textNum = 0;
        HighLightText(textNum);
        slotNum = slotNavigation.invNumber;
    }
    private void Update()
    {   
        if(playerMovement.Press_Key("Interact"))
        {   
            if (textNum == 0){
                // update left ui image icon
                inventoryUI.AddColor_1(slotNum);
            }
            else if (textNum == 1){
                inventoryUI.AddColor_2(slotNum);
            }
            SlotNavigation.isBusy = false;
            gameObject.SetActive(false);
        }
        else if(playerMovement.Press_Key("Cancel"))
        {
            SlotNavigation.isBusy = false;
            gameObject.SetActive(false);
        }
        else if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
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
            if (textNum > 2)
                textNum = 0;
        }
        else if(direction == "DOWN")
        {
            textNum -= 1;
            if (textNum < 0)
                textNum = 2;
        }
        HighLightText(textNum);
    }
    private void HighLightText(int num)
    {
        for (int i=0; i<3; i++)
        {
            if (i==num){
                textField[i].color = highLightedColor[i];
            }
            else{
                textField[i].color = Color.black;
            }
        }
    }
}
