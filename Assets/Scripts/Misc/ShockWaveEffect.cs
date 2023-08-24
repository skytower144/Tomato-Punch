using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ShockWaveEffect : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private RawImage rawImage;

    private float shockWaveTime;
    private Material mat;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
    private static int _size = Shader.PropertyToID("_Size");

    void Start()
    {
        mat = targetImage ? targetImage.material : rawImage.material;
    }

    public void CallShockWave(float duration, float size)
    {
        shockWaveTime = duration;
        mat.SetFloat(_size, size);
        StartCoroutine(ShockWaveAction());
    }

    private IEnumerator ShockWaveAction (float startPos = -0.1f, float endPos = 1f)
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
        mat.SetFloat(_size, 0);
    }
}
