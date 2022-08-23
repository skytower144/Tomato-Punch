using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EssentialLoader : MonoBehaviour
{
    [SerializeField] private GameObject essentialPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
            Instantiate(essentialPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
