using UnityEngine;

[CreateAssetMenu (fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public ItemType itemType; 

}

public enum ItemType { Consumable, NormalEquip, SuperEquip, KeyItem }