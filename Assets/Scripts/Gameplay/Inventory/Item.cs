using UnityEngine;
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public ItemType itemType; 

    public static Item ReturnMatchingItem(string name) // Type in public ItemName, not object name.
    {
        if (CountableItemDB.countableCatalog.ContainsKey(name))
            return CountableItemDB.ReturnItemOfName(name);
        
        if (EquipDB.equipCatalog.ContainsKey(name))
            return EquipDB.ReturnItemOfName(name);
        
        Debug.LogWarning("Trying to return an Item that does not exist.");
        return null;
    }

    public virtual ItemUseInfo Use(tomatoControl tomatocontrol)
    {
        return null;
    }
}

[System.Serializable]
public class ItemUseInfo
{
    public string reactAnimName;
    public string effectInfo;
    public bool isUsed;
}

public enum ItemType { Consumable, Other, NormalEquip, SuperEquip }