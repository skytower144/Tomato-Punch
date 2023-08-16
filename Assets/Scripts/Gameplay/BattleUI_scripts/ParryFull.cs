using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryFull : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject parryFullEffect;
    void parryFullLoop()
    {
        anim.Play("parryFull_loop",-1,0f);
    }

    void parryFulleffect()
    {
        Instantiate (parryFullEffect, transform.parent);
    }

}
