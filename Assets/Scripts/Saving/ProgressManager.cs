using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringProgressData : SerializableDictionary<string, ProgressData>{}

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance { get; private set; }

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private FileDataHandler dataHandler;
    public FileDataHandler pm_dataHandler => dataHandler;

    public StringProgressData progress_dict = new StringProgressData();
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than on Progress Manager in scene.");
        }
        instance = this;

        // Application.persistentDataPath will give the OS standard directory for persisting data in a Unity project.
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        progress_dict = dataHandler.Load();
    }

    public void CaptureScene(bool saveSingleScene = false, string targetSceneName = null)
    {
        // Capture Objects' progress before unloading.
        foreach (GameObject assistant in GameObject.FindGameObjectsWithTag("ProgressAssistant"))
        {
            if (saveSingleScene)
            {
                if (assistant.scene.name == targetSceneName) {
                    assistant.GetComponent<ProgressAssistant>().InitiateCapture();
                    return;
                }
            }
            else {
                Debug.Log("capturing " + assistant.scene.name);
                assistant.GetComponent<ProgressAssistant>().InitiateCapture();
            }
        }
    }
}
