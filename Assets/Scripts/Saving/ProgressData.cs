using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressData
{
    public string InkFileName, AnimationState;
    public bool IsVisible;
    public Vector2 Position;
    public List<KeyEventDialogue> KeyEventDialogues;
    public List<KeyEventProgressData> KeyEventList;
}
[System.Serializable]
public class KeyEventProgressData
{
    public PlayerKeyEvent KeyEvent;

    public bool ShowInkFileName;
    public string InkFileName;

    public bool ShowAnimationState;
    public string AnimationState;

    public bool ShowIsVisible;
    public bool IsVisible;

    public bool ShowPosition;
    public Vector2 Position;
}