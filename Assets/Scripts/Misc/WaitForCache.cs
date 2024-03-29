using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCache
{
    public static readonly Dictionary<float, WaitForSeconds> WaitDict = new Dictionary<float, WaitForSeconds>();
    public static readonly Dictionary<float, WaitForSecondsRealtime> WaitRealDict = new Dictionary<float, WaitForSecondsRealtime>();
    private static WaitForSeconds _waitSeconds;
    private static WaitForSecondsRealtime _waitRealSeconds;

    public static readonly WaitForSeconds WaitSeconds0_1 = new WaitForSeconds(0.1f);
    public static readonly WaitForSeconds WaitSeconds0_5 = new WaitForSeconds(0.5f);
    public static readonly WaitForSeconds WaitSeconds0_8 = new WaitForSeconds(0.8f);
    public static readonly WaitForSeconds WaitSeconds1 = new WaitForSeconds(1f);
    public static readonly WaitForSeconds WaitSeconds1_5 = new WaitForSeconds(1.5f);

    public static WaitForSeconds GetWaitForSecond(float seconds)
    {
        if (!WaitDict.TryGetValue(seconds, out _waitSeconds))
            WaitDict[seconds] = new WaitForSeconds(seconds);
        return WaitDict[seconds];
    }

    public static WaitForSecondsRealtime GetWaitForSecondReal(float seconds)
    {
        if (!WaitRealDict.TryGetValue(seconds, out _waitRealSeconds))
            WaitRealDict[seconds] = new WaitForSecondsRealtime(seconds);
        return WaitRealDict[seconds];
    }
}
