using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CheckBattleResult : MonoBehaviour
{
    public TextSpawn script_textSpawn;
    private void WinOrLose()
    {
        if (tomatoControl.isVictory){
            script_textSpawn.PlayVictory_Player();
        }
        else if (tomatoControl.isFainted){
            InitializeTextSpawn();
            script_textSpawn.PlayDefeated_Player();
        }

        Destroy(gameObject);
    }

    private void InitializeTextSpawn()
    {
        script_textSpawn = transform.parent.GetComponent<TextSpawn>();
    }
}
