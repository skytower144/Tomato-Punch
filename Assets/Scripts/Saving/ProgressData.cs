using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressData
{
    public string InkFileName, AnimationState;
    public bool IsVisible;
    public Vector2 Position;
    public List<KeyEventProgressData> KeyEventList;
}
[System.Serializable]
public class KeyEventProgressData
{
    public PlayerKeyEvent KeyEvent;

    public bool ShowInkFileName;
    public bool ShowAnimationState;
    public bool ShowFacingDir;
    public bool ShowPosition;
    public bool ShowIsVisible;

    public string InkFileName;
    public string AnimationState;
    public string FacingDir;
    public bool IsVisible;
    public Vector2 Position;
}