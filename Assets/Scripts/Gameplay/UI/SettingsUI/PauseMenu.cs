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
    [SerializeField] private GameObject  option_bundle, save_bundle;
    [SerializeField] private SaveLoadMenu saveLoadMenu; public SaveLoadMenu save_load_menu => saveLoadMenu;
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private CanvasGroup displayCanvas; public CanvasGroup display_canvas => displayCanvas;
    [SerializeField] private GameObject QuitPrompt;
    public static bool is_busy = false;
    private int menuNumber;
    private int maxMenuIndex = 4;
    void OnEnable()
    {
        Time.timeScale = 0;
        menuNumber = 0;
        HighlightText();
        MoveArrow();

        option_bundle.SetActive(false);
        save_bundle.SetActive(false);
    }
    void OnDisable()
    {
        Time.timeScale = 1;
        NormalizeText();
    }
    void Update()
    {
        if(!is_busy && !TitleScreen.isTitleScreen){
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
            else if(playerMovement.is_interacting && (playerMovement.Press_Key("Pause") || (playerMovement.Press_Key("Cancel"))))
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
        menuList[menuNumber].color = new Color32(73, 63, 63, 255);
    }
    public void SelectMenu()
    {
        if (menuNumber == 0)
        {
            playerMovement.HitMenu();
        }
        else if (menuNumber == 1 || menuNumber == 2)
        {
            if (menuNumber == 2)
                saveLoadMenu.isLoadMode = true;
            else
                saveLoadMenu.isLoadMode = false;
            saveLoadMenu.GetComponent<CanvasGroup>().alpha = 0;
            save_bundle.SetActive(true);
            
            StartCoroutine(saveLoadMenu.ShowSaveLoadMenu(0.4f));
        }
        else if(menuNumber == 3)
        {
            optionScript.OpenOptions();
        }
        else if(menuNumber == 4)
        {
            GameObject quit_prompt = Instantiate(QuitPrompt, transform);
            quit_prompt.GetComponent<ConfirmPrompt>().InitializeData(QuitGame, CancelQuit, "ConfirmPrompt");
            is_busy = true;
        }
    }
    private void MoveArrow()
    {
        arrowTransform.localPosition = new Vector3(arrowTransform.localPosition.x, menuList[menuNumber].gameObject.transform.localPosition.y);
    }

    public void SetMenuNumber(int inputNumber)
    {
        menuNumber = inputNumber;
    }

    public void QuitGame()
    {
        PauseMenu.is_busy = false;

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    private void CancelQuit()
    {
        PauseMenu.is_busy = false;
    }
}