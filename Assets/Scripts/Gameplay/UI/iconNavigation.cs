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
    [SerializeField] private List <Button> button;
    
    private int iconNumber;
    void Start()
    {
        if (iconNumber != 0){
            buttonNormalize(iconNumber);
        }
        iconNumber = 0;
        buttonHighlight(iconNumber);
    }
    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            buttonNormalize(iconNumber);
            uiBundle[iconNumber].SetActive(false);
            
            iconNumber = (iconNumber + 1)%4;
            button[iconNumber].Select();
            uiBundle[iconNumber].SetActive(true);
            buttonHighlight(iconNumber);
            
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            buttonNormalize(iconNumber);
            uiBundle[iconNumber].SetActive(false);
            
            iconNumber = (iconNumber + 3)%4;
            button[iconNumber].Select();
            uiBundle[iconNumber].SetActive(true);
            buttonHighlight(iconNumber);
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            playerMovement.HitStatus();
        }
    }

    public void status_enableStart()
    {
        Start();
    }
    public void iconNumberInitialize()
    {
        buttonNormalize(iconNumber);

        if(uiBundle[0].activeSelf){
            iconNumber = 0;
        }
        else if(uiBundle[1].activeSelf){
            iconNumber = 1;
        }
        else if(uiBundle[2].activeSelf){
            iconNumber = 2;
        }
        else if(uiBundle[3].activeSelf){
            iconNumber = 3;
        }
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
