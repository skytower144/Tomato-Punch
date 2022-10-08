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
    [SerializeField] private TextMeshProUGUI toggleObj_text, dropdownObj_text;
    [SerializeField] private Image toggleImg, dropdownImg;
    [SerializeField] private List<GameObject> menuList;
    [System.NonSerialized] public int graphicMenuNumber;
    [System.NonSerialized] public bool drop_isActive = false;
    private void OnEnable()
    {
        NormalizeMenu();
        graphicMenuNumber = 0;
        HighlightMenu();
    }
    private void OnDisable()
    {
        SaveResolutionSetting();
    }
    void Update()
    {
        if (!drop_isActive)
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

        NormalizeMenu();
        if(direction == "DOWN")
            graphicMenuNumber += 1;

        else if(direction == "UP")
            graphicMenuNumber -= 1;
            
        graphicMenuNumber = Mathf.Clamp(graphicMenuNumber, 0, 1);
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
}
