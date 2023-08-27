using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimControl : MonoBehaviour
{
    private EnemyControl _enemyControl;
    private Animator _anim;
    private Dictionary<string, (float, float)> _fpsDict = new Dictionary<string, (float, float)>();

    void Start()
    {
        _enemyControl = GameManager.gm_instance.battle_system.enemy_control;
    }

    void OnDisable()
    {
        _anim = null;
        _fpsDict.Clear();
    }

    public void InitFrameDict(Animator anim)
    {
        _anim = anim;
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
            _fpsDict[clip.name] = (clip.frameRate, clip.length);
    }

    public void Attack(Enemy_AttackDetail attackDetail)
    {
        string animName = attackDetail.EnemyAttackName;
        _anim.Play(animName);

        if (attackDetail.EnemyAttackType is AttackType.NEUTRAL) return;
        AttackFrameInfo frameInfo = attackDetail.FrameInfo;

        _enemyControl.guardDown();
        _enemyControl.Invoke("enemyCounterStart", frameInfo.CounterStartFrame / _fpsDict[animName].Item1);
        _enemyControl.Invoke("enemyCounterEnd", frameInfo.CounterEndFrame / _fpsDict[animName].Item1);
        _enemyControl.Invoke("hitFrame", frameInfo.HitFrame / _fpsDict[animName].Item1);
        _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
    }

    public void CancelAttackInvokes()
    {
        _enemyControl.CancelInvoke("enemyCounterStart");
        _enemyControl.CancelInvoke("enemyCounterEnd");
        _enemyControl.CancelInvoke("hitFrame");
        _enemyControl.CancelInvoke("actionOver");
    }
}