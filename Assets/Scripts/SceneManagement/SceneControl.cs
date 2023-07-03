using System;
using UnityEngine;
using System.Collections;
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
    public void SetCurrentScene(SceneDetails current_scene, bool isLoading = false)
    {
        if (isLoading)
            PreviousScene = null;
        else
            PreviousScene = CurrentScene;
        CurrentScene = current_scene;

        if (PreviousScene)
            previous_scene_name = PreviousScene.scene_name;
        else
            previous_scene_name = null;
    
        current_scene_name = CurrentScene.scene_name;
    }
    public List<Scene> ScenesExceptGameplay()
    {
        List<Scene> scene_list = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "Gameplay")
                continue;
            
            scene_list.Add(scene);
        }
        return scene_list;
    }

    public void UnloadExceptGameplay(List<Scene> scene_list, Action<bool> callback = null, bool startNewGame = false)
    {
        if (SceneManager.sceneCount == 1)
        {
            TitleScreen.isTitleScreen = false; // Important. If too early will not be able to start new game for some reason.
            TitleScreen.busy_with_menu = false; //

            callback?.Invoke(startNewGame);
            return;
        }

        Scene scene = scene_list[0];
        scene_list.Remove(scene_list[0]);
    
        var process = SceneManager.UnloadSceneAsync(scene.name);
        process.completed += (AsyncOperation operation) =>
        {
            UnloadExceptGameplay(scene_list, callback, startNewGame);
        };
    }

    public void CheckLoadComplete()
    {
        Debug.Log("Checking load completion...");
        if (!CurrentScene || !CurrentScene.CheckSceneExists()) {
            Debug.Log("Loading Main Scene...");
            return;
        }
        
        foreach (SceneDetails scene_detail in CurrentScene.connected_scenes)
        {
            if (!scene_detail.CheckSceneExists()) {
                Debug.Log("Loading chained scenes...");
                return;
            }
        }

        Debug.Log("Load Complete! Uncovering GameScreen.");
        CancelInvoke("CheckLoadComplete");
        GameManager.gm_instance.save_load_menu.isLoading = false;
        
        StartCoroutine(LoadingScreen.instance.UncoverLoadingScreen());
    }
}