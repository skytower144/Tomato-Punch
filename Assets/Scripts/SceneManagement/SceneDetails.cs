using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    public string scene_name;
    [SerializeField] private bool isIndoor;
    [SerializeField] private List<SceneDetails> connectedScenes;
    public List<SceneDetails> connected_scenes => connectedScenes;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Player"))
        {
            TriggerScene();
        }
    }

    public void TriggerScene()
    {   
        Debug.Log($"Entered {scene_name}");

        LoadScene();
        
        if (SceneControl.instance.CurrentScene != this)
            SceneControl.instance.SetCurrentScene(this);

        if (!isIndoor)
        {
            LoadChainedScenes();

            var previous_scene = SceneControl.instance.PreviousScene;

            // Unload the scenes that are no longer connected
            if (SceneControl.instance.PreviousScene != null)
            {
                var previouslyLoadedScenes = SceneControl.instance.PreviousScene.connectedScenes;
                foreach (var scene in previouslyLoadedScenes)
                {
                    // If it's not the connectected scenes of the current scene && Except the current scene
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }

                // Unload previous scene if not connected (In case of scene teleportation)
                if (!connectedScenes.Contains(previous_scene)){
                    // If the entering scene (current scene) is not the same as previous scene.
                    if (previous_scene != this)
                        previous_scene.UnloadScene();
                }
            }
        }
    }

    public void LoadChainedScenes()
    {
        foreach (var scene in connectedScenes)
        {
            scene.LoadScene();
        }
    }

    public void UnloadChainedScenes()
    {
        foreach (var scene in connectedScenes)
        {
            scene.UnloadScene();
        }
    }

    public void LoadScene(LocationPortal target_portal = null)
    {
        if (!CheckSceneExists())
        {
            var loading_process = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            
            loading_process.completed += (AsyncOperation operation) =>
            {
                if (target_portal != null)
                    target_portal.TeleportPlayer();
            };
        }
        else
        {
            if (target_portal != null)
                target_portal.TeleportPlayer();
        }
    }

    public void UnloadScene()
    {
        if (CheckSceneExists()) {
            ProgressManager.instance.CaptureScene(true, scene_name);
            SceneManager.UnloadSceneAsync(gameObject.name);
        }
    }

    public bool CheckSceneExists()
    {
        for (int n = 0; n < SceneManager.sceneCount; ++n)
        {
            Scene scene = SceneManager.GetSceneAt(n);
            if (gameObject.name == scene.name)
                return true;
        }
        return false;
    }
}
