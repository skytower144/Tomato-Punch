using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool>{}

public class CutsceneTrigger : MonoBehaviour, Interactable
{
    [SerializeField] private bool _isAuto;
    [SerializeField] private string _inkFileName;
    [SerializeField] private List<GameObject> spawnList;
    [SerializeField] private List<Sprite> imageList;

    public void Interact()
    {
        return;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !_isAuto)
            TriggerCutscene();
    }

    void OnEnable()
    {
        if (_isAuto)
            TriggerCutscene();
    }

    private void TriggerCutscene()
    {
        TextAsset inkJsonData = InkDB.ReturnTextAsset(UIControl.currentLangMode, "Cutscene", "", _inkFileName, false);
        DialogueManager.instance.EnterDialogue(inkJsonData, null, true);
        DialogueManager.instance.cutsceneHandler.InitSpawnList(spawnList);
        DialogueManager.instance.cutsceneHandler.InitImageList(imageList);
        gameObject.SetActive(false);
    }
}
