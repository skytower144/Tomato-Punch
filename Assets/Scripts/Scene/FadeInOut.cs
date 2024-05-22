using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;

    public IEnumerator Fade(float duration, float delay, int fps, bool isFadeIn)
    {
        Color color = _sr.color;
        color.a = isFadeIn ? 1 : 0;
        _sr.color = color;

        if (duration == 0f) duration = 1f;
        if (fps <= 0) fps = 60;

        float frameRate = 1f / fps;
        float timer = 0f;
        int mode = isFadeIn ? -1 : 1;

        DialogueManager.instance.cutsceneHandler.SetCutscenePosition();
        _sr.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);

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
        DialogueManager.instance.cutsceneHandler.SetCutscenePosition();
        DOTween.Rewind("fader_out");
        DOTween.Play("fader_out");
    }
    public void FadeIn()
    {
        DialogueManager.instance.cutsceneHandler.SetCutscenePosition();
        DOTween.Rewind("fader_in");
        DOTween.Play("fader_in");
    }
}
