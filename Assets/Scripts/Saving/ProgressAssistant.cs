using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressAssistant : MonoBehaviour
{
    [SerializeField]
    [ProgressInterface(typeof(ObjectProgress))]
    private List<Object> objectProgressList; // Interface List;

    void Start()
    {
        InitiateRestore();
    }

    public void InitiateCapture()
    {
        foreach (ObjectProgress progress in objectProgressList)
        {
            ProgressManager.instance.save_data.progress_dict[gameObject.scene.name + "_" + progress.ReturnID()] = progress.Capture();
        }
    }

    private void InitiateRestore()
    {
        StringProgressData dataDict = ProgressManager.instance.save_data.progress_dict;

        foreach (ObjectProgress progress in objectProgressList)
        {
            string total_key = gameObject.scene.name + "_" + progress.ReturnID();
            if (dataDict.ContainsKey(total_key))
                progress.Restore(dataDict[total_key]);
        }
        
    }
}