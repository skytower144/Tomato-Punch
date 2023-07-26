using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPrefabDB
{
    static Dictionary<string, GameObject> ItemPrefabCatalog;

    public static void Initiatlize()
    {
        ItemPrefabCatalog = new Dictionary<string, GameObject>();

        var prefabArray = Resources.LoadAll<GameObject>("ItemPrefabs/");
        //Debug.Log(prefabArray.Length);

        foreach (GameObject prefab in prefabArray)
        {
            string item_name = prefab.GetComponent<ItemPickup>().targetItem.ItemName;
            if (ItemPrefabCatalog.ContainsKey(item_name))
            {
                Debug.LogError($"Detected multiple prefabs with the name {item_name}");
                continue;
            }

            ItemPrefabCatalog[item_name] = prefab;
        }
    }

    public static GameObject ReturnItemOfName(string input_name)
    {
        if (!ItemPrefabCatalog.ContainsKey(input_name))
        {
            Debug.LogError($"Prefab with the name {input_name} not found in the ItemPrefabDB");
            return null;
        }

        return ItemPrefabCatalog[input_name];
    }
}
