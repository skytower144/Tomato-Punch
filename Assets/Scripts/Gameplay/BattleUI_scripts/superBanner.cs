using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class superBanner : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Animator tomato_anim;
    [SerializeField] private battleJolaControl battlejolacontrol;

    void superSelection()
    {
        tomato_anim.enabled = true;
        if(tomatocontrol.super == 1)
        {
            tomato_anim.Play("tomato_super_chili",-1,0f);
        }
        gameObject.SetActive(false);
    }
    
}
