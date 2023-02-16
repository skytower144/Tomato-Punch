using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmPrompt : MonoBehaviour
{
    [SerializeField] private List <TextMeshProUGUI> choiceTextList;
    [SerializeField] private List <Image> choiceFrameList;
    [SerializeField] private Color32 choiceHighlight_frame, choiceHighlight_text, choiceDefault;
    [SerializeField] private string ui_localization_tag;
    public Action confirm_action, cancel_action;
    private int choiceNumber = 0;
    private bool isSelectedChoice = false;
    void Start()
    {
        choiceTextList[2].text = UIControl.instance.uiTextDict[ui_localization_tag];
        UIControl.instance.SetFontData(choiceTextList[2], "ConfirmPrompt");

        choiceTextList[0].text = UIControl.instance.uiTextDict["ConfirmPrompt_Yes"];
        UIControl.instance.SetFontData(choiceTextList[0], "ConfirmPrompt_Yes");

        choiceTextList[1].text = UIControl.instance.uiTextDict["ConfirmPrompt_No"];
        UIControl.instance.SetFontData(choiceTextList[1], "ConfirmPrompt_No");
    }

    void Update()
    {
        if (!isSelectedChoice)
        {
            if (PlayerMovement.instance.InputDetection(PlayerMovement.instance.ReturnMoveVector()))
            {
                GameManager.gm_instance.DetectHolding(Navigate);
            }
            else if (GameManager.gm_instance.WasHolding)
            {
                GameManager.gm_instance.holdStartTime = float.MaxValue;
            }
            else if(PlayerMovement.instance.Press_Key("Interact"))
            {
                isSelectedChoice = true;
                ConfirmChoice();
            }
            else if(PlayerMovement.instance.Press_Key("Cancel"))
            {
                isSelectedChoice = true;
                ExitPrompt();
            }
        }
    }

    private void Navigate()
    {
        string direction = PlayerMovement.instance.Press_Direction();
      
        choiceTextList[choiceNumber].color = choiceDefault;
        choiceFrameList[choiceNumber].color = choiceDefault;
        if (direction == "LEFT")
        {
            choiceNumber = 0;
        }
        else if (direction == "RIGHT")
        {
            choiceNumber = 1;
        }
        choiceTextList[choiceNumber].color = choiceHighlight_text;
        choiceFrameList[choiceNumber].color = choiceHighlight_frame;
    }

    private void ConfirmChoice()
    {
        if (choiceNumber == 0)
        {
            confirm_action?.Invoke();
        }
        
        ExitPrompt();
    }
    private void ExitPrompt()
    {
        cancel_action?.Invoke();
        Destroy(gameObject);
    }

    public void InitializeData(Action confirmAction, Action cancelAction, string uiLocalizedTag)
    {
        confirm_action = confirmAction;
        cancel_action = cancelAction;
        ui_localization_tag = uiLocalizedTag;
    }
}
