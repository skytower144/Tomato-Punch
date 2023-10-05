using System;
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
    public Action<Item> onItemPickup;

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
        if (!item || count == 0) {
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
        onItemPickup?.Invoke(item);
    }

    public void RemoveItem (Item item, int targetSlotNumber = 0, int count = 1)
    {
        if (!item) {
            Debug.LogWarning("Trying to remove an Item that does not exist.");
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

    public bool IsCarrying(Item targetItem, int requiredAmount)
    {
        switch (targetItem.itemType)
        {
            case ItemType.Consumable:
            case ItemType.Other:
                List<ItemQuantity> countableItems = (targetItem.itemType == ItemType.Consumable) ? consumableItems : otherItems;

                foreach (ItemQuantity itemInfo in countableItems) {
                    if ((itemInfo.item.ItemName == targetItem.ItemName) && (itemInfo.count >= requiredAmount))
                        return true;
                }
                break;
            
            case ItemType.NormalEquip:
            case ItemType.SuperEquip:
                List<Item> equipItems = (targetItem.itemType == ItemType.NormalEquip) ? normalEquip : superEquip;

                foreach (Item item in equipItems) {
                    if (item.ItemName == targetItem.ItemName)
                        return true;
                }
                break;
            
            default:
                break;
        }
        return false;
    }
}

[System.Serializable]
public class ItemQuantity
{
    public Item item;
    [SerializeField, HideInInspector] public string ItemName;
    public int count;
    public void SerializeItemName()
    {
        ItemName = item.ItemName;
    }
    public void DeSerializeItemName()
    {
        item = CountableItemDB.ReturnItemOfName(ItemName);
    }
}
