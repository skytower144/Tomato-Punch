using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAttack : MonoBehaviour
{
    void Update()
    {
        if(battleJola_parried.isParried || battleJola_parried.pjParried)
        {
            battleJola_parried.pjParried = false;
            Destroy (gameObject);
        }
    }
    void Start()
    {
        Destroy (gameObject, 0.1f);
    }
}
