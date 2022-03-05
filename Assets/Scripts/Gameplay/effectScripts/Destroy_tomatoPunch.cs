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

    void Update()
    {
        if (tomatoHurt.isTomatoHurt)
            destroyPunch();
    }
    
    void destroyPunch()
    {
        if(!isHit && !tomatoGuard.isParry){
            TextSpawn.isMiss = true;
        }
        Destroy(gameObject);
    }
}
