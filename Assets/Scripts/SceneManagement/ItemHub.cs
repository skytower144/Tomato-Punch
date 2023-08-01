using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHub : MonoBehaviour
{
    public SceneName sceneName;

    public void SetVisibility(SceneName currentSceneName)
    {
        gameObject.SetActive(
            (sceneName == currentSceneName) ||
            (SceneDetails.connectedSceneDict[currentSceneName].Contains(sceneName))
        );
    }
}
