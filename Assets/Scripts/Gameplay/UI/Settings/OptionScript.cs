using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class OptionScript : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ResolutionMenu resolutionMenu;
    [SerializeField] private RebindKey rebindKey;
    [SerializeField] private ControlScroll controlScroll;
    [SerializeField] private GameObject optionBase, optionLine;
    [SerializeField] private Animator bgAnimator;
    [SerializeField] private Image illustration;
    public Image optionDrawing => illustration;
    [SerializeField] private List <Sprite> illustrationList;
    [SerializeField] private List <TextMeshProUGUI> optionTexts;
    [SerializeField] private List <GameObject> optionList;
    [SerializeField] private CanvasGroup firstMenu, leftGuide, rightGuide;
    private int optionNumber;
    private bool canNavigate = false;
    void Update()
    {
        if(PauseMenu.is_busy && !controlScroll.isPrompt && canNavigate)
        {
            if(!rebindKey.isBinding && !TitleScreen.isTitleScreen && playerMovement.Press_Key("Pause"))
            {
                canNavigate = false;
                optionBase.SetActive(false);
                PauseMenu.is_busy = false;
                playerMovement.HitMenu();
            }
            else if(!resolutionMenu.drop_isActive && playerMovement.Press_Key("Cancel"))
            {
                CloseOptions();
            }
            else if(playerMovement.Press_Key("LeftPage"))
            {
                SwitchOption("-");
            }
            else if(playerMovement.Press_Key("RightPage"))
            {
                SwitchOption("+");
            }
        }
    }
    public void EnableNavigation()
    {
        canNavigate = true;
        if (TitleScreen.isTitleScreen) {
            TitleScreen.instance.gameObject.SetActive(false);
        }
    }
    public void OpenOptions()
    {
        GameManager.gm_instance.DetermineKeyOrPad();
        PlayerMovement.instance.faderObj.SetActive(false); // Due to FaderCanvas blocking the UI canvas.
        PauseMenu.is_busy = true;

        firstMenu.alpha = 0;

        UncolorText(); // turn off last opened option text
        optionList[optionNumber].SetActive(false); // turn off last opened option
        optionLine.GetComponent<RectTransform>().transform.localPosition = new Vector3(-523.4996f, 455.4f); // optionLine original place

        optionNumber = 0;
        ColorText();
        illustration.sprite = illustrationList[0];

        leftGuide.alpha = 0;
        rightGuide.alpha = 1;
        // ---------------------------------------------------------- //
        
        optionList[0].SetActive(true);
        optionBase.SetActive(true);

        DOTween.Rewind("option_fade");
        DOTween.Play("option_fade");

        firstMenu.DOFade(1, 0.15f).SetDelay(0.3f).SetUpdate(true);

        DOTween.Rewind("option_text_fade");
        DOTween.Play("option_text_fade");

        // EnableNavigation() -> When Unroll animation is completed
    }
    private void CloseOptions()
    {
        canNavigate = false;
        
        DOTween.Complete("option_fade");

        DOTween.Rewind("option_clear");
        DOTween.Play("option_clear");
        bgAnimator.Play("rollOption");
    }
    public void TurnoffOption()
    {
        optionBase.SetActive(false);
        PauseMenu.is_busy = false;
        
        PlayerMovement.instance.faderObj.SetActive(true);
    }
    private void SwitchOption(string direction)
    {
        if (direction == "+")
        {
            if (optionNumber == 0)
            {
                canNavigate = false;
                
                DOTween.Rewind("option_0_1");
                DOTween.Play("option_0_1");

                DOTween.Rewind("left_guide_in");
                DOTween.Play("left_guide_in");
            }
            else if (optionNumber == 1)
            {
                canNavigate = false;
                
                DOTween.Rewind("option_1_2");
                DOTween.Play("option_1_2");

                DOTween.Rewind("right_guide_out");
                DOTween.Play("right_guide_out");
            }
            if (optionNumber < 2)
                ClearOption();
            optionNumber += 1;
            optionNumber = Mathf.Clamp(optionNumber, 0, 2);
        }
        else if (direction == "-")
        {
            if (optionNumber == 2)
            {
                canNavigate = false;
                
                DOTween.Rewind("option_2_1");
                DOTween.Play("option_2_1");

                DOTween.Rewind("right_guide_in");
                DOTween.Play("right_guide_in");
            }
            else if (optionNumber == 1)
            {
                canNavigate = false;
                
                DOTween.Rewind("option_1_0");
                DOTween.Play("option_1_0");

                DOTween.Rewind("left_guide_out");
                DOTween.Play("left_guide_out");
            }
            if (optionNumber > 0)
                ClearOption();
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
        private void ClearOption()
    {
        UncolorText();
        optionList[optionNumber].SetActive(false);
    }
    public void DisplayOption() // OnComplete: option_1_0 DOTween
    {
        ColorText();

        if (optionNumber == 1 && !controlScroll.isKeyBoard)
            illustration.sprite = illustrationList[3]; // Gamepad bg sprite
        
        else
            illustration.sprite = illustrationList[optionNumber];
        optionList[optionNumber].SetActive(true);

        DOTween.Rewind("option_fade");
        DOTween.Play("option_fade");
        
        if (optionNumber == 0)
            firstMenu.DOFade(1, 0.15f).SetUpdate(true);
        else{
            firstMenu.alpha = 0;
        }
        canNavigate = true;
    }

    public void RebindPushUp()
    {
        illustration.enabled = false;
        bgAnimator.Play("rebind_pushup",-1,0f);
    }
    public void RebindPushupFinish()
    {
        bgAnimator.Play("optionBg_default",-1,0f);
        illustration.enabled = true;
    }

    public void OptionToggleDrawing(string current_scheme)
    {
        if (current_scheme == "KEY")
            illustration.sprite = illustrationList[1];
        
        else if (current_scheme == "PAD")
             illustration.sprite = illustrationList[3];
    }
}
