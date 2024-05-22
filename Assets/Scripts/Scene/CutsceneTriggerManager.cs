using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerManager : MonoBehaviour
{
    private Dictionary<string, CutsceneTrigger> _cutsceneTriggerDict = new Dictionary<string, CutsceneTrigger>();
    private CutsceneTrigger[] triggers;
    string triggerName;
    
    private void InitDict()
    {
        if (triggers == null) {
            triggers = GetComponentsInChildren<CutsceneTrigger>(true);
            foreach (CutsceneTrigger trigger in triggers)
                _cutsceneTriggerDict[trigger.gameObject.name] = trigger;
        }
    }
    public void SetCutsceneTrigger(string tag, bool state)
    {
        InitDict();
        _cutsceneTriggerDict[tag].gameObject.SetActive(state);
    }
    public void Capture()
    {
        if (triggers == null)
            triggers = GetComponentsInChildren<CutsceneTrigger>(true);
        
        foreach (CutsceneTrigger trigger in triggers) {
            triggerName = trigger.gameObject.name;
            _cutsceneTriggerDict[triggerName] = trigger;
            ProgressManager.instance.save_data.CutTrigger_dict[triggerName] = trigger.gameObject.activeSelf;
        }
    }
    public void Restore()
    {
        InitDict();
        foreach (KeyValuePair<string, bool> keyValuePair in ProgressManager.instance.save_data.CutTrigger_dict) {
            if (_cutsceneTriggerDict.ContainsKey(keyValuePair.Key))
                _cutsceneTriggerDict[keyValuePair.Key].gameObject.SetActive(keyValuePair.Value);
        }
    }
}
