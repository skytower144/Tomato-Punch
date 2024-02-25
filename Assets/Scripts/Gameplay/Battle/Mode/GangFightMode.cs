using System.Collections.Generic;
using UnityEngine;

public class GangFightMode : MonoBehaviour
{
    public enum GangParry { Physical, Projectile }

    [System.Serializable]
    public class ActDetailWithAnim
    {
        public int AnimIndex;
        public string Name;
        public int Percentage;
        public int AttackDamage;
        public ActDetailWithAnim(int animIndex, string name, int percentage, int damage)
        {
            AnimIndex = animIndex;
            Name = name;
            Percentage = percentage;
            AttackDamage = damage;
        }
    }
    public List<Animator> AnimList;
    public List<string> VictoryAnimNames;
    public List<SpriteRenderer> SrList;
    public List<ActDetailWithAnim> Attacks;
    public int DefaultDamage;
    public DuplicateRenderer DuplicateRd;

    [System.NonSerialized] public GangParry CurrentState;
    [System.NonSerialized] public GameObject CurrentGuy, CurrentProjectile;
    [System.NonSerialized] public bool IsAction = false;
    [System.NonSerialized] public int ParriedPjIndex = 0;
    [System.NonSerialized] public int ColorIndex;

    [SerializeField] private GameObject _getReadyText, _hitEffect, _puffEffect;
    [SerializeField] private List<GameObject> _colliders, _parriedAnimations, _parriedPjs;

    private List<ActDetailWithAnim> _backupAttacks = new List<ActDetailWithAnim>();
    private List<bool> _isDeadList = new List<bool>();
    private Transform _attackBoxSpawn;
    private int _maxPercent = 100;
    private int _savedIndex;

    private Color _invisibleColor = new Color(255, 255, 255, 0f);
    private Color _visibleColor = new Color(255, 255, 255, 1f);

    void OnEnable() {
        Instantiate(_getReadyText, GameManager.gm_instance.battle_system.textSpawn.transform);
        BackupAttacks();
        InitIsDeadList();

        GameManager.gm_instance.battle_system.gangFightMode = this;
        GameManager.gm_instance.battle_system.IsGangfight = true;

        _attackBoxSpawn = GameManager.gm_instance.battle_system.enemy_control.AttackBoxSpawn;

        InvokeRepeating("Attack", 1f, 1f);
    }
    void OnDisable() {
        GameManager.gm_instance.battle_system.IsGangfight = false;
        CancelInvoke();

        Destroy(gameObject);
    }
    private void Attack()
    {
        if (IsAction) return;
        if (!GameManager.gm_instance.battle_system.enemy_control.enemyAiControl.ShouldActivate()) return;

        int sumPercent = 0;
        int randomPercent = Random.Range(1, _maxPercent + 1);

        for (int i = 0; i < Attacks.Count; i++)
        {
            sumPercent += Attacks[i].Percentage;

            if (sumPercent >= randomPercent) {
                AnimList[Attacks[i].AnimIndex].Play(Attacks[i].Name);
                GameManager.gm_instance.battle_system.enemy_control.gangFightDmg = Attacks[i].AttackDamage;
                _savedIndex = i;
                IsAction = true;
                return;
            }
        }
    }
    public void SpawnAttackBox(string type)
    {
        switch (type) {
            case "LEFT":
                Instantiate(_colliders[0], _attackBoxSpawn);
                break;

            case "RIGHT":
                Instantiate(_colliders[1], _attackBoxSpawn);
                break;       

            case "DOWN":
                Instantiate(_colliders[2], _attackBoxSpawn);
                break;
            
            case "PJ":
                Instantiate(_colliders[3], _attackBoxSpawn);
                break;

            default:
                break;
        }
    }
    public void SpawnParriedAnimation()
    {
        switch (CurrentState) {
            case GangParry.Physical:
                CurrentGuy.SetActive(false);
                MarkDead(Attacks[_savedIndex].AnimIndex);

                Instantiate(_parriedAnimations[Random.Range(0, 3)], transform.parent);
                Invoke("EnableAction", 0.5f);
                break;

            case GangParry.Projectile:
                Destroy(CurrentProjectile);
                Instantiate(_parriedPjs[ParriedPjIndex], transform.parent);
                break;
        }
        RemoveCurrentAttack();
    }
    public void RemoveCurrentAttack()
    {
        if (_savedIndex != -1) {
            SetMaxPercent(-Attacks[_savedIndex].Percentage);
            Attacks.RemoveAt(_savedIndex);
            _savedIndex = -1;

            CheckBattleOver();
        }
    }
    private void EnableAction()
    {
        IsAction = false;
    }
    private void InitIsDeadList()
    {
        _isDeadList.Clear();

        for (int i = 0; i < AnimList.Count; i++)
            _isDeadList.Add(false);
    }
    private void BackupAttacks()
    {
        for (int i = 0; i < Attacks.Count; i++) {
            _backupAttacks.Add(new ActDetailWithAnim(
                Attacks[i].AnimIndex,
                Attacks[i].Name,
                Attacks[i].Percentage,
                Attacks[i].AttackDamage
            ));
        }
    }
    public void RestoreAttacks()
    {
        Attacks.Clear();
        for (int i = 0; i < _backupAttacks.Count; i++) {
            Attacks.Add(new ActDetailWithAnim(
                _backupAttacks[i].AnimIndex,
                _backupAttacks[i].Name,
                _backupAttacks[i].Percentage,
                _backupAttacks[i].AttackDamage
            ));
        }
        _maxPercent = 100;
    }
    public void MarkDead(int index)
    {
        _isDeadList[index] = true;
    }
    public void ReviveDead()
    {
        Instantiate(_puffEffect, transform);

        for (int i = 0; i < _isDeadList.Count; i++) {
            _isDeadList[i] = false;
            AnimList[i].gameObject.SetActive(false);
            AnimList[i].gameObject.SetActive(true);
        }
    }
    public bool IsDead(int index)
    {
        return _isDeadList[index];
    }
    public void RemoveAttacks(int index)
    {
        for (int i = Attacks.Count - 1; i >= 0; i--) {
            if (Attacks[i].AnimIndex == index) {
                SetMaxPercent(-Attacks[i].Percentage);
                Attacks.RemoveAt(i);
            }
        }
        CheckBattleOver();
    }
    public void SetMaxPercent(int amount)
    {
        _maxPercent += amount;
    }
    public void HideTarget(int index)
    {
        SrList[index].color = _invisibleColor;
    }
    public void ShowTarget()
    {
        SrList[ColorIndex].color = _visibleColor;
    }
    private void CheckBattleOver()
    {
        if (Attacks.Count == 0)
            GameManager.gm_instance.battle_system.enemy_control.enemy_hurt.checkDefeat();
        // Connect HP Bar
    }
    public void PlayVictoryAnimation()
    {
        for (int i = 0; i < AnimList.Count; i++) {
            if (!_isDeadList[i])
                AnimList[i].Play(VictoryAnimNames[i], -1, 0f);
        }
    }
}