using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public static SceneControl instance { get; private set; }
    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }
    [SerializeField] private string current_scene_name, previous_scene_name;
    public Dictionary<string, SceneDetails> sceneDict = new Dictionary<string, SceneDetails>();

    void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
        SceneControl.instance.InitializeSceneDict();
    }

    public void InitializeSceneDict()
    {
        SceneDetails[] all_scene_details = transform.GetComponentsInChildren<SceneDetails>(true);

        foreach (SceneDetails scene_detail in all_scene_details)
        {
            sceneDict[scene_detail.scene_name] = scene_detail;
        }
    }
    public void SetCurrentScene(SceneDetails current_scene, bool onlyCurrent = false)
    {
        if (!onlyCurrent)
            PreviousScene = CurrentScene;
        CurrentScene = current_scene;

        if (PreviousScene)
            previous_scene_name = PreviousScene.scene_name;
        current_scene_name = CurrentScene.scene_name;
    }
    
    public void UnloadExceptGameplay() // This function will not capture any progress beforehand.
    {
        for (int n = 0; n < SceneManager.sceneCount; ++n)
        {
            Scene scene = SceneManager.GetSceneAt(n);
            if(scene.name == "Gameplay")
                continue;
            
            SceneManager.UnloadSceneAsync(scene.name);
        }
    }

    public bool CheckLoadComplete()
    {
        Debug.Log("checking load completion...");
        if (!CurrentScene)
            return false;
        
        if (!CurrentScene.CheckSceneExists())
            return false;
        
        foreach (SceneDetails scene_detail in CurrentScene.connected_scenes)
        {
            if (!scene_detail.CheckSceneExists())
                return false;
        }

        Debug.Log("Load Complete.\nUncovering GameScreen.");
        CancelInvoke("CheckLoadComplete");
        return true;
    }
}
