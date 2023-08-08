using System;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    public static EssentialObjects instance { get; private set; }
    
    [SerializeField] private GameObject PortableBundle;
    public GameObject portable_bundle => PortableBundle;
    [SerializeField] private WorldCamera worldCamera;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;

        worldCamera.Init();

        TitleScreen.isTitleScreen = true;
        
        PlayerMovement.instance.collider_obj.SetActive(false);

        PortableBundle.transform.SetParent(transform.parent);
        PortableBundle.transform.position = new Vector3(-113.435f, -0.02000007f, 0f);
    }

    public void RestorePortablePosition(Action<bool> callback = null, bool startNewGame = false)
    {
        SceneControl.instance.UnloadExceptGameplay(SceneControl.instance.ScenesExceptGameplay(), GameManager.gm_instance.save_load_menu.ProceedLoad_1, startNewGame);

        PortableBundle.transform.position = new Vector3(0f, 0f, 0f);
        PortableBundle.transform.SetParent(transform);
    }
}
