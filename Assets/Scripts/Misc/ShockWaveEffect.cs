using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ShockWaveEffect : MonoBehaviour
{
    [SerializeField] private Material mat;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
    private static int _waveStrength = Shader.PropertyToID("_ShockWaveStrength");
    private static int _size = Shader.PropertyToID("_Size");
    private float shockWaveTime;
    private float _cacheSize;

    public void CallShockWave(float duration, float size, float strength)
    {
        GameManager.gm_instance.battle_system.parallax.enabled = false;
        _cacheSize = size;
        shockWaveTime = duration;
        mat.SetFloat(_size, size);
        mat.SetFloat(_waveDistanceFromCenter, strength);
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
        while (_cacheSize > 0) {
            _cacheSize -= 0.01f;
            mat.SetFloat(_size, _cacheSize);
            yield return null;
        }
        ResetMat();
        GameManager.gm_instance.battle_system.parallax.enabled = true;
    }

    public void ResetMat()
    {
        mat.SetFloat(_size, 0);
    }
}

[System.Serializable]
public class ShockWaveInfo
{
    public float Duration, Size, Strength;
}
