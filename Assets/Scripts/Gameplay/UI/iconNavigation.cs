using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class iconNavigation : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
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
        if(playerMovement.Press_Key("RightPage"))
        {
            FlipPage(1);
        }
        else if(playerMovement.Press_Key("LeftPage"))
        {
            FlipPage(-1);
        }
        else if(playerMovement.Press_Key("Status"))
        {
            playerMovement.HitStatus();
        }
    }

    public void status_enableStart()
    {
        Start();
    }
    public void FlipPage(int direction)
    {
        buttonNormalize(iconNumber);

        uiBundle[iconNumber].SetActive(false);
        
        if (direction == 1)
            iconNumber = (iconNumber + 1) % matoMaxIcons;
        else
            iconNumber = (iconNumber + (matoMaxIcons - 1)) % matoMaxIcons;

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
