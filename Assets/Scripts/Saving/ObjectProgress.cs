using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObjectProgress
{
    object Capture();
    void Restore(object state);
    string ReturnID();
}
