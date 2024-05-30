using System;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    public static EssentialObjects instance { get; private set; }
    
    [SerializeField] private GameObject PortableBundle;
    public GameObject portable_bundle => PortableBundle;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;

        TitleScreen.isTitleScreen = true;
        
        PlayerMovement.instance.myCol.gameObject.SetActive(false);
        PlayerMovement.instance.Teleport(18.45999f, -0.9450035f);

        PortableBundle.transform.SetParent(transform.parent);
        PortableBundle.transform.position = new Vector3(-113.435f, -0.02000007f, 0f);
    }

    public void RestorePortablePosition(bool startNewGame = false)
    {
        SceneControl.instance.UnloadExceptGameplay(SceneControl.instance.ScenesExceptGameplay(), GameManager.gm_instance.save_load_menu.ProceedLoad_1, startNewGame);

        PortableBundle.transform.position = new Vector3(0f, 0f, 0f);
        PortableBundle.transform.SetParent(transform);
    }
}
