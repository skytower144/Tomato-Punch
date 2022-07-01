using UnityEngine;
using DG.Tweening;
public class ExitFightText : MonoBehaviour
{
    void UnleashCharacters()
    {
        if(!tomatoControl.isFainted){
            DOTween.Play("intro");
            tomatoControl.isIntro = false;
            EnemyAIControl.enemy_isIntro = false;
        }
        else{
            tomatoControl.isFainted = false;
        }

        Destroy(gameObject);
    }
}
