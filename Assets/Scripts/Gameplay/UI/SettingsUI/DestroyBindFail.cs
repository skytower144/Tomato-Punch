using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBindFail : MonoBehaviour
{
    void DeleteFailPrompt()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
