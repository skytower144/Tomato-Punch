using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, Interactable
{
    [SerializeField] private Item item; public Item targetItem => item;
    [SerializeField] private string id; public string itemID => id;

    private void Awake()
    {
        GenerateItemGuid();
    }

    // [ContextMenu("Generate guid for ID")]
    public void GenerateItemGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public void Interact()
    {
        Inventory.instance.AddItem(item);
        Destroy(gameObject);
    }
}
