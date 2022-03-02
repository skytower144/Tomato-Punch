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
    void destroyPunch()
    {
        if(!isHit && !tomatoGuard.isParry)
            Debug.Log("miss");
        Destroy(gameObject);
    }
}
