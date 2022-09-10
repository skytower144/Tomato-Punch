using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour, ObjectProgress
{
    [SerializeField] private int testNumber;
    [SerializeField] private float position;
    public ProgressData Capture()
    {
        ProgressData game_data = new ProgressData();
        game_data.testNumber = testNumber;
        game_data.position = position;
        return game_data;
    }

    public void Restore(ProgressData game_data)
    {
        testNumber = game_data.testNumber;
        position = game_data.position;
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
