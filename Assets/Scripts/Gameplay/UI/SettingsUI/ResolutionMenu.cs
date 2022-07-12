using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResolutionMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    private List<(int, int)> resolutions;
    private int currentResolutionIndex;
    [SerializeField] private List <GameObject> menuList;
    private int menuNumber;
    private void OnEnable()
    {
        menuNumber = 0;
        menuList[0].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(97, 125, 97, 255);
    }
    private void OnDisable()
    {
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
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

        resolutionDropdown.value = 0;
        SetResolution(0);
        SetFullScreen(true);
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
}
