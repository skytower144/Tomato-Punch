using UnityEngine;
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public ItemType itemType; 

}

public enum ItemType { Consumable, NormalEquip, SuperEquip, KeyItem }