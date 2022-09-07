using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour, ObjectProgress
{
    [SerializeField] private int testNumber;
    public object Capture()
    {
        return testNumber;
    }

    public void Restore(object state)
    {
        testNumber = (int)state;
    }

    public string ReturnID()
    {
        return this.GetType().Name;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            testNumber = 100;
        }
    }
}
