using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public int restoreAmount;
    public bool restoreFullHp;
}
