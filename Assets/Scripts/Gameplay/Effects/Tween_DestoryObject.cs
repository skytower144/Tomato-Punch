using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween_DestoryObject : MonoBehaviour
{
    public void destroyObject_AfterTween()
    {
        Destroy(gameObject);
    }
}
