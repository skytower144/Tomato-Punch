using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaksung : MonoBehaviour
{
    [SerializeField] private Animator gaksung_anim;
    void gaksung_idle()
    {
        gaksung_anim.Play("Gaksung_idle",-1,0f);
    }
}
