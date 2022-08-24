using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LoadScene();
            //Debug.Log($"entered {gameObject.name}");
            GameManager.gm_instance.SetCurrentScene(this);

            // Load all connected scenes
            foreach (var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            // Unload the scenes that are no longer connected
            if (GameManager.gm_instance.PreviousScene != null)
            {
                var previouslyLoadedScenes = GameManager.gm_instance.PreviousScene.connectedScenes;
                foreach (var scene in previouslyLoadedScenes)
                {
                    // If it's not the connectected scenes of the current scene && If this scene is not the currently loaded scene
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!CheckSceneExists())
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
        }
    }

    public void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(gameObject.name);
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
