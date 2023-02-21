using UnityEngine;
using DG.Tweening;

public class QuickSave : MonoBehaviour
{
    [SerializeField] ParticleSystem sparkleEffect;
    private bool isQuickSaved = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isQuickSaved && (collision.tag == "Player"))
        {
            isQuickSaved = true;
            Invoke("EnableQuickSave", 2f);
            SparkleEffect();
            PopupEffect();
            GameManager.gm_instance.save_load_menu.ProceedSave(3);
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    private void EnableQuickSave()
    {
        isQuickSaved = false;
    }

    private void SparkleEffect()
    {
        var sparkle_em = sparkleEffect.emission;
        sparkle_em.enabled = true;
        sparkleEffect.Play();
    }

    private void PopupEffect()
    {
        DOTween.Rewind("savecat_popup");
        DOTween.Play("savecat_popup");
    }
}
