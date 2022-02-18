using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, Interactable
{
    [SerializeField] private Item item;
    public void Interact()
    {
        Debug.Log("picked up item : " + item.ItemName);
    }
}
