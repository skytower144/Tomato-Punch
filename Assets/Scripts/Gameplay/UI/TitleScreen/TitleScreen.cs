using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TitleScreen : MonoBehaviour
{
    public static TitleScreen instance { get; private set; }
    public static bool isTitleScreen = false;
    public static bool busy_with_menu = false;

    [SerializeField] private Color32 highlightColor, defaultColor, disabledColor;
    [SerializeField] private List <TextMeshProUGUI> menuList;
    private PauseMenu pauseMenu;
    private PlayerMovement playerMovement;
    private GameObject darkFilter;
    private CanvasGroup displayCanvas;
    private int menuNumber = 0;
    private int minMenuNumber = 0;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        playerMovement = PlayerMovement.instance;
        pauseMenu = playerMovement.pause_menu;
        darkFilter = playerMovement.dark_filter;
        displayCanvas = pauseMenu.display_canvas;

        pauseMenu.gameObject.GetComponent<Image>().enabled = false;
        displayCanvas.alpha = 0;
        darkFilter.SetActive(true); // because it will be inversed.
        playerMovement.HitMenu();

        AdjustMenuOption();
        HighlightText();
    }

    void Update()
    {
        if (!busy_with_menu)
        {
            if (playerMovement.Press_Key("Move"))
            {
                Navigate();
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                SelectMenu();
            }
        }
    }
    private void Navigate()
    {
        string direction = playerMovement.Press_Direction();

        NormalizeText();

        if (direction == "UP")
        {
            menuNumber -= 1;
            if (menuNumber < minMenuNumber)
                menuNumber = minMenuNumber;
        }
        
        else if (direction == "DOWN")
        {
            menuNumber += 1;
            if (menuNumber > 2)
                menuNumber = 2;
        }

        HighlightText();
    }

    private void SelectMenu()
    {
        if (menuNumber == 0)
        {
            pauseMenu.SetMenuNumber(2);
            pauseMenu.SelectMenu();
        }
        else if (menuNumber == 1)
        {
            pauseMenu.save_load_menu.gameObject.SetActive(true);
            pauseMenu.save_load_menu.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            StartCoroutine(pauseMenu.save_load_menu.PrepareLoad(true));
        }
        else if(menuNumber == 2)
        {
            pauseMenu.SetMenuNumber(3);
            pauseMenu.SelectMenu();
        }
        busy_with_menu = true;
    }

    private void HighlightText()
    {
        menuList[menuNumber].color = highlightColor;
    }
    private void NormalizeText()
    {
        menuList[menuNumber].color = defaultColor;
    }

    private void AdjustMenuOption()
    {
        if (ProgressManager.instance.CheckAnySaveExists())
        {
            minMenuNumber = 0;
            menuNumber = 0;
        }
        else
        {
            minMenuNumber = 1;
            menuNumber = 1;
            menuList[0].color = disabledColor;
        }
    }

    public void ResetTitle()
    {
        darkFilter.SetActive(true);
        pauseMenu.gameObject.GetComponent<Image>().enabled = true;
        displayCanvas.alpha = 1;
    }
}
