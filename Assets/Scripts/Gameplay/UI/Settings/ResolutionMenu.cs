using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResolutionMenu : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private DropDown dropDownControl;
    public TMP_Dropdown resolutionDropdown;
    public Toggle resolutionToggle;
    private List<(int, int)> resolutions;
    private int currentResolutionIndex;
    [SerializeField] private TextMeshProUGUI toggleObj_text, dropdownObj_text, language_text;
    [SerializeField] private Image toggleImg, dropdownImg, leftArrow, rightArrow;
    [SerializeField] private GameObject dropdownDisplay;

    [System.NonSerialized] public int graphicMenuNumber;
    [System.NonSerialized] public bool drop_isActive = false;
    void Start()
    {
        resolutionToggle.isOn = Screen.fullScreen;
    }

    private void OnEnable()
    {
        NormalizeMenu();
        graphicMenuNumber = 0;
        HighlightMenu();

        if (TitleScreen.isTitleScreen)
            TitleScreen.busy_with_menu = true;
    }
    private void OnDisable()
    {
        SaveResolutionSetting();
        SaveLanguageSetting();
    }
    void Update()
    {
        if (!drop_isActive && !dropdownDisplay.activeSelf)
        {
            if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                gameManager.DetectHolding(UINavigate);
            }
            else if (gameManager.WasHolding)
            {
                gameManager.holdStartTime = float.MaxValue;
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                InteractMenu();
            }
        }
    }

    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();

        if (graphicMenuNumber == 2 && ((direction == "RIGHT") || (direction == "LEFT")))
        {
            if (direction == "RIGHT") 
                UIControl.SetLanguage(1);

            else if (direction == "LEFT")
                UIControl.SetLanguage(-1);
            
            StartCoroutine(BlinkLangArrow(0.1f, direction));
            SwitchLanguage();
            return;
        }

        NormalizeMenu();
        if(direction == "DOWN")
            graphicMenuNumber += 1;

        else if(direction == "UP")
            graphicMenuNumber -= 1;
            
        graphicMenuNumber = Mathf.Clamp(graphicMenuNumber, 0, 2);
        HighlightMenu();
    }
    public void SetupGraphic()
    {
        resolutionDropdown.ClearOptions();

        List<string> optionList = new List<string>();
        resolutions = new List<(int, int)>();
        
        for (int i = 8; i >= 1; i--)
        {
            //string option = string.Format("{0} x {1}", 240 * i, 135 * i);
            int width = 16 * (13 - i) * 10;
            int height = 9 * (13 - i) * 10;
            string option = string.Format("{0} x {1}", width, height);
            
            optionList.Add(option);
            resolutions.Add((width, height));
        }

        resolutionDropdown.AddOptions(optionList);
    }
    public void SetFullScreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution (int index)
    {
        currentResolutionIndex = index;
        Screen.SetResolution(resolutions[index].Item1, resolutions[index].Item2, Screen.fullScreen);
    }

    public void NormalizeMenu()
    {
        if (graphicMenuNumber == 0)
        {
            toggleObj_text.color = new Color32(112, 82, 75, 255);
            toggleImg.color = new Color32(233, 199, 199, 255);
        }
        else if (graphicMenuNumber == 1)
        {
            dropdownObj_text.color = new Color32(112, 82, 75, 255);
            dropdownImg.color =  new Color32(185, 179, 160, 255);
        }
        else if (graphicMenuNumber == 2)
        {
            language_text.color = new Color32(112, 82, 75, 255);
            leftArrow.color = new Color32(144, 121, 115, 255);
            rightArrow.color = new Color32(144, 121, 115, 255);
        }
    }
    public void HighlightMenu()
    {
        if (graphicMenuNumber == 0)
        {
            toggleObj_text.color = new Color32(97, 125, 97, 255);
            toggleImg.color = new Color32(201, 233, 199, 255);
        }
        else if (graphicMenuNumber == 1)
        {
            dropdownObj_text.color = new Color32(97, 125, 97, 255);
            dropdownImg.color = new Color32(134, 166, 134, 255);
        }
        else if (graphicMenuNumber == 2)
        {
            language_text.color = new Color32(97, 125, 97, 255);
            leftArrow.color = new Color32(120, 147, 120, 255);
            rightArrow.color = new Color32(120, 147, 120, 255);
        }
    }

    private void InteractMenu()
    {
        if (graphicMenuNumber == 0)
        {
            resolutionToggle.isOn = !resolutionToggle.isOn;
        }
        else if (graphicMenuNumber == 1)
        {
            resolutionDropdown.Show();
            dropDownControl.GatherResolution();
        }
    }

    private void SaveResolutionSetting()
    {
        PlayerPrefs.SetInt("FullScreenState", Convert.ToInt32(resolutionToggle.isOn));
        PlayerPrefs.SetInt("ResolutionState", resolutionDropdown.value);
    }

    public void LoadResolutionSetting()
    {
        int loaded_fullscreen = PlayerPrefs.GetInt("FullScreenState");
        SetFullScreen(Convert.ToBoolean(loaded_fullscreen));

        int loaded_resolution = PlayerPrefs.GetInt("ResolutionState", resolutions.Count - 1);
        resolutionDropdown.value = loaded_resolution;
        SetResolution(loaded_resolution);
    }
    private void SaveLanguageSetting()
    {
        PlayerPrefs.SetInt("LanguageSetting", (int)UIControl.currentLang);
    }

    public void LoadLanguageSetting()
    {
        int saved_lang_setting = PlayerPrefs.GetInt("LanguageSetting", 0);
        UIControl.currentLang = (LanguageType)saved_lang_setting;
        SwitchLanguage();
    }

    private void SwitchLanguage()
    {
        LocalizeUI.OnLocalizeUI?.Invoke();
    }

    IEnumerator BlinkLangArrow(float interval, string direction)
    {
        if (direction == "RIGHT")
            rightArrow.color = new Color32(169, 90, 69, 255);
        else
            leftArrow.color = new Color32(169, 90, 69, 255);
        
        yield return WaitForCache.GetWaitForSecondReal(interval);

        if (direction == "RIGHT")
            rightArrow.color = new Color32(144, 121, 115, 255);
        else
            leftArrow.color = new Color32(144, 121, 115, 255);
    }
}
