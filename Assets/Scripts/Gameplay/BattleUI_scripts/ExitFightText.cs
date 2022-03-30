using UnityEngine;
using DG.Tweening;
public class ExitFightText : MonoBehaviour
{
    void UnleashCharacters()
    {
        DOTween.Play("intro");
        tomatoControl.isIntro = false;
        EnemyAIControl.enemy_isIntro = false;
        Destroy(gameObject);
    }
}
