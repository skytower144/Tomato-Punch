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
    public InventoryUI inventory_UI => inventoryUI;

    public List<ItemQuantity> consumableItems = new List<ItemQuantity>();
    public List<ItemQuantity> otherItems = new List<ItemQuantity>();
    public List<Item> normalEquip = new List<Item>();
    public List<Item> superEquip = new List<Item>();
    
    public void GatherEquipSlots()
    {
        inventoryUI.InitiatlizeInventoryUI();
    }
    public void AddItem(Item item, int count = 1)
    {
        if (!item) {
            Debug.LogWarning("Trying to add an Item that does not exist.");
            return;
        }
        
        ItemQuantity tempItem = new ItemQuantity();
        tempItem.item = item;
        tempItem.count = count;
        
        switch (item.itemType) {
            case ItemType.Consumable:
                ChangeItemStack(tempItem, consumableItems, count);
                break;
            
            case ItemType.Other:
                ChangeItemStack(tempItem, otherItems, count);
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
        
        StartCoroutine(ItemChangeEvent(item.itemType));
    }

    public void RemoveItem (Item item, int targetSlotNumber = 0, int count = 1)
    {
        if (!item) {
            Debug.LogWarning("Trying to add an Item that does not exist.");
            return;
        }

        switch (item.itemType) {
            case ItemType.Consumable:
                ChangeItemStack(consumableItems[targetSlotNumber], consumableItems, -count);
                break;

            case ItemType.Other:
                ChangeItemStack(otherItems[targetSlotNumber], otherItems, -count);
                break;

            case ItemType.NormalEquip:
                normalEquip.Remove(item);
                break;

            default:
                break;
        }
        
        StartCoroutine(ItemChangeEvent(item.itemType));
    }

    public void ChangeItemStack(ItemQuantity targetItem, List<ItemQuantity> itemList, int amount)
    {
        for (int i = 0; i < itemList.Count; i++) {
            if (itemList[i].item.ItemName == targetItem.item.ItemName) {
                itemList[i].count += amount;

                if ((itemList[i].count < 0) || itemList[i].count > 999) {
                    itemList[i].count = Mathf.Clamp(itemList[i].count, 0, 999);
                    Debug.LogWarning("Item amount has reached its valid limit.");
                }
                if (itemList[i].count == 0) {
                    itemList.Remove(targetItem);
                }
                return;
            }
        }
        if (amount > 0)
            itemList.Add(targetItem); // Adding a new item that player does not have.
    }

    IEnumerator ItemChangeEvent(ItemType item_type)
    {
        yield return new WaitForEndOfFrame();
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke(item_type);
    }
}

[System.Serializable]
public class ItemQuantity
{
    public Item item;
    public int count;
}
