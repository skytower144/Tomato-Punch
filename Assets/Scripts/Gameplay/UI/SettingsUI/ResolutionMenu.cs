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

    [SerializeField] private List<LanguageSetting> languageList = new List<LanguageSetting>();
    private int languageIndex = 0;

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
                languageIndex += 1;

            else if (direction == "LEFT")
                languageIndex -= 1;
            StartCoroutine(BlinkLangArrow(0.1f, direction));
        
            if (languageIndex >= languageList.Count)
                languageIndex = 0;
            else if (languageIndex < 0)
                languageIndex = languageList.Count - 1;
            
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
            string option = string.Format("{0} x {1}", 240 * i, 135 * i);
            optionList.Add(option);
            
            resolutions.Add((240 * i, 135 * i));
        }

        resolutionDropdown.AddOptions(optionList);

        // resolutionDropdown.value = 0;
        // SetResolution(0);
        // SetFullScreen(true);
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
        resolutionToggle.isOn = Convert.ToBoolean(loaded_fullscreen);

        int loaded_resolution = PlayerPrefs.GetInt("ResolutionState");
        resolutionDropdown.value = loaded_resolution;
        SetResolution(loaded_resolution);
    }

    private void DisplayLanguageMode(LanguageSetting lang_setting)
    {
        language_text.text = lang_setting.display_name;
        language_text.font = lang_setting.font_detail.font_type;
        language_text.fontSize = lang_setting.font_detail.font_size;
        language_text.characterSpacing = lang_setting.font_detail.character_space;
        language_text.wordSpacing = lang_setting.font_detail.word_space;
        language_text.lineSpacing = lang_setting.font_detail.line_space;
    }

    private void SaveLanguageSetting()
    {
        PlayerPrefs.SetString("LanguageSetting", UIControl.currentLangMode);
    }

    public void LoadLanguageSetting()
    {
        string saved_lang_setting = PlayerPrefs.GetString("LanguageSetting");
        
        if (!String.IsNullOrEmpty(saved_lang_setting))
        {
            UIControl.currentLangMode = saved_lang_setting;
            for (int i = 0; i < languageList.Count; i ++)
            {
                if (languageList[i].language_mode == saved_lang_setting)
                {
                    languageIndex = i;
                    SwitchLanguage();
                    return;
                }
            }
        }
    }

    private void SwitchLanguage()
    {
        DisplayLanguageMode(languageList[languageIndex]);
        UIControl.instance.InitializeInkLangDict(languageList[languageIndex].language_mode);
    }

    IEnumerator BlinkLangArrow(float interval, string direction)
    {
        if (direction == "RIGHT")
            rightArrow.color = new Color32(169, 90, 69, 255);
        else
            leftArrow.color = new Color32(169, 90, 69, 255);
        
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(interval));

        if (direction == "RIGHT")
            rightArrow.color = new Color32(144, 121, 115, 255);
        else
            leftArrow.color = new Color32(144, 121, 115, 255);
    }
}
