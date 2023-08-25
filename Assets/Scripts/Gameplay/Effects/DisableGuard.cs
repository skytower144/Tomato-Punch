using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGuard : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }
    void Update()
    {
        if(!tomatoControl.isGuard || tomatoGuard.isParry)
        {
            gameObject.SetActive(false);
        }
    }
}
