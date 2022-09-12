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
        game_data.int_list.Add(testNumber);
        game_data.float_list.Add(position);
        return game_data;
    }

    public void Restore(ProgressData game_data)
    {
        testNumber = game_data.int_list[0];
        position = game_data.float_list[0];
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
