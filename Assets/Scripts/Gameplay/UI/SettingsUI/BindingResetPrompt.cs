using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BindingResetPrompt : MonoBehaviour
{
    private GameManager game_manager = null;
    private ResetBindings reset_bindings = null;
    private PlayerMovement player_movement = null;
    private ControlScroll control_scroll = null;

    private TextMeshProUGUI yes_text, no_text;
    private Image yes_bg, yes_frame, no_bg, no_frame;
    private GameObject tomatoIcon;
    private bool canNavigate = false;
    private int selectNumber = 0;

    void Update()
    {
        if (canNavigate)
        {
            if (player_movement.InputDetection(player_movement.ReturnMoveVector()))
            {
                game_manager.DetectHolding(UINavigate);
            }
            else if (game_manager.WasHolding)
            {
                game_manager.holdStartTime = float.MaxValue;
            }
            else if(player_movement.Press_Key("Interact"))
            {
                ConfirmChoice();
            }
            else if(player_movement.Press_Key("Cancel"))
            {
                ExitPrompt();
            }
        }
    }
    
    public void InitalizeBindPrompt(GameManager gm, PlayerMovement pm, ResetBindings rst, ControlScroll ctrl)
    {
        game_manager = gm;
        player_movement = pm;
        reset_bindings = rst;
        control_scroll = ctrl;

        yes_text = gameObject.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        no_text = gameObject.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();

        yes_bg = gameObject.transform.GetChild(1).GetComponent<Image>();
        no_bg = gameObject.transform.GetChild(2).GetComponent<Image>();

        yes_frame = gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        no_frame = gameObject.transform.GetChild(2).GetChild(0).GetComponent<Image>();

        tomatoIcon = gameObject.transform.GetChild(3).gameObject;
        
        Deselect(); // Because within prefab, both buttons are highlighted. Deselect yes button.
        selectNumber = 1;
        canNavigate = true;
    }

    private void UINavigate()
    {
        string direction = player_movement.Press_Direction();

        Deselect();
        if ((direction == "LEFT"))
            selectNumber -= 1;
        
        else if ((direction == "RIGHT"))
            selectNumber += 1;

        selectNumber = Mathf.Clamp(selectNumber, 0, 1);
        Select();
        MoveIcon();
    }

    private void Deselect()
    {
        if (selectNumber == 0)
        {
            yes_text.alpha = 0.35f;
            yes_bg.color = new Color32(255, 255, 255, 94);
            yes_frame.color = new Color32(115, 91, 91, 74);
        }
        else if (selectNumber == 1)
        {
            no_text.alpha = 0.35f;
            no_bg.color = new Color32(255, 255, 255, 94);
            no_frame.color = new Color32(115, 91, 91, 74);
        }
    }

    private void Select()
    {
        if (selectNumber == 0)
        {
            yes_text.alpha = 1f;
            yes_bg.color = new Color32(255, 255, 255, 148);
            yes_frame.color = new Color32(148, 161, 149, 255);

            DOTween.Rewind("bounce_yes");
            DOTween.Play("bounce_yes");
        }
        else if (selectNumber == 1)
        {
            no_text.alpha = 1f;
            no_bg.color = new Color32(255, 255, 255, 148);
            no_frame.color = new Color32(148, 161, 149, 255);

            DOTween.Rewind("bounce_no");
            DOTween.Play("bounce_no");
        }
    }

    private void MoveIcon()
    {
        if (selectNumber == 0)
            tomatoIcon.transform.localPosition = new Vector3(-318.9f, tomatoIcon.transform.localPosition.y);
        
        else if (selectNumber == 1)
            tomatoIcon.transform.localPosition = new Vector3(2.86f, tomatoIcon.transform.localPosition.y);
    }

    private void ConfirmChoice()
    {
        if (selectNumber == 0)
        {
            reset_bindings.ResetAllBindings(control_scroll.isModeRoam);
            Invoke("ExitPrompt", 0.15f);
        }
        else if (selectNumber == 1)
        {
            ExitPrompt();
        }
    }
    private void ExitPrompt()
    {
        control_scroll.isPrompt = false;
        Destroy(gameObject);
    }

    public void PromptMouseHover(int index)
    {
        if (selectNumber == index)
            return;
        
        Deselect();
        selectNumber = index;
        Select();
        MoveIcon();
    }
    public void PromptMouseDown()
    {
        ConfirmChoice();
    }
}
