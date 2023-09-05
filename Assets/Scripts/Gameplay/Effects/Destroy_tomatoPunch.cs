using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_tomatoPunch : MonoBehaviour
{
    private bool isHit = false;
    void OnTriggerEnter2D(Collider2D col)
    {
        isHit = true;
    }
    void OnEnable()
    {
        Invoke("destroyPunch", 0.1f);
    }

    void OnDisable()
    {
        if(!isHit && !tomatoControl.isFainted && !tomatoGuard.isParry && !TutorialMode.isTutorial){
            GameManager.gm_instance.battle_system.tomato_control.isMiss = true;
        }
    }
    
    void destroyPunch()
    {
        Destroy(gameObject);
    }
}
