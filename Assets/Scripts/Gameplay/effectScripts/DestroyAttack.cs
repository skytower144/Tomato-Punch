using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAttack : MonoBehaviour
{
    void Update()
    {
        if(Enemy_parried.isParried || Enemy_parried.pjParried)
        {
            Enemy_parried.pjParried = false;
            Destroy (gameObject);
        }
    }
    void Start()
    {
        Destroy (gameObject, 0.1f);
    }
}
