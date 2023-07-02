using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateRenderer : MonoBehaviour
{
    [SerializeField] EnemyControl enemyControl;
    [SerializeField] SpriteRenderer sr, parent_sr;
    [SerializeField] private Material matYellow, matWhite;
    private int matType = 0;
    private float flashSpeed;
    private bool stopFlash;

    void OnEnable()
    {
        if (matType == 0)
            parent_sr.material = matWhite;
        else
            parent_sr.material = matYellow;
        stopFlash = false;
    }
    void OnDisable()
    {
        stopFlash = true;
        sr.sprite = null;
    }
    void Update()
    {
        sr.sprite = parent_sr.sprite;
        if (!stopFlash)
        {
            Color color = parent_sr.material.color;
            color.a -= (0.002f + flashSpeed);
            parent_sr.material.color = color;
        }
    }

    public void FlashEffect(float duration, int mat_type)
    {
        flashSpeed = (1 - duration) * 0.001f;
        matType = mat_type;
        gameObject.SetActive(true);
    }
}