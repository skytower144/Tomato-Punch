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

    [SerializeField] private Color32 highlightColor, defaultColor, disabledColor, choiceHighlight_frame, choiceHighlight_text, choiceDefault;
    [SerializeField] private List <TextMeshProUGUI> menuList;
    [SerializeField] private List <TextMeshProUGUI> choiceTextList;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private List <Image> choiceFrameList;
    [SerializeField] private GameObject confirmBox;
    private PauseMenu pauseMenu;
    private PlayerMovement playerMovement;
    private GameObject darkFilter;
    private CanvasGroup displayCanvas;
    private int menuNumber = 0; private int choiceNumber = 1;
    private int minMenuNumber = 0;
    private bool isPrompt = false;
    private bool saveExists = true;

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
        Init();
    }

    public void Init()
    {
        playerMovement = PlayerMovement.instance;
        pauseMenu = playerMovement.pauseMenu;
        darkFilter = playerMovement.darkFilter;
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
            if (playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                GameManager.gm_instance.DetectHolding(Navigate);
            }
            else if (GameManager.gm_instance.WasHolding)
            {
                GameManager.gm_instance.holdStartTime = float.MaxValue;
            }
            else if (playerMovement.Press_Key("Interact"))
            {
                SelectMenu();
            }
        }

        else if (isPrompt)
        {
            if (playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                GameManager.gm_instance.DetectHolding(Navigate);
            }
            else if (GameManager.gm_instance.WasHolding)
            {
                GameManager.gm_instance.holdStartTime = float.MaxValue;
            }
            else if (playerMovement.Press_Key("Interact"))
            {
                SelectMenu();
            }
            else if (playerMovement.Press_Key("Cancel"))
            {
                ClosePrompt();
            }
        }
    }
    private void Navigate()
    {
        string direction = playerMovement.Press_Direction();

        NormalizeText();

        if (!isPrompt)
        {
            if (direction == "UP")
            {
                menuNumber -= 1;
                if (menuNumber < minMenuNumber)
                    menuNumber = minMenuNumber;
            }
            
            else if (direction == "DOWN")
            {
                menuNumber += 1;
                if (menuNumber > 3)
                    menuNumber = 3;
            }
        }
        else if (isPrompt)
        {
            if (direction == "LEFT")
            {
                choiceNumber = 0;
            }
            else if (direction == "RIGHT")
            {
                choiceNumber = 1;
            }
        }
        HighlightText();
    }

    private void SelectMenu()
    {
        if (!isPrompt)
        {
            GameManager.gm_instance.DetermineKeyOrPad();
            if (menuNumber == 0)
            {
                pauseMenu.SetMenuNumber(2);
                pauseMenu.SelectMenu();
                busy_with_menu = true;
            }
            else if (menuNumber == 1)
            {
                if (saveExists)
                    PromptNewSave();
                else
                    ProceedNewSave();
                busy_with_menu = true;
            }
            else if(menuNumber == 2)
            {
                pauseMenu.SetMenuNumber(3);
                pauseMenu.SelectMenu();
                // gameObject.SetActive(false); --> When Unroll Animation Ends.
            }
            else if(menuNumber == 3)
            {
                pauseMenu.QuitGame();
            }
        }
        else if (isPrompt)
        {
            if (choiceNumber == 0)
                ProceedNewSave();
            ClosePrompt();
        }
    }

    private void HighlightText()
    {
        if (!isPrompt)
            menuList[menuNumber].color = highlightColor;
        else {
            choiceTextList[choiceNumber].color = choiceHighlight_text;
            choiceFrameList[choiceNumber].color = choiceHighlight_frame;
        }
    }
    private void NormalizeText()
    {
        if (!isPrompt)
            menuList[menuNumber].color = defaultColor;
        else {
            choiceTextList[choiceNumber].color = choiceDefault;
            choiceFrameList[choiceNumber].color = choiceDefault;
        }
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

            saveExists = false;
        }
    }

    public void ResetTitle()
    {
        darkFilter.SetActive(pauseMenu.gameObject.activeSelf);
        pauseMenu.gameObject.GetComponent<Image>().enabled = true;
        displayCanvas.alpha = 1;
    }
    private void PromptNewSave()
    {
        isPrompt = true;
        choiceNumber = 1;
        HighlightText();
        confirmBox.SetActive(true);

        DOTween.Rewind("prompt_NewSave");
        DOTween.Play("prompt_NewSave");
    }
    private void ClosePrompt()
    {
        confirmBox.SetActive(false);
        NormalizeText();
        isPrompt = false;
        busy_with_menu = false;
    }
    private void ProceedNewSave()
    {
        pauseMenu.save_load_menu.gameObject.SetActive(true);
        pauseMenu.save_load_menu.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(pauseMenu.save_load_menu.PrepareLoad(true));
    }
}
