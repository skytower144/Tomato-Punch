using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CountableItemDB
{
    static Dictionary<string, Item> CountableCatalog;
    public static Dictionary<string, Item> countableCatalog => CountableCatalog;

    public static void Initiatlize()
    {
        CountableCatalog = new Dictionary<string, Item>();

        var consumableArray = Resources.LoadAll<Item>("Consumable/");
        var otheritemArray = Resources.LoadAll<Item>("OtherItem/");

        var totalArray = consumableArray.Concat(otheritemArray).ToArray();
        
        foreach (Item item in totalArray)
        {
            if (CountableCatalog.ContainsKey(item.ItemName))
            {
                Debug.LogError($"Detected multiple items with the name {item.ItemName}");
                continue;
            }

            CountableCatalog[item.ItemName] = item;
        }
    }

    public static Item ReturnItemOfName(string input_name)
    {
        if (!CountableCatalog.ContainsKey(input_name))
        {
            Debug.LogError($"Item with the name {input_name} not found in the ConsumableDB");
            return null;
        }

        return CountableCatalog[input_name];
    }
}
