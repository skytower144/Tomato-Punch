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
    private BattleSystem _battleSystem;
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
        _battleSystem = GameManager.gm_instance.battle_system;
        _enemyControl = _battleSystem.enemy_control;
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
    public int GetTotalActionFrames(string name)
    {
        return (int)(_fpsDict[name].Item1 * _fpsDict[name].Item2); 
    }
    public void Idle(string animName, bool noDelay = true)
    {
        CancelScheduledInvokes();
        if (noDelay) _anim.Play(animName, -1, 0f);
        else _anim.Play(animName);

        _enemyControl.disableBools();
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

            case BattleActType.Angry:
                // _battleSystem.tomato_control.tomatoAnim.Play("tomato_intro", -1, 0f);
                _battleSystem.Invoke("ResumeBattle", _fpsDict[animName].Item2);
                _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.ReEngage:
            case BattleActType.Recover:
                if (_battleSystem.IsNextPhase)
                    _enemyControl.Invoke("Angry", _fpsDict[animName].Item2);
                else
                    _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Guard:
                GameManager.gm_instance.assistManager.SetIsBlast(false);
                StartCoroutine(SetCollider(true));
                _enemyControl.Invoke("actionOver", _fpsDict[animName].Item2);
                return;
            
            case BattleActType.Intro:
                _enemyControl.Invoke("enemyIntroOver", _fpsDict[animName].Item2 + 0.05f);
                break;
            
            case BattleActType.Defeated:
                _enemyControl.CancelCounterState();
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
                _enemyControl.duplicate_r.InitFlash();
                StartCoroutine(SetCollider(true));
                return;
            
            case BattleActType.Uppered:
                StartCoroutine(_enemyControl.duplicate_r.BlinkEffect(4, 0.02f));
                _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Knockback:
                StartCoroutine(SetCollider(true, 0.1f));

                if (Enemy_countered.enemy_isCountered) {
                    _enemyControl.duplicate_r.InitFlash();
                    StartCoroutine(_enemyControl.duplicate_r.BlinkEffect(3, 0.05f));
                }
                else if (Enemy_parried.isParried)
                    _enemyControl.enemyHurtFlash(0.3f);

                _enemyControl.Invoke("DetermineCC", _fpsDict[animName].Item2);
                return;
            
            case BattleActType.Bounce:
                _enemyControl.duplicate_r.InitFlash();
                _enemyControl.DunkBounceSmoke2();
                _enemyControl.Invoke("RecoverAnimation", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Blast:
                if (!_battleSystem.IsNextPhase) {
                    _enemyControl.Invoke("EnableDunk", 1 / _fpsDict[animName].Item1);
                    _enemyControl.Invoke("DisableDunk", 5 / _fpsDict[animName].Item1);
                }
                _enemyControl.CancelCounterState();
                _enemyControl.WallHitEffect();
                _enemyControl.Invoke("BlastShrink", 0.9f / _fpsDict[animName].Item1);
                _enemyControl.Invoke("RecoverShrink", 2 / _fpsDict[animName].Item1);
                _enemyControl.Invoke("Bounce", _fpsDict[animName].Item2);
                break;
            
            case BattleActType.Dunk:
                _enemyControl.duplicate_r.InitFlash();
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
            CurrentHitType = frame.HitType;

            switch (frame.EnemyAttackType) {
                case AttackType.LA:
                case AttackType.RA:
                case AttackType.DA:
                case AttackType.GuardOrJump:
                    PhysicalAttackFrame attack = (PhysicalAttackFrame)frame;
                    if (attack.CounterStartFrame != -1) {
                        _enemyControl.Invoke("enemyCounterStart", attack.CounterStartFrame / _fpsDict[animName].Item1);
                        _enemyControl.Invoke("enemyCounterEnd", attack.CounterEndFrame / _fpsDict[animName].Item1);
                    }
                    SetColliderAfterHit(finishedAttack, attack.HitFrame, animName);
                    _enemyControl.Invoke("hitFrame", attack.HitFrame / _fpsDict[animName].Item1);
                    break;
                
                case AttackType.PJ:
                case AttackType.JumpPJ:
                    ProjectileAttackFrame pj = (ProjectileAttackFrame)frame;
                    _enemyControl.currentProjectile = pj.Projectile;
                    _enemyControl.Invoke("projectileSpawn", pj.SpawnFrame / _fpsDict[animName].Item1);

                    SetColliderAfterHit(finishedAttack, pj.SpawnFrame, animName);
                    break;
                
                default:
                    break;
            }
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

    // If Gangfight situation, animation states will act differently /////////////////////////////////////////////////
    public void Parried()
    {
        _battleSystem.gangFightMode.SpawnParriedAnimation();
    }
    public void ReEngage()
    {
        if (_battleSystem.IsGangfight) {
            _battleSystem.gangFightMode.RestoreAttacks();
            _battleSystem.gangFightMode.ReviveDead();
            return;
        }
        string enemyReEngage = _battleSystem.GetEnemyBase().ReEngage;
        Act(enemyReEngage, BattleActType.ReEngage);
    }
    public void Victory()
    {
        if (_battleSystem.IsGangfight) {
            _battleSystem.gangFightMode.PlayVictoryAnimation();
            return;
        }
        string victoryAnimation = _battleSystem.GetEnemyBase().Victory;
        Act(victoryAnimation, BattleActType.Victory);
    }
    public void Defeated()
    {
        if (_battleSystem.IsGangfight) {
            _enemyControl.UnFreeze();
            return;
        }
        string defeatedAnimation = _battleSystem.GetEnemyBase().Defeated_AnimationString;
        Act(defeatedAnimation, BattleActType.Defeated);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}

public enum BattleActType {
    None,
    Intro,      Defeated,   Knockback,  Suffer, Stun,
    Uppered,    Recover,    Guard,      Hurt,   Wait,
    ReEngage,   Victory,    Blast,      Bounce, Dunk,
    Angry
}