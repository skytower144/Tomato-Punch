using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerManager : MonoBehaviour
{
    private Dictionary<string, CutsceneTrigger> _cutsceneTriggerDict = new Dictionary<string, CutsceneTrigger>();
    void Start()
    {
        CutsceneTrigger[] triggers = GetComponentsInChildren<CutsceneTrigger>(true);
        
        foreach (CutsceneTrigger trigger in triggers) {
            _cutsceneTriggerDict[trigger.gameObject.name] = trigger;
        }
    }

    public void SetCutsceneTrigger(string tag, bool state)
    {
        _cutsceneTriggerDict[tag].gameObject.SetActive(state);
    }
}
