using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CheckBattleResult : MonoBehaviour
{
    private TextSpawn script_textSpawn;
    private void WinOrLose()
    {
        InitializeTextSpawn();

        if (tomatoControl.isVictory){
            script_textSpawn.PlayVictory_Player();
        }
        else if (tomatoControl.isFainted){
            script_textSpawn.PlayDefeated_Player();
        }

        Destroy(gameObject);
    }

    private void InitializeTextSpawn()
    {
        script_textSpawn = transform.parent.GetComponent<TextSpawn>();
    }
}
