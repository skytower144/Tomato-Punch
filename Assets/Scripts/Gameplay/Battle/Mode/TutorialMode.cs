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
    [SerializeField] private GameObject holdArrow, exitGuide, exitGuide_key, exitGuide_pad;

    [SerializeField] private TextMeshProUGUI exit_key_text;
    [SerializeField] private Image exit_pad_image;

    private GameManager gm;
    private Dictionary<string, Dictionary<string, ControlMapDisplay>> currentMapDict;
    
    [SerializeField] private Animator anim;
    private TextSpawn textSpawn;
    private Coroutine warmupPhase;

    void Update()
    {
        if ((gm.player_movement.Press_Key("SuperSkill")) && (isTutorial))
        {
            StartCoroutine(ExitTutorial());
        }
        
        if (gm.control_scroll.isKeyBoard)
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

    void OnEnable()
    {
        gm = GameManager.gm_instance;
        gm.control_scroll.CaptureCurrentBind();
        currentMapDict = gm.control_scroll.CurrentBindingsDict;

        textSpawn = transform.parent.gameObject.GetComponent<TextSpawn>();
        anim = gm.battle_system.enemy_control.enemyAnim;

        // Exit Button UI
        exit_key_text.text = currentMapDict["BATTLE"]["SuperSkill"].keyboardMap;
        exit_pad_image.sprite = currentMapDict["BATTLE"]["SuperSkill"].gamepadMap[gm.gamepadType];

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

        DOTween.Rewind("tutorial_ui_appear");
        DOTween.Play("tutorial_ui_appear");
    }

    IEnumerator WarmUp()
    {
        while (isTutorial)
        {
            controlGuide.SetActive(true);

            PressButtonAnimation();
            DisplayControlGuide("LeftEvade");
            yield return Play("Tutorial_LeftEvade");
            
            yield return Play("Tutorial_intro");

            PressButtonAnimation();
            DisplayControlGuide("RightEvade");
            yield return Play("Tutorial_RightEvade");

            yield return Play("Tutorial_intro");
            
            PressButtonAnimation();
            DisplayControlGuide("Jump");
            yield return Play("Tutorial_Jump");

            yield return Play("Tutorial_intro");

            PressButtonAnimation();
            DisplayControlGuide("LeftPunch");
            yield return Play("Tutorial_LeftPunch");

            yield return Play("Tutorial_intro");

            PressButtonAnimation();
            DisplayControlGuide("RightPunch");
            yield return Play("Tutorial_RightPunch");

            yield return Play("Tutorial_intro");

            holdArrow.SetActive(true);
            DOTween.Rewind("tutorial_hold");
            DOTween.Play("tutorial_hold");
            DisplayControlGuide("Guard");
            yield return Play("Tutorial_Guard");

            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(1f));

            PressButtonAnimation();
            DisplayControlGuide2("RightPunch");
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

        DOTween.Complete("tutorial_ui_appear");

        DOTween.Rewind("tutorial_exit");
        DOTween.Play("tutorial_exit");
        
        gm.battle_system.tomato_control.ReleaseGuard();
        tomatoControl.isVictory = true;
        gm.battle_system.tomato_control.tomatoAnim.Play("tomato_victory", -1, 0f);

        Play("Tutorial_Finish");
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(0.3f));
        Play("Tutorial_Finish_Idle");

        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(1.5f));

        gm.battle_system.ExitBattle(true);
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
        gamepad_image.sprite = currentMapDict["BATTLE"][moveName].gamepadMap[gm.gamepadType];
        exit_pad_image.sprite = currentMapDict["BATTLE"]["SuperSkill"].gamepadMap[gm.gamepadType];
    }

    private void DisplayControlGuide2(string moveName)
    {
        keyboard_text2.text = currentMapDict["BATTLE"][moveName].keyboardMap;
        gamepad_image2.sprite = currentMapDict["BATTLE"][moveName].gamepadMap[gm.gamepadType];
    }
}
