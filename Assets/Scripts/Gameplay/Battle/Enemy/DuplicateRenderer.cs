using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateRenderer : MonoBehaviour
{
    [SerializeField] EnemyControl enemyControl;
    [SerializeField] SpriteRenderer sr, parent_sr;
    [SerializeField] private Material matYellow;
    [System.NonSerialized] public float flashSpeed;
    private bool stopFlash;

    void OnEnable()
    {
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
    
}