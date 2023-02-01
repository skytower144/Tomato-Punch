using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class iconNavigation : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private StatusNavigation statusNavigation;
    [SerializeField] private equipControl equipcontrol;
    [SerializeField] private List <Sprite> SelectedIconSprite;
    [SerializeField] private List <Sprite> defaultIconSprite;
    [SerializeField] private List <Image> buttonImage;
    [SerializeField] private List <InventoryButtonScript> texts;
    [SerializeField] private List <GameObject> uiBundle;
    
    private int iconNumber;
    private int matoMaxIcons;
    void Start()
    {
        if (iconNumber != 0){
            buttonNormalize(iconNumber);
        }
        iconNumber = 0;
        matoMaxIcons = SelectedIconSprite.Count;
        buttonHighlight(iconNumber);
    }
    void Update() 
    {
        if (!equipcontrol.enterEquipNavigation && !statusNavigation.navigating_status)
        {
            if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                GameManager.gm_instance.DetectHolding(UINavigate);
            }
            else if (GameManager.gm_instance.WasHolding)
            {
                GameManager.gm_instance.holdStartTime = float.MaxValue;
            }
        }

        else if(playerMovement.Press_Key("Status"))
        {
            playerMovement.HitStatus();
        }
    }

    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();
        if(direction == "RIGHT")
        {
            FlipPage(1);
        }
        else if(direction == "LEFT")
        {
            FlipPage(-1);
        }
    }

    public void status_enableStart()
    {
        Start();
    }
    private void FlipPage(int direction)
    {
        buttonNormalize(iconNumber);
        uiBundle[iconNumber].SetActive(false);

        iconNumber += direction;
        iconNumber = Mathf.Clamp(iconNumber, 0, matoMaxIcons - 1);

        uiBundle[iconNumber].SetActive(true);
        buttonHighlight(iconNumber);
    }

    void buttonNormalize(int number)
    {
        buttonImage[number].sprite = defaultIconSprite[number];     
        texts[number].buttonNormalizeText();
    }
    void buttonHighlight(int number)
    {
        buttonImage[number].sprite = SelectedIconSprite[iconNumber];
        texts[number].buttonHighlightText();
    }
}
