using UnityEngine;

public class DestroyBindFail : MonoBehaviour
{
    void DeleteFailPrompt()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
