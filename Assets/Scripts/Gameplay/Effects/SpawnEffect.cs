using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    [SerializeField] private List<GameObject> _effects;
    [SerializeField] private Transform _spawnPoint;
    void Spawn(int index)
    {
        if (_spawnPoint == null)
            _spawnPoint = transform;

        if (index < _effects.Count)
            Instantiate(_effects[index], _spawnPoint);
    }
}
