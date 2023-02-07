using UnityEngine;
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public ItemType itemType; 

    public static Item ReturnMatchingItem(string name)
    {
        if (CountableItemDB.countableCatalog.ContainsKey(name))
            return CountableItemDB.ReturnItemOfName(name);
        
        if (EquipDB.equipCatalog.ContainsKey(name))
            return EquipDB.ReturnItemOfName(name);
        
        Debug.LogWarning("Trying to return an Item that does not exist.");
        return null;
    }
}

public enum ItemType { Consumable, NormalEquip, SuperEquip, KeyItem }