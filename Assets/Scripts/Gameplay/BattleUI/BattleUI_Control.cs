using UnityEngine;
using DG.Tweening;

public class BattleUI_Control : MonoBehaviour
{
    [SerializeField] private Animator anim, button_anim_L, button_anim_R, tomato_anim;
    [SerializeField] private GameObject gatleButtons, gatleCircle, upperCircle, tomatoRush_ef, parryBg, blackBars;
    [SerializeField] private GuardBar guardBar;
    [System.NonSerialized] public bool stopGatle = false;
    private GameObject temp;
    public bool IsGatleMode => isGatleMode;
    private bool isGatleMode = false;
    private bool cancelEnemyRecover = false;
    void Update()
    {
        if(isGatleMode)
        {
            if(gatleCircleControl.failUppercut)
            {
                isGatleMode = false;
                stopGatle = true;
                tomatoControl.isGatle = false;
                tomatoControl.gatleButton_once = false;
                gatleCircleControl.failUppercut = false;

                gatleCircle.SetActive(false);
                anim.Play("fade_InOut",-1,0f);
                button_anim_L.Play("gatleButton_L_blink",-1,0f);
                button_anim_R.Play("gatleButton_R_blink",-1,0f);

                tomato_anim.Play("tomato_gatling2idle",-1,0f);

                if (!cancelEnemyRecover)
                    GameManager.gm_instance.battle_system.enemy_control.RecoverAnimation();
                cancelEnemyRecover = false;
            }
            else if(tomatoControl.uppercutYes)
            {
                isGatleMode = false;
                stopGatle = true;
                tomatoControl.isGatle = false;
                tomatoControl.uppercutYes = false;
                gatleCircleControl.uppercut_time = false;

                gatleCircle.SetActive(false);
                gatleButtons.SetActive(false);
                GameManager.gm_instance.battle_system.tomato_control.parry_bar.gameObject.SetActive(false);

                Instantiate (upperCircle);
                Instantiate (tomatoRush_ef);

                anim.Play("fade_darken",-1,0f);
                tomato_anim.Play("tomato_upperReady",-1,0f);
            }
        }
    }
    public void EnterGatleMode()
    {
        isGatleMode = true;
        Invoke("fade_InOut", 0.2f);
        Invoke("activate_GatleButton", 0.6f);
        Invoke("activate_GatleCircle", 3f);
    }
    public void CancelGatleCircle()
    {
        CancelInvoke("activate_GatleCircle");
        cancelEnemyRecover = true;
        gatleCircleControl.failUppercut = true;
    }

    void activate_GatleButton()
    {
        if(!Enemy_is_hurt.enemy_isDefeated)
            gatleButtons.SetActive(true);
    }

    void activate_GatleCircle()
    {
        if(!Enemy_is_hurt.enemy_isDefeated)
            gatleCircle.SetActive(true);
    }

    void fade_InOut()
    {
        anim.Play("fade_InOut",-1,0f);
    }

    void parryBg_Setup()
    {
        if(isGatleMode)
        {
            parryBg.SetActive(true);
            GameManager.gm_instance.battle_system.tomato_control.parry_bar.gameObject.SetActive(true);
        }
        else
        {
            parryBg.SetActive(false);
            GameManager.gm_instance.battle_system.tomato_control.parry_bar.gameObject.SetActive(false);
        }
    }
    public void DisableParryBg()
    {
        parryBg.SetActive(false);
        anim.Play("defaultUI",-1,0f);
    }

    public void ShowBattleUI()
    {
        DOTween.Rewind("intro");
        DOTween.Play("intro"); // HealthBar UI DoTween
    }
    public void HideBattleUI()
    {
        DOTween.Rewind("HideBattleUI");
        DOTween.Play("HideBattleUI");
    }
    public void NormalizeBattleUI()
    {
        DOTween.Rewind("intro");
    }
    public void ShowBlackbars()
    {
        temp = Instantiate(blackBars, GameManager.gm_instance.battle_system.enemy_control.PropTransform);
    } 
    public void HideBlackbars()
    {
        DOTween.Play("HideBattleBlackbars");
        Destroy(temp, 0.6f);
    }
}
