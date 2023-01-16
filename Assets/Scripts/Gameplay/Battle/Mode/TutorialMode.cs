using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMode : MonoBehaviour
{
    public static bool isTutorial = false;
    [SerializeField] private GameObject warmupText;
    private Animator anim;
    void OnEnable()
    {
        isTutorial = true;
        anim = transform.parent.gameObject.GetComponent<TextSpawn>().enemy_anim;

        Instantiate(warmupText, transform);
        Invoke("StartWorkout", 2.5f);
    }

    private void StartWorkout()
    {
        tomatoControl.isIntro = false;
        anim.Play("Tutorial_start");
    }
}
