using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimControl : MonoBehaviour
{
    /// <summary>
    /// This system is built on the principle that almost all animation frames must be consistent across all enemies.
    /// ATTACKS, UNIQUE BEHAVIOURS are exempt from the principle.
    /// For example, Blast animation must ALWAYS be total 7 frames.
    /// </summary>

    private Animator _anim;
    [SerializeField] private BoxCollider2D _collider;
    private EnemyControl _enemyControl;
    private Enemy_is_hurt _enemyHurt;
    private Dictionary<string, (float, float)> _fpsDict = new Dictionary<string, (float, float)>();
    private string[] _invokeMethods = {
        "enemyCounterStart", "enemyCounterEnd", "hitFrame", "actionOver",
        "EnableDunk", "DisableDunk", "Bounce", "DunkBounceSmoke",
        "BlastShrink", "RecoverShrink", "RecoverAnimation",
        "enemy_isPunchedEnd", "hurtOver", "projectileSpawn"
    };
    public Dictionary<string, (float, float)> FpsDict => _fpsDict;
    public HitType CurrentHitType { private set; get; }

    void Start()
    {
        _enemyControl = GameManager.gm_instance.battle_system.enemy_control;
        _enemyHurt = _enemyControl.enemy_hurt;
    }

    void OnEnable()
    {
        if (!_collider.enabled) _collider.enabled = true;
    }

    void OnDisable()
    {
        _fpsDict.Clear();
        WaitForCache.WaitDict.Clear();
    }

    public void InitFrameDict(Animator anim)
    {
        _anim = anim;
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
            _fpsDict[clip.name] = (clip.frameRate, clip.length);
    }

    public void Idle(string animName, bool noDelay = true)
    {
        CancelScheduledInvokes();
        _enemyControl.disableBools();
        
        if (noDelay) _anim.Play(animName, -1, 0f);
        else _anim.Play(animName);
        StartCoroutine(SetCollider(true));
    }

    public void Act(string animName, BattleActType actType)
    {
        CancelScheduledInvokes();
        _anim.Play(animName, -1, 0f);

        switch (actType) {
            case BattleActType.Victory:
            case BattleActType.Wait:
                break;
            
            case BattleActType.Recover:
            case BattleActType.ReEngage:
                _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Guard:
                GameManager.gm_instance.assistManager.SetIsBlast(false);
                StartCoroutine(SetCollider(true));
                _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
                return;
            
            case BattleActType.Intro:
                _enemyControl.Invoke("enemyIntroOver", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Defeated:
                _enemyControl.Invoke("freezeAnimation", 1 / _fpsDict[animName].Item1);
                break;

            case BattleActType.Hurt:
                StartCoroutine(SetCollider(true));
                float frameDelay = 1 / _fpsDict[animName].Item1;
                _enemyControl.Invoke("enemy_isPunchedEnd", _fpsDict[animName].Item2 - frameDelay);
                _enemyControl.Invoke("hurtOver", _fpsDict[animName].Item2);
                return;
            
            case BattleActType.Stun:
            case BattleActType.Suffer:
                StartCoroutine(SetCollider(true));
                return;
            
            case BattleActType.Uppered:
                StartCoroutine(_enemyControl.duplicate_r.BlinkEffect(4, 0.02f));
                _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Knockback:
                StartCoroutine(SetCollider(true, 0.1f));
                _enemyControl.Invoke("DetermineCC", _fpsDict[animName].Item2);
                return;
            
            case BattleActType.Bounce:
                _enemyControl.DunkBounceSmoke2();
                _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Blast:
                GameManager.gm_instance.assistManager.SetIsBlast(false);
                _enemyControl.CancelCounterState();
                _enemyControl.WallHitEffect();
                _enemyControl.Invoke("EnableDunk", 1 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("BlastShrink", 0.9f / _fpsDict[animName].Item1);
                _enemyControl.Invoke("RecoverShrink", 2 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("DisableDunk", 5 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("Bounce", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Dunk:
                StartCoroutine(_enemyControl.duplicate_r.BlinkEffect(6, 0.02f));
                _enemyControl.Invoke("DunkBounceSmoke", 1 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("Bounce", _fpsDict[animName].Item2);
                break;
            
            default:
                break;
        }
        StartCoroutine(SetCollider(false));
    }

    public void Act(EnemyActDetail actDetail)
    {
        CancelScheduledInvokes();
        string animName = actDetail.Name;
        CurrentHitType = actDetail.EnemyHitType;

        if (actDetail is Enemy_IdleDetail) {
            Idle(animName, false);
            return;
        }
        else if (actDetail is Enemy_NeutralDetail) {
            // do something
            return;
        }
        int actionCount = 0;

        foreach (DamageFrame frame in _enemyControl.TotalDamageFrames) {
            actionCount++;
            bool finishedAttack = actionCount == _enemyControl.TotalDamageFrames.Count;

            switch (frame.EnemyAttackType) {
                case AttackType.LA:
                case AttackType.RA:
                case AttackType.DA:
                    PhysicalAttackFrame attack = (PhysicalAttackFrame)frame;
                    _enemyControl.Invoke("enemyCounterStart", attack.CounterStartFrame / _fpsDict[animName].Item1);
                    _enemyControl.Invoke("enemyCounterEnd", attack.CounterEndFrame / _fpsDict[animName].Item1);
                    
                    SetColliderAfterHit(finishedAttack, attack.HitFrame, animName);
                    break;
                
                case AttackType.PJ:
                    ProjectileAttackFrame pj = (ProjectileAttackFrame)frame;
                    _enemyHurt.SetProjectileHit(true);
                    _enemyControl.currentProjectile = pj.Projectile;
                    _enemyControl.Invoke("projectileSpawn", pj.SpawnFrame / _fpsDict[animName].Item1);

                    SetColliderAfterHit(finishedAttack, pj.HitFrame, animName);
                    break;
                
                default:
                    break;
            }
            _enemyControl.Invoke("hitFrame", frame.HitFrame / _fpsDict[animName].Item1);
        }
        _anim.Play(animName);
        StartCoroutine(SetCollider(false));
        _enemyControl.guardDown();

        _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
    }

    public void CancelScheduledInvokes()
    {
        StopAllCoroutines();

        foreach (string methodName in _invokeMethods)
            _enemyControl.CancelInvoke(methodName);
    }
    private void SetColliderAfterHit(bool finishedAttack, int hitFrame, string animName)
    {
        if (finishedAttack) StartCoroutine(SetCollider(true, (hitFrame + 1)/ _fpsDict[animName].Item1));
    }
    public IEnumerator SetCollider(bool state, float wait = 0f)
    {
        yield return WaitForCache.GetWaitForSecond(wait);
        
        if (_collider.enabled == state) yield break;
        _collider.enabled = state;
    }
}

public enum BattleActType {
    None,
    Intro,      Defeated,   Knockback,  Suffer, Stun,
    Uppered,    Recover,    Guard,      Hurt,   Wait,
    ReEngage,   Victory,    Blast,      Bounce, Dunk
}