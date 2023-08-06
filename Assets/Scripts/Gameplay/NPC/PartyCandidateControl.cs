using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyCandidateControl : MonoBehaviour
{
    public ProgressAssistant progressAssistant;
    private List<SceneName> visibleScenes;
    [System.NonSerialized] public string sceneName = "CandidateControl";

    void Start()
    {
        foreach (Transform candidate in transform)
            candidate.gameObject.SetActive(false);
    }

    public void SetVisibility(SceneName currentScene)
    {
        visibleScenes = new List<SceneName> { currentScene };
        visibleScenes.AddRange(SceneDetails.connectedSceneDict[currentScene]);

        foreach (Transform candidate in transform)
            candidate.gameObject.SetActive(IsCandidateNearby(candidate));
    }

    private bool IsCandidateNearby(Transform candidate)
    {
        return visibleScenes.Contains(SceneControl.instance.GetSceneNameByPos(candidate.position));
    }
}
