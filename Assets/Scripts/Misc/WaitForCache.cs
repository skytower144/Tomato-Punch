using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCache
{
    public static readonly Dictionary<float, WaitForSeconds> WaitDict = new Dictionary<float, WaitForSeconds>();
    public static readonly Dictionary<float, WaitForSecondsRealtime> WaitRealDict = new Dictionary<float, WaitForSecondsRealtime>();
    private static WaitForSeconds _waitSeconds;
    private static WaitForSecondsRealtime _waitRealSeconds;


    public static WaitForSeconds GetWaitForSecond(float seconds)
    {
        if (!WaitDict.TryGetValue(seconds, out _waitSeconds)) {
            if (WaitDict.Count > 40)
                WaitDict.Clear();
            WaitDict[seconds] = new WaitForSeconds(seconds);
        }
        return WaitDict[seconds];
    }

    public static WaitForSecondsRealtime GetWaitForSecondReal(float seconds)
    {
        if (!WaitRealDict.TryGetValue(seconds, out _waitRealSeconds)) {
            if (WaitRealDict.Count > 30)
                WaitRealDict.Clear();
            WaitRealDict[seconds] = new WaitForSecondsRealtime(seconds);
        }
        return WaitRealDict[seconds];
    }
}
