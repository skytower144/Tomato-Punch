using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialLoader : MonoBehaviour
{
    public static EssentialLoader instance { get; private set; }

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
    }

    private void Awake()
    {
        SetEssentialLoaderInstance();

        var existingObjects = GameObject.FindGameObjectsWithTag("EssentialObjects");
        if (existingObjects.Length == 0) {
            TitleScreen.isTitleScreen = true;

            essential = Instantiate(essentialPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            
            essential.SetActive(false);
            essentialControl = essential.GetComponent<EssentialObjects>();
            essentialControl.portable_bundle.SetActive(false);

            essentialControl.portable_bundle.transform.SetParent(transform.parent);
            essentialControl.portable_bundle.transform.position = new Vector3(-113.435f, -0.02000007f, 0f);

            essentialControl.portable_bundle.SetActive(true);
        }
    }

    public void RestorePortablePosition()
    {
        if (essential)
        {
            SceneControl.instance.UnloadExceptGameplay();

            essentialControl.portable_bundle.SetActive(false);
            
            essentialControl.portable_bundle.transform.position = new Vector3(0f, 0f, 0f);
            essentialControl.portable_bundle.transform.SetParent(essentialControl.transform);

            essentialControl.portable_bundle.SetActive(true);
            essential.SetActive(true);

            TitleScreen.isTitleScreen = false;
            TitleScreen.busy_with_menu = false;
        }
    }
}
