using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitbox : MonoBehaviour
{
    void SpawnHitBox() {
        GameManager.gm_instance.battle_system.enemy_control.hitFrame();
    }
    void SpawnProp(int index) {
        GameManager.gm_instance.battle_system.enemy_control.SpawnProp(index);
    }
}
