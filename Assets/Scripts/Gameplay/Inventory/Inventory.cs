using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;
    void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of Inventory found.");
            return;
        }

        instance = this;
    }
    #endregion

    public delegate void OnItemChanged(ItemType item_type);
    public OnItemChanged onItemChangedCallback;
    [SerializeField] private InventoryUI inventoryUI;

    public List<ItemQuantity> consumableItems = new List<ItemQuantity>();
    public List<Item> normalEquip = new List<Item>();
    public List<Item> superEquip = new List<Item>();
    
    public void GatherEquipSlots()
    {
        inventoryUI.InitiatlizeInventoryUI();
    }
    public void AddItem(Item item, int count = 1)
    {
        ItemQuantity tempItem = new ItemQuantity();
        tempItem.item = item;
        tempItem.count = count;
        
        switch (item.itemType) {
            case ItemType.Consumable:
                consumableItems.Add(tempItem);
                break;

            case ItemType.NormalEquip:
                normalEquip.Add(item);
                break;
            
            case ItemType.SuperEquip:
                superEquip.Add(item);
                break;
            
            default:
                break;
        }
        
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke(item.itemType);
    }

    public void RemoveItem (Item item)
    {
        switch (item.itemType) {

            case ItemType.NormalEquip:
                normalEquip.Remove(item);
                break;

            default:
                break;
        }

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke(item.itemType);
    }
}

[System.Serializable]
public class ItemQuantity
{
    public Item item;
    public int count;
}
