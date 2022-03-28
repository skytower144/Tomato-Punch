using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExitFightText : MonoBehaviour
{
    void UnleashCharacters()
    {
        tomatoControl.isIntro = false;
        EnemyAIControl.enemy_isIntro = false;
        Destroy(gameObject);
    }
}
