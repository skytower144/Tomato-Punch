using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitbox : MonoBehaviour
{
    void SpawnHitBox(int index) {
        GameManager.gm_instance.battle_system.enemy_control.pjPropIndex = index;
        GameManager.gm_instance.battle_system.enemy_control.hitFrame();
    }
}
