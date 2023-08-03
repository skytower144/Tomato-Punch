using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressAssistant : MonoBehaviour
{
    public GameObject levelHolder;

    [SerializeField]
    [ProgressInterface(typeof(ObjectProgress))]
    private List<Object> objectProgressList; // Interface List;

    void Start()
    {
        if (!ProgressManager.instance.assistants.ContainsValue(this))
            ProgressManager.instance.assistants.Add(gameObject.scene.name, this);
        InitiateRestore();
    }

    void OnDestroy()
    {
        if (!string.IsNullOrEmpty(gameObject.scene.name) && ProgressManager.instance.assistants.ContainsValue(this))
            ProgressManager.instance.assistants.Remove(gameObject.scene.name);
    }

    public void InitiateCapture()
    {
        foreach (ObjectProgress progress in objectProgressList)
        {
            ProgressManager.instance.save_data.progress_dict[progress.ReturnID()] = progress.Capture();
        }
    }

    public void InitiateRestore()
    {
        StringProgressData dataDict = ProgressManager.instance.save_data.progress_dict;

        foreach (ObjectProgress progress in objectProgressList)
        {
            string total_key = progress.ReturnID();
            if (dataDict.ContainsKey(total_key))
                progress.Restore(dataDict[total_key]);
        }   
    }
}
