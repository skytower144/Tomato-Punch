using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class superBanner : MonoBehaviour
{
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Animator tomato_anim;

    void superSelection()
    {
        tomato_anim.enabled = true;
        if(tomatocontrol.tomatoSuper == 0)
        {
            tomato_anim.Play("tomato_super_chili",-1,0f);
        }
        gameObject.SetActive(false);
    }
    
}
