using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringItemLocation : SerializableDictionary<string, ItemLocationData>{}

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance { get; private set; }
    [SerializeField] private List<Transform> itemTrackers = new List<Transform>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Item Manager in scene.");
        }
        instance = this;
    }

    // if (Input.GetKeyDown(KeyCode.C))
    // {
    //     GameObject newItem = Instantiate(ItemPrefabDB.ReturnItemOfName("Knuckle Sandwich"), itemTrackers[0]);
    //     newItem.GetComponent<ItemPickup>().OnFirstCreated();
    // }

    public void RecoverItemState()
    {
        StringItemLocation createdItemDict = ProgressManager.instance.save_data.CreatedItem_dict;
        StringItemLocation removedItemDict = ProgressManager.instance.save_data.RemovedItem_dict;

        List<string> pickedUpItems = new List<string>();

        foreach (KeyValuePair<string, ItemLocationData> data in createdItemDict)
        {
            if (removedItemDict.ContainsKey(data.Key))
            {
                pickedUpItems.Add(data.Key);
                continue;
            }
            GameObject sceneItem = Instantiate(ItemPrefabDB.ReturnItemOfName(data.Value.itemName), itemTrackers[data.Value.located_sceneIndex]);
            sceneItem.GetComponent<ItemPickup>().RecoverId(data.Key);
            sceneItem.transform.localPosition = data.Value.located_position;
        }

        foreach (string itemKey in pickedUpItems)
        {
            removedItemDict.Remove(itemKey);
            createdItemDict.Remove(itemKey);
        }

        foreach (KeyValuePair<string, ItemLocationData> data in removedItemDict)
        {
            Transform parent_scene = itemTrackers[data.Value.located_sceneIndex];
            foreach (Transform itemTransform in parent_scene)
            {
                if (itemTransform.GetComponent<ItemPickup>().itemID == data.Key)
                    Destroy(itemTransform.gameObject);
            }
        }
    }
    
}

[System.Serializable]
public class ItemLocationData
{
    public string itemName;
    public Vector3 located_position;
    public int located_sceneIndex;

    public ItemLocationData(string itemName, Vector3 position, int located_sceneIndex)
    {
        this.itemName = itemName;
        this.located_position = position;
        this.located_sceneIndex = located_sceneIndex;
    }
}
