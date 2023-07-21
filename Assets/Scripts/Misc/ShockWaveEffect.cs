using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ShockWaveEffect : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    private float shockWaveTime;
    private Material mat;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    void Start()
    {
        mat = targetImage.material;
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown("space"))
    //         CallShockWave();
    // }

    public void CallShockWave(float duration = 0.75f)
    {
        shockWaveTime = duration;
        StartCoroutine(ShockWaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockWaveAction (float startPos, float endPos)
    {
        mat.SetFloat(_waveDistanceFromCenter, endPos);

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
