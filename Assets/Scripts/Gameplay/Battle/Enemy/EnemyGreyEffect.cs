using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGreyEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private Color greyedColor;
    [SerializeField] private float interval;
    [System.NonSerialized] public bool isGreyed = false;

    void Start()
    {
        sr = GameManager.gm_instance.battle_system.enemy_control.enemyRenderer;
    }
    
    IEnumerator GreyEffect()
    {
        while (isGreyed) {
            sr.color = greyedColor;
            yield return new WaitForSecondsRealtime(interval);
            sr.color = Color.white;
            yield return new WaitForSecondsRealtime(interval);
        }
    }

    public void StartGreyEffect()
    {
        isGreyed = true;
        StartCoroutine(GreyEffect());
    }

    public void StopGreyEffect()
    {
        isGreyed = false;
        StopAllCoroutines();
        sr.color = Color.white;
    }
}
