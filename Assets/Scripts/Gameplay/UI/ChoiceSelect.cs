using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceSelect : MonoBehaviour
{
    const int MAXCHOICE = 2;
    [SerializeField] private Transform cursorPosition;
    [SerializeField] private List<Vector2> navigatePos;
    public Action proceedAction;
    private int choiceNumber = 0;
    public int choice_number => choiceNumber;

    void OnEnable()
    {
        choiceNumber = 0;
        cursorPosition.localPosition = navigatePos[choiceNumber];
    }

    void Update()
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
            ConfirmChoice();
        }
    }

    private void Navigate()
    {
        string direction = PlayerMovement.instance.Press_Direction();

        if (direction == InputDir.UP)
        {
            choiceNumber -= 1;
        }
        else if (direction == InputDir.DOWN)
        {
            choiceNumber += 1;
        }

        choiceNumber = Mathf.Clamp(choiceNumber, 0, MAXCHOICE - 1);
        cursorPosition.localPosition = navigatePos[choiceNumber];
    }

    private void ConfirmChoice()
    {
        proceedAction?.Invoke();
    }
}