using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressAssistant : MonoBehaviour
{
    [SerializeField] private bool isCandidateAssistant;

    [SerializeField]
    [ProgressInterface(typeof(ObjectProgress))]
    public List<Object> objectProgressList; // Interface List;

    void OnEnable()
    {
        string sceneName = isCandidateAssistant ? GameManager.gm_instance.partyManager.candidateControl.sceneName : gameObject.scene.name;

        if (!ProgressManager.instance.assistants.ContainsValue(this))
            ProgressManager.instance.assistants.Add(sceneName, this);
    }

    void Start()
    {
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
            
            GameManager.gm_instance.playerKeyEventManager.CheckProgressKeyEvent(progress);
        }   
    }
}
