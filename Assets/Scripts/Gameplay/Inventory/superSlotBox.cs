using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class superSlotBox : MonoBehaviour
{
    [SerializeField] private List <TextMeshProUGUI> textField;
    [SerializeField] private Color highLightedColor;
    [SerializeField] private SuperSlotNavigation superSlotNavigation;
    [SerializeField] private InventoryUI inventoryUI;
    private int textNum, slotNum;
    private void OnEnable()
    {
        textNum = 0;
        slotNum = superSlotNavigation.s_invNumber;
    }
    private void Update()
    {   
        if(Input.GetKeyDown(KeyCode.O))
        {   
            if (textNum == 0){
                inventoryUI.AddColor_S(slotNum);
            }
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            SlotNavigation.isBusy = false;
            gameObject.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            textNum += 1;
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            textNum -= 1;
        }
        textNum = Mathf.Clamp(textNum,0,1);
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
