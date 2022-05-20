using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_OnDisable : MonoBehaviour
{
    void OnDisable()
    {
        Destroy(gameObject);
    }
}
