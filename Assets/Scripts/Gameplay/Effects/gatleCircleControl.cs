using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gatleCircleControl : MonoBehaviour
{
    [HideInInspector] public static bool failUppercut = false;
    [HideInInspector] public static bool uppercut_time = false;

    void Update()
    {
        
    }
    void gatleDisappear()
    {
        failUppercut = true;
    }

    void uppercutTime_true()
    {
        Invoke("uppercutTime_false", 0.13f);
        uppercut_time = true;
    }

    void uppercutTime_false()
    {
        uppercut_time = false;
    }

}
