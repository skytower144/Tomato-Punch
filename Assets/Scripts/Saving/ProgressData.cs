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
}