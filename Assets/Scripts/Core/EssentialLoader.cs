using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EssentialLoader : MonoBehaviour
{
    public static EssentialLoader instance { get; private set; }

    [SerializeField] private WorldCamera worldCamera;
    [SerializeField] private GameObject essentialPrefab;
    private GameObject essential;
    private EssentialObjects essentialControl;

    private void SetEssentialLoaderInstance()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        worldCamera.Init();
    }

    private void Awake()
    {
        SetEssentialLoaderInstance();

        var existingObjects = GameObject.FindGameObjectsWithTag("EssentialObjects");
        if (existingObjects.Length == 0) {
            TitleScreen.isTitleScreen = true;
            
            essential = Instantiate(essentialPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            
            PlayerMovement.instance.collider_obj.SetActive(false);
            essentialControl = essential.GetComponent<EssentialObjects>();

            essentialControl.portable_bundle.transform.SetParent(transform.parent);
            essentialControl.portable_bundle.transform.position = new Vector3(-113.435f, -0.02000007f, 0f);
        }
    }

    public void RestorePortablePosition(Action<bool> callback = null, bool startNewGame = false)
    {
        if (essential)
        {
            SceneControl.instance.UnloadExceptGameplay(SceneControl.instance.ScenesExceptGameplay(), GameManager.gm_instance.save_load_menu.ProceedLoad_1, startNewGame);

            essentialControl.portable_bundle.transform.position = new Vector3(0f, 0f, 0f);
            essentialControl.portable_bundle.transform.SetParent(essentialControl.transform);
        }
    }
}
