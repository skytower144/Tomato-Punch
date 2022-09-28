using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private OptionScript optionScript;
    [SerializeField] private List <TextMeshProUGUI> menuList;
    [SerializeField] private GameObject option_bundle, save_bundle;
    [SerializeField] private Transform arrowTransform;

    [System.NonSerialized] public bool is_busy = false;
    private int menuNumber;
    private int maxMenuIndex = 4;
    void OnEnable()
    {
        Time.timeScale = 0;
        menuNumber = 0;
        HighlightText();
        MoveArrow();
    }
    void OnDisable()
    {
        Time.timeScale = 1;
        NormalizeText();
    }
    void Update()
    {
        if(!is_busy){
            if (playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                gameManager.DetectHolding(UINavigate);
            }
            else if (gameManager.WasHolding)
            {
                gameManager.holdStartTime = float.MaxValue;
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                SelectMenu();
            }
            else if(playerMovement.Press_Key("Pause") || (playerMovement.Press_Key("Cancel") && playerMovement.is_interacting))
            {
                playerMovement.HitMenu();
            }
        }
    }

    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();

        if (direction == "UP")
            Scroll("UP");
        
        else if (direction == "DOWN")
            Scroll("DOWN");
        HighlightText();
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

        if(direction == "DOWN"){
            menuNumber += 1;
            if (menuNumber > maxMenuIndex)
                menuNumber = 0;
        }
        
        else if(direction == "UP"){
            menuNumber -= 1;
            if(menuNumber < 0)
                menuNumber = maxMenuIndex;
        }
        MoveArrow();
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
        else if (menuNumber == 1) // Save
        {
            save_bundle.SetActive(true);
            /*
            ProgressManager.instance.CaptureScene();
            ProgressManager.instance.SaveSaveData();
            */
        }
        else if(menuNumber == 3)
        {
            optionScript.OpenOptions();
        }
    }
    private void MoveArrow()
    {
        arrowTransform.localPosition = new Vector3(arrowTransform.localPosition.x, menuList[menuNumber].gameObject.transform.localPosition.y);
    }
}
