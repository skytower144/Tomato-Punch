using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFightText : MonoBehaviour
{
    [SerializeField] private GameObject FightText;
    void OnDestroy()
    {
        Instantiate(FightText, gameObject.transform.parent);
    }
}
