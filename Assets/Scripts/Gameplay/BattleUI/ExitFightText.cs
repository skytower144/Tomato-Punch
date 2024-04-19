using UnityEngine;

public class ExitFightText : MonoBehaviour
{
    void UnleashCharacters()
    {
        BattleSystem battleSystem = GameManager.gm_instance.battle_system;

        if (battleSystem.IsNextPhase)
            battleSystem.IsNextPhase = false;
        
        else if (tomatoControl.isIntro) {
            battleSystem.battleUI_Control.ShowBattleUI();
            tomatoControl.isIntro = false;
        }
        else if (tomatoControl.isFainted)
            tomatoControl.isFainted = false;
        
        EnemyAIControl.enemy_isIntro = false;
        Destroy(gameObject);
    }
}
