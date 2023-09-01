using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateRenderer : MonoBehaviour
{
    [SerializeField] SpriteRenderer d_sr, enemy_sr;
    [SerializeField] private Material matYellow, matWhite;
    private float flashSpeed;
    private bool stopFlash = true;
    public bool StopFlash => stopFlash;

    void Update()
    {
        d_sr.sprite = enemy_sr.sprite;

        if (!stopFlash)
        {
            Color color = enemy_sr.material.color;
            color.a -= (0.002f + flashSpeed);
            enemy_sr.material.color = color;
        }
    }

    public IEnumerator BlinkEffect(int repeat, float interval)
    {
        for (int i = 0; i < repeat; i++) {
            enemy_sr.material = matWhite;
            yield return WaitForCache.GetWaitForSecondReal(interval);
            enemy_sr.material  = GameManager.gm_instance.battle_system.enemy_control.mat_default;
            yield return WaitForCache.GetWaitForSecondReal(interval);
        }
        yield break;
    }

    public void FlashEffect(float duration, int mat_type)
    {
        flashSpeed = (1 - duration) * 0.001f;
        enemy_sr.material = mat_type == 0 ? matWhite : matYellow;
        stopFlash = false;
        gameObject.SetActive(true);
        enemy_sr.color = Color.white;
        
        StartCoroutine(ResetFlash(duration));
    }
    IEnumerator ResetFlash(float delay)
    {
        yield return WaitForCache.GetWaitForSecondReal(delay);

        enemy_sr.material = GameManager.gm_instance.battle_system.enemy_control.mat_default;
        d_sr.sprite = null;
        stopFlash = true;
        gameObject.SetActive(false);
    }
}