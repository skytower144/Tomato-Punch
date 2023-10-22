using UnityEngine;

public class BattleTimeManager : MonoBehaviour
{
    private float slowdownFactor = 0.01f; // 0.5f = 2x slower // lower the number: slower
    private float slowdownLength = 0.8f;
    private float originalFixedDeltaTime;
    private bool startSlowMotion = false;

    void Start()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }
    void Update()
    {
        // timeScale determines the scale at which time is passing
        // Time.deltaTime: the amount of time elapsed since last frame (affected by Time.timeScale)             

        if (Time.timeScale < 1f) {
            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        }
        else if (startSlowMotion && Time.timeScale == 1) {
            startSlowMotion = false;
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }

    public void DoSlowmotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        // fixedUpdate to run every 0.02 seconds
    }

    public void SetSlowSetting(float factor, float length)
    {
        startSlowMotion = true;
        slowdownFactor = factor;
        slowdownLength = length;
    }
}
