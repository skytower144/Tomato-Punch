using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI_Control : MonoBehaviour
{

    [SerializeField] private Animator anim, button_anim_L, button_anim_R, tomato_anim;
    [SerializeField] private GameObject gatleButtons, gatleCircle, upperCircle, tomatoRush_ef, parryBg, parry_Bar;
    [HideInInspector] static public bool gatleCircle_once = false;
    [HideInInspector] static public bool stopGatle = false;
    [SerializeField] private GuardBar guardBar;

    void Update()
    {
        if(battleJola_parried.isParried && battleJolaControl.isPhysical)
        {
            if(!gatleCircle_once)
            {
                gatleCircle_once = true;                            // stop from repeating inside update

                guardBar.RestoreGuardBar();

                Invoke("fade_InOut", 0.2f);
                Invoke("activate_GatleButton", 0.6f);
                Invoke("activate_GatleCircle", 3f);
            }

            if(gatleCircleControl.failUppercut)
            {
                Debug.Log("fail");
                stopGatle = true;

                gatleCircle.SetActive(false);

                tomato_anim.Play("tomato_gatling2idle",-1,0f);
                //enemy_anim.Play("battleJola_parried2idle",-1,0f); --> battleJolaControl
                
                anim.Play("fade_InOut",-1,0f);
                button_anim_L.Play("gatleButton_L_blink",-1,0f);
                button_anim_R.Play("gatleButton_R_blink",-1,0f);

                tomatoControl.isGatle = false;
                tomatoControl.isGuard = false;
                tomatoGuard.isParry = false;
                battleJola_parried.isParried = false;

                gatleCircle_once = false;
                tomatoControl.gatleButton_once = false;

                gatleCircleControl.failUppercut = false;            // stop from repeating inside update
            }
            else if(tomatoControl.uppercutYes)
            {
                Debug.Log("upper");
                stopGatle = true;

                gatleCircleControl.uppercut_time = false;
                gatleCircle.SetActive(false);
                gatleButtons.SetActive(false);

                tomato_anim.Play("tomato_upperReady",-1,0f);
                Instantiate (upperCircle);
                Instantiate (tomatoRush_ef);
                anim.Play("fade_darken",-1,0f);

                tomatoControl.isGatle = false;

                tomatoControl.uppercutYes = false;                  // stop from repeating inside update
            }
        }
        if(tomatoControl.enemyUppered)
        {
            tomatoControl.enemyUppered = false;

            parryBg.SetActive(false);
            parry_Bar.SetActive(false);
            anim.Play("defaultUI",-1,0f);
        }
    }

    void activate_GatleButton()
    {
        gatleButtons.SetActive(true);
    }

    void activate_GatleCircle()
    {
        gatleCircle.SetActive(true);
    }

    void fade_InOut()
    {
        anim.Play("fade_InOut",-1,0f);
    }

    void parryBg_Setup()
    {
        if(battleJola_parried.isParried && battleJolaControl.isPhysical)
        {
            parryBg.SetActive(true);
            parry_Bar.SetActive(true);
        }
        else
        {
            parryBg.SetActive(false);
            parry_Bar.SetActive(false);
        }
    }
}
