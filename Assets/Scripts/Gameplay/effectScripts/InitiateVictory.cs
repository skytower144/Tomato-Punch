using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateVictory : MonoBehaviour
{
    public TextSpawn script_textSpawn;
    private void PlayVictory()
    {
        tomatoControl.isVictory = true;
        script_textSpawn.PlayVictory_Player();
    }
}
