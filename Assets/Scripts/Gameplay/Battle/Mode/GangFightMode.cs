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
    public List<ActDetailWithAnim> Attacks;
    [System.NonSerialized] public GangParry CurrentState;
    [System.NonSerialized] public GameObject CurrentGuy, CurrentProjectile;
    [System.NonSerialized] public bool IsAction = false;
    [System.NonSerialized] public int ParriedPjIndex = 0;

    [SerializeField] private GameObject _getReadyText;
    [SerializeField] private List<GameObject> _colliders, _parriedAnimations, _parriedPjs;

    private Transform _attackBoxSpawn;
    private int _maxPercent = 100;
    private int _savedIndex;

    void OnEnable() {
        Instantiate(_getReadyText, GameManager.gm_instance.battle_system.textSpawn.transform);
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
        if (_savedIndex != -1) {
            _savedIndex = -1;
            _maxPercent -= Attacks[_savedIndex].Percentage;
            Attacks.RemoveAt(_savedIndex);
        }
        switch (CurrentState) {
            case GangParry.Physical:
                Instantiate(_parriedAnimations[Random.Range(0, 3)], transform.parent);
                Invoke("EnableAction", 0.5f);
                CurrentGuy.SetActive(false);
                break;

            case GangParry.Projectile:
                Destroy(CurrentProjectile);
                Instantiate(_parriedPjs[ParriedPjIndex], transform.parent);
                break;
        }
    }
    private void EnableAction()
    {
        IsAction = false;       
    }
}
