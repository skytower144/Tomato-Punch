using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestCanvas : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    private List<(int, int)> resolutions;
    private int currentResolutionIndex;
    void Start()
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

        resolutionDropdown.value = 1;
        SetResolution(1);
    }
    public void SetFullScreen (bool isFullscreen)
    {
        if (isFullscreen){
            Screen.SetResolution(resolutions[0].Item1, resolutions[0].Item2, true);
        }
        else {
            SetResolution(currentResolutionIndex);
        }
    }

    public void SetResolution (int index)
    {
        currentResolutionIndex = index;
        Screen.SetResolution(resolutions[index].Item1, resolutions[index].Item2, false);
    }
}
