using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GangFightMode : MonoBehaviour
{
    [SerializeField] private GameObject _getReadyText;
    [SerializeField] private List<ActDetailWithAnim> _attacks;
    [SerializeField] private List<GameObject> _colliders;
    [SerializeField] private List<GameObject> _parriedAnimations;
    private Transform _attackBoxSpawn;

    public enum GangParry { Physical, Projectile }
    [System.NonSerialized] public GangParry CurrentState;
    [System.NonSerialized] public GameObject CurrentGuy;
    [System.NonSerialized] public bool IsAction = false;

    private int _maxPercent = 100;
    private int _savedIndex;

    [System.Serializable]
    public class ActDetailWithAnim
    {
        public Animator Anim;
        public string Name;
        public int Percentage;
    }
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

        for (int i = 0; i < _attacks.Count; i++)
        {
            sumPercent += _attacks[i].Percentage;

            if (sumPercent >= randomPercent) {
                _attacks[i].Anim.Play(_attacks[i].Name);
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

            default:
                break;
        }
    }
    public void SpawnParriedAnimation()
    {
        CurrentGuy.SetActive(false);
        _maxPercent -= _attacks[_savedIndex].Percentage;
        _attacks.RemoveAt(_savedIndex);
        Invoke("EnableAction", 0.5f);

        switch (CurrentState) {
            case GangParry.Physical:
                Instantiate(_parriedAnimations[Random.Range(0, 3)], transform.parent);
                break;

            case GangParry.Projectile:
                Instantiate(_parriedAnimations[3], transform.parent);
                break; 
        }
    }
    private void EnableAction()
    {
        IsAction = false;       
    }
}
