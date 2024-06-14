using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAttack : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DetectEvasionAndDestroy());
    }
    IEnumerator DetectEvasionAndDestroy()
    {
        yield return WaitForCache.GetWaitForSecond(0.1f);

        gameObject.SetActive(false);
        GameManager.gm_instance.battle_system.enemy_control.detectEvasion();
        Destroy(gameObject);
    }
}
