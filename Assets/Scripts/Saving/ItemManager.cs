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
    public void CaptureItemState()
    {
        StringItemLocation locationDict = ProgressManager.instance.save_data.itemLocationDict;
        locationDict.Clear();

        for (int i = 0; i < itemTrackers.Count; i++)
        {
            Transform scene = itemTrackers[i];
            foreach (Transform itemTransform in scene)
            {
                ItemPickup item_info = itemTransform.gameObject.GetComponent<ItemPickup>();
                locationDict[item_info.itemID] = new ItemLocationData(item_info.targetItem.ItemName, itemTransform.localPosition, i);
            }
        }
    }

    public void RecoverItemState()
    {
        StringItemLocation locationDict = ProgressManager.instance.save_data.itemLocationDict;

        foreach (Transform scene in itemTrackers)
        {
            foreach (Transform itemTransform in scene)
            {
                ItemPickup item_info = itemTransform.gameObject.GetComponent<ItemPickup>();

                if (locationDict.ContainsKey(item_info.itemID)){
                    locationDict.Remove(item_info.itemID);
                }
                
                else
                    Destroy(itemTransform.gameObject);
            }
        }

        if (locationDict.Count > 0)
        {
            foreach (KeyValuePair<string, ItemLocationData> data in locationDict)
            {
                GameObject sceneItem = Instantiate(ItemPrefabDB.ReturnItemOfName(data.Value.itemName), itemTrackers[data.Value.located_sceneIndex]);
                sceneItem.transform.localPosition = data.Value.located_position;
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
