using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    [SerializeField] private GameObject PortableBundle;
    public GameObject portable_bundle => PortableBundle;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
