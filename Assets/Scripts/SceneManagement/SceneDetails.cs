using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    private string sceneName;
    public string scene_name => sceneName;
    [SerializeField] private bool isIndoor;
    public bool is_indoor => isIndoor;

    void Start()
    {
        sceneName = gameObject.name;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LoadScene();
            //Debug.Log($"entered {gameObject.name}");
            GameManager.gm_instance.SetCurrentScene(this);

            if (!isIndoor)
            {
                LoadChainedScenes();

                var previous_scene = GameManager.gm_instance.PreviousScene;

                // Unload the scenes that are no longer connected
                if (GameManager.gm_instance.PreviousScene != null)
                {
                    var previouslyLoadedScenes = GameManager.gm_instance.PreviousScene.connectedScenes;
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
                        previous_scene.UnloadScene();
                    }
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

    public void LoadScene(LocationPortal portal = null)
    {
        if (!CheckSceneExists())
        {
            var loading_process = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            
            loading_process.completed += (AsyncOperation operation) =>
            {
                if (portal != null)
                    portal.TeleportPlayer();
            };
            
        }
    }

    public void UnloadScene()
    {
        if (CheckSceneExists()) {
            
            // Capture Objects' progress before unloading.
            foreach (GameObject assistant in GameObject.FindGameObjectsWithTag("ProgressAssistant"))
            {
                if (assistant.scene.name == scene_name) {
                    assistant.GetComponent<ProgressAssistant>().InitiateCapture();
                    break;
                }
            }

            SceneManager.UnloadSceneAsync(gameObject.name);
        }
    }


    private bool CheckSceneExists()
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
