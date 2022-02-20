using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class normalSlotBox : MonoBehaviour
{
    [SerializeField] private List <TextMeshProUGUI> textField;
    [SerializeField] private Color highLightedColor;
    [SerializeField] private SlotNavigation slotNavigation;
    private int textNum;
    private void Start()
    {
        textNum = 0;
    }
    private void Update()
    {   
        if(Input.GetKeyDown(KeyCode.E))
        {
            slotNavigation.isBusy = false;
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
        textNum = Mathf.Clamp(textNum,0,2);
        HighLightText(textNum);
    }

    private void HighLightText(int num)
    {
        for (int i=0; i<3; i++)
        {
            if (i==num)
                textField[i].color = highLightedColor;
            else
                textField[i].color = Color.black;
        }
    }

    private void OnDisable()
    {
        Start();
    }
}
