using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialMode : MonoBehaviour
{
    public static bool isTutorial = false;

    [SerializeField] private GameObject warmupText, controlGuide, keyboardGuide, gamepadGuide;
    [SerializeField] private TextMeshProUGUI keyboard_text;
    [SerializeField] private Image gamepad_image;

    [SerializeField] private GameObject controlGuide2, keyboardGuide2, gamepadGuide2;
    [SerializeField] private TextMeshProUGUI keyboard_text2;
    [SerializeField] private Image gamepad_image2;

    [SerializeField] private Animator tutorialUI;
    [SerializeField] private GameObject holdArrow, exitGuide, exitGuide_key, exitGuide_pad, pad_ps4, pad_xbox, pad_switch;
    
    private Animator anim;
    private TextSpawn textSpawn;
    private Dictionary<string, Dictionary<string, ControlMapDisplay>> currentMapDict;
    private Coroutine warmupPhase;

    void Update()
    {
        if ((GameManager.gm_instance.player_movement.Press_Key("Esc")) && (isTutorial))
        {
            StartCoroutine(ExitTutorial());
        }
        
        if (GameManager.gm_instance.control_scroll.isKeyBoard)
        {
            exitGuide_key.SetActive(true);
            exitGuide_pad.SetActive(false);

            keyboardGuide.SetActive(true);
            gamepadGuide.SetActive(false);

            keyboardGuide2.SetActive(true);
            gamepadGuide2.SetActive(false);
        }
        else
        {
            exitGuide_key.SetActive(false);
            exitGuide_pad.SetActive(true);
            UpdatePadType();

            keyboardGuide.SetActive(false);
            gamepadGuide.SetActive(true);

            keyboardGuide2.SetActive(false);
            gamepadGuide2.SetActive(true);
        }
    }

    void OnDisable()
    {
        controlGuide.SetActive(false);
        exitGuide.SetActive(false);
        Destroy(gameObject);
    }

    private void UpdatePadType()
    {
        int gamePadType = GameManager.gm_instance.gamepadType;
        if (gamePadType == 1)
        {
            pad_xbox.SetActive(true);
            pad_ps4.SetActive(false);
            pad_switch.SetActive(false);
        }
        else if (gamePadType == 2)
        {
            pad_xbox.SetActive(false);
            pad_ps4.SetActive(true);
            pad_switch.SetActive(false);
        }
        else if (gamePadType == 3)
        {
            pad_xbox.SetActive(false);
            pad_ps4.SetActive(false);
            pad_switch.SetActive(true);
        }

    }
    void OnEnable()
    {
        textSpawn = transform.parent.gameObject.GetComponent<TextSpawn>();
        anim = GameManager.gm_instance.battle_system.enemy_control.enemyAnim;
        currentMapDict = GameManager.gm_instance.control_scroll.CaptureCurrentBind();

        GameObject temp = Instantiate(warmupText, transform);
        Destroy(temp, 3f);

        Invoke("StartWorkout", 2.5f);
    }

    private void StartWorkout()
    {
        tomatoControl.isIntro = false;
        isTutorial = true;
        warmupPhase = StartCoroutine(WarmUp());

        controlGuide.SetActive(true);
        exitGuide.SetActive(true);
    }

    IEnumerator WarmUp()
    {
        while (isTutorial)
        {
            controlGuide.SetActive(true);

            PressButtonAnimation();
            DisplayControlGuide("* Evade Left");
            yield return Play("Tutorial_LeftEvade");
            
            yield return Play("Tutorial_intro");

            PressButtonAnimation();
            DisplayControlGuide("* Evade Right");
            yield return Play("Tutorial_RightEvade");

            yield return Play("Tutorial_intro");
            
            PressButtonAnimation();
            DisplayControlGuide("* Jump");
            yield return Play("Tutorial_Jump");

            yield return Play("Tutorial_intro");

            PressButtonAnimation();
            DisplayControlGuide("* Left Punch");
            yield return Play("Tutorial_LeftPunch");

            yield return Play("Tutorial_intro");

            PressButtonAnimation();
            DisplayControlGuide("* Right Punch");
            yield return Play("Tutorial_RightPunch");

            yield return Play("Tutorial_intro");

            holdArrow.SetActive(true);
            DOTween.Rewind("tutorial_hold");
            DOTween.Play("tutorial_hold");
            DisplayControlGuide("* Guard");
            yield return Play("Tutorial_Guard");

            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(1f));

            PressButtonAnimation();
            DisplayControlGuide2("* Right Punch");
            controlGuide2.SetActive(true);
            yield return Play("Tutorial_Parry");
            controlGuide.SetActive(false);
            controlGuide2.SetActive(false);
            holdArrow.SetActive(false);

            yield return Play("Tutorial_intro");
            yield return Play("Tutorial_intro");
        }
        
        yield break;
    }

    IEnumerator ExitTutorial()
    {
        isTutorial = false;
        StopCoroutine(warmupPhase);
        
        tomatoControl tomato_control = GameManager.gm_instance.battle_system.tomato_control;
        tomato_control.ReleaseGuard();
        tomatoControl.isVictory = true;
        tomato_control.tomatoAnim.Play("tomato_victory", -1, 0f);

        Play("Tutorial_Finish");
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(1.5f));

        GameManager.gm_instance.battle_system.ExitBattle();
        yield break;
    }

    private void PressButtonAnimation()
    {
        tutorialUI.Play("Tutorial_press", -1, 0f);
    }

    private object Play(string animName)
    {
        return StartCoroutine(AnimWaitPlay.Play(anim, animName));
    }

    private void DisplayControlGuide(string moveName)
    {
        keyboard_text.text = currentMapDict["BATTLE"][moveName].keyboardMap;
        gamepad_image.sprite = currentMapDict["BATTLE"][moveName].gamepadMap[GameManager.gm_instance.gamepadType];
    }

    private void DisplayControlGuide2(string moveName)
    {
        keyboard_text2.text = currentMapDict["BATTLE"][moveName].keyboardMap;
        gamepad_image2.sprite = currentMapDict["BATTLE"][moveName].gamepadMap[GameManager.gm_instance.gamepadType];
    }
}
