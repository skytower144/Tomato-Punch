using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private RectTransform UiCanvasTransform;
    public IEnumerator Fade(float duration, float delay, int fps, bool isFadeIn, bool setToCanvasPosition = true)
    {
        Color color = _sr.color;
        color.a = isFadeIn ? 1 : 0;
        _sr.color = color;

        if (duration == 0f) {
            color = _sr.color;
            color.a = isFadeIn ? 0 : 1;
            _sr.color = color;
        }
        if (fps <= 0) fps = 60;

        float frameRate = 1f / fps;
        float timer = 0f;
        int mode = isFadeIn ? -1 : 1;

        if (setToCanvasPosition)
            SetPosition();
        
        _sr.gameObject.SetActive(true);
        yield return WaitForCache.GetWaitForSecond(delay);

        while ((isFadeIn && _sr.color.a > 0) || (!isFadeIn && _sr.color.a < 1)) {
            timer += Time.deltaTime;

            if (timer > frameRate) {
                timer = 0f;
                color = _sr.color;
                color.a += frameRate / duration * mode;
                _sr.color = color;
            }
            yield return null;
        }
        if (isFadeIn) _sr.gameObject.SetActive(false);
    }
    public void FadeOut()
    {
        SetPosition();
        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");
    }
    public void FadeIn()
    {
        SetPosition();
        DOTween.Rewind("fader_in");
        DOTween.Play("fader_in");
    }
    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, 0);
    }
    public void SetPosition()
    {
        transform.position = UiCanvasTransform.position;
    }
}
