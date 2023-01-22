using UnityEngine;

public class ExitFightText : MonoBehaviour
{
    void UnleashCharacters()
    {
        if(!tomatoControl.isFainted){
            GameManager.gm_instance.battle_system.battleUI_Control.ShowBattleUI();
            tomatoControl.isIntro = false;
            EnemyAIControl.enemy_isIntro = false;
        }
        else{
            tomatoControl.isFainted = false;
        }

        Destroy(gameObject);
    }
}
