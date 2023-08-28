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
    [SerializeField] private Animator _anim;
    private Dictionary<string, (float, float)> _fpsDict = new Dictionary<string, (float, float)>();
    private string[] _invokeMethods = {
        "enemyCounterStart", "enemyCounterEnd", "hitFrame", "actionOver",
        "EnableDunk", "DisableDunk", "Bounce", "DunkBounceSmoke",
        "BlastShrink", "RecoverShrink", "RecoverAnimation", "actionOver",
        "enemy_isPunchedEnd", "hurtOver"
    };

    void Start()
    {
        _enemyControl = GameManager.gm_instance.battle_system.enemy_control;
    }

    void OnDisable()
    {
        _fpsDict.Clear();
    }

    public void InitFrameDict(Animator anim)
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
            _fpsDict[clip.name] = (clip.frameRate, clip.length);
    }

    public void Act(string animName, BattleActType actType)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        switch (actType) {
            case BattleActType.Recover:
            case BattleActType.Guard:
            case BattleActType.ReEngage:
                _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Intro:
                _enemyControl.Invoke("enemyIntroOver", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Defeated:
                _enemyControl.Invoke("freezeAnimation", 1 / _fpsDict[animName].Item1);
                break;

            case BattleActType.Hurt:
                float frameDelay = 1 / _fpsDict[animName].Item1;
                _enemyControl.Invoke("enemy_isPunchedEnd", _fpsDict[animName].Item2 - frameDelay);
                _enemyControl.Invoke("hurtOver", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Uppered:
                _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Knockback:
                _enemyControl.Invoke("DetermineCC", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Bounce:
                _enemyControl.DunkBounceSmoke2();
                _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Blast:
                _enemyControl.WallHitEffect();
                _enemyControl.Invoke("EnableDunk", 1 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("BlastShrink", 0.08f);
                _enemyControl.Invoke("RecoverShrink", 0.182f);
                _enemyControl.Invoke("DisableDunk", 5 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("Bounce", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Dunk:
                _enemyControl.Invoke("DunkBounceSmoke", 1 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("Bounce", _fpsDict[animName].Item2);
                break;
            
            default:
                break;
        }
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

public enum BattleActType {
    None, Intro, Defeated, Knockback, Suffer,
    Stun, Uppered, Recover, Guard, Hurt,
    Wait, ReEngage, Victory, Blast, Bounce,
    Dunk
}