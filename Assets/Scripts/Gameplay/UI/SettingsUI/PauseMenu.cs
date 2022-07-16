using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private OptionScript optionScript;
    [SerializeField] private List <TextMeshProUGUI> menuList;
    [SerializeField] private GameObject option_bundle;
    private int menuNumber;
    void OnEnable()
    {
        menuNumber = 0;
        HighlightText();
    }
    void OnDisable()
    {
        NormalizeText();
    }
    void Update()
    {
        if(!optionScript.is_busy_option){
            if(playerMovement.Press_Direction("DOWN"))
            {
                Scroll("DOWN");
                HighlightText();
            }
            else if(playerMovement.Press_Direction("UP"))
            {
                Scroll("UP");
                HighlightText();
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                SelectMenu();
            }
            else if(playerMovement.Press_Key("Pause") || (playerMovement.Press_Key("Cancel") && !option_bundle.activeSelf))
            {
                playerMovement.HitMenu();
            }
        }
    }

    public void SpawnPauseMenu(bool state)
    {
        gameObject.SetActive(state);
        DOTween.Rewind("pause_menu");
        DOTween.Play("pause_menu");
    }
    private void Scroll(string direction)
    {
        NormalizeText();

        if(direction == "DOWN")
            menuNumber += 1;
        
        else if(direction == "UP")
            menuNumber -= 1;
  
        menuNumber = Mathf.Clamp(menuNumber, 0, 2);
    }
    private void HighlightText()
    {
        menuList[menuNumber].color = new Color32(176, 74, 74, 255);
    }
    private void NormalizeText()
    {
        menuList[menuNumber].color = new Color32(53, 52, 52, 255);
    }
    private void SelectMenu()
    {
        if (menuNumber == 0)
        {
            playerMovement.HitMenu();
        }
        else if(menuNumber == 2)
        {
            optionScript.OpenOptions();
        }
    }
}
