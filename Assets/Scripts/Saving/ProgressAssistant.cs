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
