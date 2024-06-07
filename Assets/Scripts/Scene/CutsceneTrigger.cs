using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool>{}

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private string _inkFileName;
    [SerializeField] private List<GameObject> spawnList;
    [SerializeField] private List<Sprite> imageList;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.activeSelf && collision.tag == "Player")
            TriggerCutscene();
    }
    public void TriggerCutscene()
    {
        gameObject.SetActive(false);
        TextAsset inkJsonData = InkDB.ReturnTextAsset("Cutscene", "", _inkFileName, false);
        DialogueManager.instance.EnterDialogue(inkJsonData, null, true);
        DialogueManager.instance.cutsceneHandler.InitSpawnList(spawnList);
        DialogueManager.instance.cutsceneHandler.InitImageList(imageList);
    }
}
