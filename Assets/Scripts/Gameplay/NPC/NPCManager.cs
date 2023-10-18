using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringNpccontrol : SerializableDictionary<string, NPCController>{}

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance { get; private set; }
    public StringNpccontrol npc_dict = new StringNpccontrol();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one NPC Manager in scene.");
        }
        instance = this;
    }
}
