using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDB
{
    static Dictionary<string, Item> EquipCatalog;
    public static Dictionary<string, Item> equipCatalog => EquipCatalog;

    public static void Initiatlize()
    {
        EquipCatalog = new Dictionary<string, Item>();

        var equipArray = Resources.LoadAll<Item>("Equip/");
        foreach (Item item in equipArray)
        {
            if (EquipCatalog.ContainsKey(item.ItemName))
            {
                Debug.LogError($"Detected multiple items with the name {item.ItemName}");
                continue;
            }

            EquipCatalog[item.ItemName] = item;
        }
    }

    public static Item ReturnItemOfName(string input_name)
    {
        if (!EquipCatalog.ContainsKey(input_name))
        {
            Debug.LogError($"Item with the name {input_name} not found in the EquipDB");
            return null;
        }

        return EquipCatalog[input_name];
    }
}
