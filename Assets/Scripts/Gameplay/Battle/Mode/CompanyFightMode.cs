using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyFightMode : MonoBehaviour
{
    [SerializeField] private GameObject _employees, _getReadyText;
    private GameObject _temp;

    void Start() {
        GameManager.gm_instance.battle_system.IsGangfight = true;
        _temp = Instantiate(_employees, GameManager.gm_instance.battle_system.enemy_control.transform);
    }
    void OnEnable() {
        Instantiate(_getReadyText, transform.parent);
    }

    void OnDisable() {
        GameManager.gm_instance.battle_system.IsGangfight = false;
        Destroy(_temp);
        Destroy(gameObject);
    }
}
