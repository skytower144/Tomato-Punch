using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGreyEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private DuplicateRenderer dr;
    [SerializeField] private Color greyedColor;
    [SerializeField] private float interval;
    [System.NonSerialized] public bool isGreyed = false;
    private WaitForSecondsRealtime _greyInterval;

    void Start()
    {
        sr = GameManager.gm_instance.battle_system.enemy_control.enemyRenderer;
        dr = GameManager.gm_instance.battle_system.enemy_control.duplicate_r;

        _greyInterval = new WaitForSecondsRealtime(interval);
    }
    
    IEnumerator GreyEffect()
    {
        while (isGreyed) {
            if (dr.StopFlash) sr.color = greyedColor;
            yield return _greyInterval;
            sr.color = Color.white;
            yield return _greyInterval;
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
