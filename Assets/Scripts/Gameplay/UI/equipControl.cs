using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class equipControl : MonoBehaviour
{
    [SerializeField] private GameObject super_Parent, normal_Parent;
    void OnDisable()
    {
        super_Parent.SetActive(false);
        normal_Parent.SetActive(true);
    }
}
