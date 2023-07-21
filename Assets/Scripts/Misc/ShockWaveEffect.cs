using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveEffect : MonoBehaviour
{
    [SerializeField] private float shockWaveTime = 0.75f;
    [SerializeField] private SpriteRenderer sr;
    private Material mat;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    void Start()
    {
        mat = sr.material;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
            CallShockWave();
    }

    public void CallShockWave()
    {
        StartCoroutine(ShockWaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockWaveAction (float startPos, float endPos)
    {
        mat.SetFloat(_waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < shockWaveTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / shockWaveTime));
            mat.SetFloat(_waveDistanceFromCenter, lerpedAmount);

            yield return null;
        }
        
    }
}
