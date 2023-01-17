using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMode : MonoBehaviour
{
    public static bool isTutorial = false;
    [SerializeField] private GameObject warmupText, controlGuide;
    private Animator anim;
    private TextSpawn textSpawn;

    void OnEnable()
    {
        isTutorial = true;
        textSpawn = transform.parent.gameObject.GetComponent<TextSpawn>();
        anim = textSpawn.enemy_anim;

        GameObject temp = Instantiate(warmupText, transform);
        Destroy(temp, 3f);

        Invoke("StartWorkout", 2.5f);
    }

    private void StartWorkout()
    {
        tomatoControl.isIntro = false;
        StartCoroutine(WarmUp());
        Instantiate(controlGuide, transform);
    }

    IEnumerator WarmUp()
    {
        yield return Play("Tutorial_LeftEvade");
        

        yield return Play("Tutorial_intro");

        yield return Play("Tutorial_RightEvade");
        

        yield return Play("Tutorial_intro");
    }

    private object Play(string animName)
    {
        return StartCoroutine(AnimWaitPlay.Play(anim, animName));
    }

    private void DisplayControlGuide()
    {

    }
}
