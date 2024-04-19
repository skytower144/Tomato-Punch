using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBools
{
    public static void DebugFunctions()
    {
        if (!GameManager.gm_instance.EnableDebug) return;

        // Kill Mato
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            GameManager.gm_instance.battle_system.tomato_hurt.TakeDamage(5000); 
        }

        // Kill Enemy
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            GameManager.gm_instance.battle_system.enemy_control.enemy_hurt.enemyHurtDamage(5000);
            GameManager.gm_instance.battle_system.enemy_control.enemy_hurt.checkDefeat();
        }
        
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            Debug.Log($"isAction : {GameManager.gm_instance.battle_system.tomato_control.IsAction}");
            Debug.Log($"isPunch : {GameManager.gm_instance.battle_system.tomato_control.IsPunch}");
            Debug.Log($"guardRelease : {GameManager.gm_instance.battle_system.tomato_control.GuardRelease}");
            Debug.Log($"isGatle : {tomatoControl.isGatle}");
            Debug.Log($"isGuard : {tomatoControl.isGuard}");
            Debug.Log($"isParry : {tomatoGuard.isParry}");
            Debug.Log($"isMiss : {GameManager.gm_instance.battle_system.tomato_control.isMiss}");

            Debug.Log($"stopGatle : {GameManager.gm_instance.battle_system.battleUI_Control.stopGatle}");

            Debug.Log($"isIntro : {tomatoControl.isIntro}");
            Debug.Log($"isVictory : {tomatoControl.isVictory}");
            Debug.Log($"isFainted : {tomatoControl.isFainted}");

            Debug.Log($"gatleButton_once : {tomatoControl.gatleButton_once}");
            Debug.Log($"uppercutYes : {tomatoControl.uppercutYes}");
            Debug.Log($"enemyUppered : {tomatoControl.enemyUppered}");
            Debug.Log($"enemyFreeze : {tomatoControl.enemyFreeze}");
            Debug.Log($"tomato - enemy_supered : {GameManager.gm_instance.battle_system.tomato_control.enemy_supered}");
            Debug.Log($"isTired : {GameManager.gm_instance.battle_system.tomato_control.IsTired}");

            Debug.Log($"preventDamageOverlap : {tomatoGuard.preventDamageOverlap}");
            Debug.Log($"isHurt : {tomatoHurt.isTomatoHurt}");

            Debug.Log($"enemy_isIntro : {EnemyAIControl.enemy_isIntro}");
            Debug.Log($"enemy_isParried : {Enemy_parried.isParried}");
            Debug.Log($"pjParried : {Enemy_parried.pjParried}");
            Debug.Log($"enemy_isCountered : {Enemy_countered.enemy_isCountered}");
            Debug.Log($"enemy_isPunched : {Enemy_is_hurt.enemy_isPunched}");
            Debug.Log($"enemy_isHit : {Enemy_is_hurt.enemyIsHit}");
            Debug.Log($"enemy_isDefeated : {Enemy_is_hurt.enemy_isDefeated}");
            Debug.Log($"enemy - isGuarding : {GameManager.gm_instance.battle_system.enemy_control.enemy_hurt.isGuarding}");
            Debug.Log($"enemy - guardUp : {GameManager.gm_instance.battle_system.enemy_control.enemy_hurt.guardUp}");
            Debug.Log($"enemy - isRecovering : {GameManager.gm_instance.battle_system.enemy_control.isRecovering}");
            Debug.Log($"enemy - enemy_supered : {GameManager.gm_instance.battle_system.enemy_control.enemy_supered}");
            Debug.Log($"isBlast : {GameManager.gm_instance.assistManager.isBlast}");
        }
    }
}
