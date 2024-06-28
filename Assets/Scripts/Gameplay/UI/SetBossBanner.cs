using UnityEngine;
using DG.Tweening;
public class SetBossBanner : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private DOTweenAnimation _doTween;
    [SerializeField] private GameObject _vsText;
    void Start()
    {
        _sr.sprite = GameManager.gm_instance.battle_system.enemy_control._base.bossBanner;
    }

    void ShowText()
    {
        _vsText.SetActive(true);
        _doTween.DOPlay();
    }
}
