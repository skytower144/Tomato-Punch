using System;
using UnityEngine;

public class EnemyGimmicks : MonoBehaviour
{
    private tomatoControl _tomatoControl;
    private EnemyControl _enemyControl;
    private EnemyAnimControl _enemyAnimControl;
    private Action _loadedAction;
    void Start()
    {
        _tomatoControl = GameManager.gm_instance.battle_system.tomato_control;
        _enemyControl = GameManager.gm_instance.battle_system.enemy_control;
        _enemyAnimControl = _enemyControl.enemyAnimControl;
    }

    void OnDisable()
    {
        _loadedAction = null;
    }

    public void InvokeLoadedAction()
    {
        _loadedAction?.Invoke();
    }

// GIMMICKS ////////////////////////////////////////////////////////////////////////////////////////////////////////
    void Load_Donut_AbsorbAttack()
    {
        _loadedAction = Play_Donut_AbsorbAttack;
    }
    
    void Play_Donut_AbsorbAttack()
    {
        _enemyAnimControl.CancelScheduledInvokes();

        string animName = "Donut_attack_2_absorb";
        float attackFinishedTime = 5 / _enemyAnimControl.FpsDict[animName].Item1;

        _enemyControl.enemyAnim.Play(animName, -1, 0f);
        _tomatoControl.tomatoAnim.Play("tomato_absorbed", -1, 0f);
        _tomatoControl.gaksung_OFF();
        _tomatoControl.tomatoHurtStart();

        StartCoroutine(_tomatoControl.ChangeAnimationState("tomato_D_hurt", attackFinishedTime));
        StartCoroutine(_enemyAnimControl.SetCollider(true, attackFinishedTime));
        _enemyControl.Invoke("actionOver", _enemyAnimControl.FpsDict[animName].Item2);
    }

}
