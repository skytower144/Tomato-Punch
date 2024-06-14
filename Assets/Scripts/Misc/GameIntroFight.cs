using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntroFight : MonoBehaviour
{
    public Animator BigSmackAnim, OpponentAnim;
    [SerializeField] private List<Animator> _flashes;
    [SerializeField] private int _flashRate;
    private System.Random _random = new System.Random();
    private float[] _randomDelays = {0, 0.1f, 0.2f, 0.3f};
    private bool _startFlash = false;

    void Start()
    {
        _startFlash = true;
        StartCoroutine(StartCameraFlash());
    }
    IEnumerator StartCameraFlash()
    {
        while (_startFlash) {
            foreach (Animator anim in _flashes) {
                int _randomValue = _random.Next(1, 101);
                if (_randomValue <= _flashRate) {
                    anim.Play("GameIntro_CameraFlash", -1, 0f);
                    yield return WaitForCache.GetWaitForSecond(_randomDelays[_random.Next(0, 4)]);
                }
            }
            yield return WaitForCache.GetWaitForSecond(_randomDelays[_random.Next(0, 3)]);
        }
    }
    public void StopCameraFlash()
    {
        _startFlash = false;
        StopAllCoroutines();
    }
}
