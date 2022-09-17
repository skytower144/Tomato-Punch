using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, Interactable
{
    [SerializeField] private Item item; public Item targetItem => item;
    [SerializeField] private string id; public string itemID => id;

    [ContextMenu("Generate guid for ID")]
    public void GenerateItemGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public void RecoverId(string backup_id)
    {
        id = backup_id;
    }

    public void OnFirstCreated() // Declare this function after instantiating Item.
    {
        GenerateItemGuid();
        ProgressManager.instance.save_data.CreatedItem_dict[id] = new ItemLocationData(item.ItemName, transform.localPosition, transform.parent.transform.GetSiblingIndex());
    }
    public void Interact()
    {
        Inventory.instance.AddItem(item);
        ProgressManager.instance.save_data.RemovedItem_dict[id] = new ItemLocationData(item.ItemName, transform.localPosition, transform.parent.transform.GetSiblingIndex());
        Destroy(gameObject);
    }
}
