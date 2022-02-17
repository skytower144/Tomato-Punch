using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSlotHover : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void SuperSlot_HoverEffect()
    {
        anim.Play("superSlot_hover",-1,0f);
    }
    public void SuperSlot_Default()
    {
        anim.Play("superSlot_default",-1,0f);
    }
}
