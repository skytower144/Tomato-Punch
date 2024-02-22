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
        public ActDetailWithAnim(int animIndex, string name, int percentage)
        {
            AnimIndex = animIndex;
            Name = name;
            Percentage = percentage;
        }
    }
    public List<Animator> AnimList;
    public List<SpriteRenderer> SrList;
    public List<ActDetailWithAnim> Attacks;
    public DuplicateRenderer DuplicateRd;

    [System.NonSerialized] public GangParry CurrentState;
    [System.NonSerialized] public GameObject CurrentGuy, CurrentProjectile;
    [System.NonSerialized] public bool IsAction = false;
    [System.NonSerialized] public int ParriedPjIndex = 0;
    [System.NonSerialized] public int ColorIndex;

    [SerializeField] private GameObject _getReadyText, _hitEffect;
    [SerializeField] private List<GameObject> _colliders, _parriedAnimations, _parriedPjs;

    private List<bool> isDeadList = new List<bool>();
    private Transform _attackBoxSpawn;
    [SerializeField] private int _maxPercent = 100;
    private int _savedIndex;

    private Color _invisibleColor = new Color(255, 255, 255, 0f);
    private Color _visibleColor = new Color(255, 255, 255, 1f);

    void OnEnable() {
        Instantiate(_getReadyText, GameManager.gm_instance.battle_system.textSpawn.transform);
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
        }
    }
    private void EnableAction()
    {
        IsAction = false;
    }
    private void InitIsDeadList()
    {
        for (int i = 0; i < AnimList.Count; i++)
            isDeadList.Add(false);
    }
    public void MarkDead(int index)
    {
        isDeadList[index] = true;
    }
    public bool IsDead(int index)
    {
        return isDeadList[index];
    }
    public void RemoveAttacks(int index)
    {
        for (int i = Attacks.Count - 1; i >= 0; i--) {
            if (Attacks[i].AnimIndex == index) {
                SetMaxPercent(-Attacks[i].Percentage);
                Attacks.RemoveAt(i);
            }
        }
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
}