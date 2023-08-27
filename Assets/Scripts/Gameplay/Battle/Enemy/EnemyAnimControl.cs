using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimControl : MonoBehaviour
{
    /// <summary>
    /// This system is built on the principle that almost all animation frames must be consistent across all enemies.
    /// ATTACKS, UNIQUE BEHAVIOURS are excluded from the principle.
    /// For example, Blast animation must ALWAYS be total 7 frames.
    /// </summary>

    private EnemyControl _enemyControl;
    private Animator _anim;
    private Dictionary<string, (float, float)> _fpsDict = new Dictionary<string, (float, float)>();
    private string[] _invokeMethods = {
        "enemyCounterStart", "enemyCounterEnd", "hitFrame", "actionOver",
        "EnableDunk", "DisableDunk", "Bounce", "DunkBounceSmoke",
        "BlastShrink", "RecoverShrink", "RecoverAnimation", "actionOver"
    };

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

    public void SimpleAct(string animName)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
    }

    public void Intro(string animName)
    {
        _anim.Play(animName, -1, 0f);
        _enemyControl.Invoke("enemyIntroOver", _fpsDict[animName].Item2);
    }

    public void KO(string animName)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        _enemyControl.Invoke("freezeAnimation", 1 / _fpsDict[animName].Item1);
    }

    public void Uppered(string animName)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
    }

    public void KnockBack(string animName)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        _enemyControl.Invoke("DetermineCC", _fpsDict[animName].Item2);
    }

    public void Dunk(string animName)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        _enemyControl.Invoke("DunkBounceSmoke", 1 / _fpsDict[animName].Item1);
        _enemyControl.Invoke("Bounce", _fpsDict[animName].Item2);
    }

    public void Bounce(string animName)
    {
        _anim.Play(animName, -1, 0f);

        _enemyControl.DunkBounceSmoke2();
        _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
    }

    public void Blast(string animName)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        _enemyControl.WallHitEffect();
        _enemyControl.Invoke("EnableDunk", 1 / _fpsDict[animName].Item1);
        _enemyControl.Invoke("BlastShrink", 0.08f);
        _enemyControl.Invoke("RecoverShrink", 0.182f);
        _enemyControl.Invoke("DisableDunk", 5 / _fpsDict[animName].Item1);
        _enemyControl.Invoke("Bounce", _fpsDict[animName].Item2);
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

    public void CancelScheduledInvokes()
    {
        foreach (string methodName in _invokeMethods)
            _enemyControl.CancelInvoke(methodName);
    }
}