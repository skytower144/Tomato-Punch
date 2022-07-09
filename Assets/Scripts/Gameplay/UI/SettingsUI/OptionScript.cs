using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class OptionScript : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject optionBase;
    [SerializeField] private List <TextMeshProUGUI> optionTexts;
    [System.NonSerialized] public bool is_busy_option;
    private int optionNumber;
    void Update()
    {
        if(is_busy_option)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                playerMovement.HitMenu();
                CloseOptions();
            }
            else if(Input.GetKeyDown(KeyCode.P))
            {
                CloseOptions();
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                SwitchOption("-");
            }
            else if(Input.GetKeyDown(KeyCode.E))
            {
                SwitchOption("+");
            }
        }
    }
    public void OpenOptions()
    {
        is_busy_option = true;

        optionNumber = 0;
        optionBase.SetActive(true);
    }
    private void CloseOptions()
    {
        is_busy_option = false;
        Invoke("TurnoffOption", 0.1f);
    }
    public void TurnoffOption()
    {
        optionBase.SetActive(false);
    }
    private void SwitchOption(string direction)
    {
        
        if (direction == "+")
        {
            if (optionNumber == 0)
            {
                DOTween.Rewind("option_0_1");
                DOTween.Play("option_0_1");
            }
            else if (optionNumber == 1)
            {
                DOTween.Rewind("option_1_2");
                DOTween.Play("option_1_2");
            }
            if (optionNumber < 2)
                UncolorText();
            optionNumber += 1;
            optionNumber = Mathf.Clamp(optionNumber, 0, 2);
        }
        else if (direction == "-")
        {
            if (optionNumber == 2)
            {
                DOTween.Rewind("option_2_1");
                DOTween.Play("option_2_1");
            }
            else if (optionNumber == 1)
            {
                DOTween.Rewind("option_1_0");
                DOTween.Play("option_1_0");
            }
            if (optionNumber > 0)
                UncolorText();
            optionNumber -= 1;
            optionNumber = Mathf.Clamp(optionNumber, 0, 2);
        }
    }
    private void UncolorText()
    {
        optionTexts[optionNumber].color = new Color32(112, 82, 75, 255);
    }
    private void ColorText()
    {
        optionTexts[optionNumber].color = new Color32(97, 125, 97, 255);
    }
    public void DisplayOption()
    {
        ColorText();
    }
}
