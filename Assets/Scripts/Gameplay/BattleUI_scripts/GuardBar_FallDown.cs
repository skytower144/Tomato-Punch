using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardBar_FallDown : MonoBehaviour
{
    private RectTransform rectTransform;
    private float fallDownTimer;
    private Image image;
    private Color color;

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        image = transform.GetComponent<Image>();
        color = image.color;
        fallDownTimer = 1f;
    }
    private void Update()
    {
        fallDownTimer -= Time.deltaTime;
        if (fallDownTimer < 0)
        {
            float fallSpeed = 50f;
            rectTransform.anchoredPosition += Vector2.up * fallSpeed * Time.deltaTime;
            
            float alphaFadeSpeed = 4f;
            color.a -= alphaFadeSpeed * Time.deltaTime;
            image.color = color;
            
            if(color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
