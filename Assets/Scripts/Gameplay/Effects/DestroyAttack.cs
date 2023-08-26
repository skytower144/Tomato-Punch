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
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
        GameManager.gm_instance.battle_system.enemy_control.detectEvasion();
        Destroy(gameObject);
    }
}
