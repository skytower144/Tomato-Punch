using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObjectProgress
{
    ProgressData Capture();
    void Restore(ProgressData state);
    void ApplyKeyEvent(KeyEventProgressData data);
    List<KeyEventProgressData> ReturnKeyEventProgressList();
    string ReturnID();
}
