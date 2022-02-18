using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, Interactable
{
    public void Interact()
    {
        Debug.Log("picked up item");
    }
}
