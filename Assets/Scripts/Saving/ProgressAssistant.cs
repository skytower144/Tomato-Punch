using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressAssistant : MonoBehaviour
{
    
    [SerializeField]
    [ProgressInterface(typeof(ObjectProgress))]
    private List<Object> objectProgressList;

    void Start()
    {
        InitiateRestore();
    }

    public void InitiateCapture()
    {
        Dictionary<string, object> dataDict = new Dictionary<string, object>();
        foreach (ObjectProgress progress in objectProgressList)
        {
            dataDict[progress.ReturnID()] = progress.Capture();
        }

        ProgressManager.scene_progress[gameObject.scene.name] = dataDict;
    }

    private void InitiateRestore()
    {
        string loading_scene = gameObject.scene.name;
        if (ProgressManager.scene_progress.ContainsKey(loading_scene))
        {
            Dictionary<string, object> dataDict = new Dictionary<string, object>();
            dataDict = (Dictionary<string, object>) ProgressManager.scene_progress[loading_scene];

            foreach (ObjectProgress progress in objectProgressList)
            {
                progress.Restore(dataDict[progress.ReturnID()]);
            }
        }
    }
}
