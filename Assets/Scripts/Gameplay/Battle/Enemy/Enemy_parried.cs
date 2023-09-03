using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_parried : MonoBehaviour
{
    private Animator anim;
    private EnemyControl _enemyControl;
    private tomatoControl _tomatoControl;
    [SerializeField] private GameObject parryEffect, parryCircle;
    [HideInInspector] public static bool isParried = false;
    [HideInInspector] public static bool pjParried = false;

    void OnEnable()
    {
        _enemyControl = GameManager.gm_instance.battle_system.enemy_control;
        anim = _enemyControl.enemyAnim;
        _tomatoControl = GameManager.gm_instance.battle_system.tomato_control;
        isParried = pjParried = false;
    }
    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.tag.Equals("tomato_PRY"))
        {
            EnemyControl.totalParry += 1;

            Instantiate (parryEffect, new Vector2 (transform.position.x - 0.2f , transform.position.y - 1.3f), Quaternion.identity);
            GameManager.gm_instance.battle_system.enemy_control.EraseAllAttacks();
            GameManager.gm_instance.battle_system.tomato_control.guard_bar.RestoreGuardBar();

            if(EnemyControl.isPhysical)    // if isParried True -> enemy cannot move
            {
                isParried = true;
                Instantiate (parryCircle, new Vector2 (transform.position.x - 0.2f , transform.position.y - 1.3f), Quaternion.identity);
                _enemyControl.enemyAnimControl.Act(_enemyControl._base.Knockback_AnimationString, BattleActType.Knockback);
            }
            else
            {
                Enemy_parried.pjParried = true;
                _enemyControl.DestroyProjectiles();
                _tomatoControl.StartCoroutine(_tomatoControl.SetDeflectLaser(_tomatoControl.deflectLaser, true));
                Invoke("TurnOffPjParried", 0.1f);
            }
        }
    }

    private void TurnOffPjParried()
    {
        Enemy_parried.pjParried = false;
    }
}

