using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScroll : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    void OnEnable()
    {
        contentTransform.localPosition = new Vector3(0, contentTransform.transform.position.y);
    }
}
