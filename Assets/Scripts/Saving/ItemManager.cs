using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringItemLocation : SerializableDictionary<string, ItemLocationData>{}

public class ItemManager : MonoBehaviour
{
    [SerializeField] private List<ItemHub> itemHubList = new List<ItemHub>();

    // if (Input.GetKeyDown(KeyCode.C))
    // {
    //     GameObject newItem = Instantiate(ItemPrefabDB.ReturnItemOfName("Knuckle Sandwich"), itemTrackers[0]);
    //     newItem.GetComponent<ItemPickup>().OnFirstCreated();
    // }

    void Awake()
    {
        Dictionary<string, int> duplicateIdCheck = new Dictionary<string, int>();

        for (int i = 0; i < itemHubList.Count; i++)
        {
            Transform parent_scene = itemHubList[i].transform;
            ItemPickup[] items = parent_scene.GetComponentsInChildren<ItemPickup>(true);

            foreach (ItemPickup itemInfo in items)
            {
                if (duplicateIdCheck.ContainsKey(itemInfo.itemID)) {
                    Debug.LogError($"Detected duplicate Item ID from : [{itemInfo.targetItem.ItemName}]. Check ItemManager.");
                    return;
                }
                duplicateIdCheck[itemInfo.itemID] = 1;
            }
        }
    }

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
            GameObject sceneItem = Instantiate(ItemPrefabDB.ReturnItemOfName(data.Value.itemName), ReturnItemTransform(data.Value.located_scene));
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
            Transform parent_scene = ReturnItemTransform(data.Value.located_scene);
            foreach (Transform itemTransform in parent_scene)
            {
                if (itemTransform.GetComponent<ItemPickup>().itemID == data.Key)
                    Destroy(itemTransform.gameObject);
            }
        }
    }

    public void SetItemVisibility(SceneName currentScene)
    {
        foreach (ItemHub hub in itemHubList) {
            hub.SetVisibility(currentScene);
        }
    }

    private Transform ReturnItemTransform(SceneName targetScene)
    {
        foreach (ItemHub hub in itemHubList) {
            if (hub.sceneName == targetScene)
                return hub.transform;
        }
        Debug.LogError($"Item transform not found : {targetScene} does not exist.");
        return transform;
    }
}

[System.Serializable]
public class ItemLocationData
{
    public string itemName;
    public Vector3 located_position;
    public SceneName located_scene;

    public ItemLocationData(string itemName, Vector3 position, SceneName located_scene)
    {
        this.itemName = itemName;
        this.located_position = position;
        this.located_scene = located_scene;
    }
}
