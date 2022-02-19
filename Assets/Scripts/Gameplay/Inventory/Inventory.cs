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
            Debug.LogWarning("More than one instance of Inventory found.");
            return;
        }

        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    [SerializeField] private InventoryUI inventoryUI;
    [System.NonSerialized] public int itemType_num;
    public List<Item> normalEquip = new List<Item>();
    

    private void Start()
    {
        inventoryUI.activateUI();
    }
    public void AddItem(Item item)
    {
        if(item.itemType == ItemType.NormalEquip)
        {
            normalEquip.Add(item);
            itemType_num = 1;
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
    }

    public void RemoveItem (Item item)
    {
        if(item.itemType == ItemType.NormalEquip)
        {
            normalEquip.Remove(item);
            itemType_num = 1;
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
    }

}
