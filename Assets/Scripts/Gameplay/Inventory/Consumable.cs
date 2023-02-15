using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    [Header("HP")]
    public int restoreAmount;
    public bool restoreFullHp;

    [Header("Status Conditions")]
    public bool recoverAllStatus;

    public override bool Use(tomatoControl tomatoControl)
    {
        if (restoreFullHp || restoreAmount > 0) {
            tomatoControl.currentHealth = Mathf.Clamp(tomatoControl.currentHealth + restoreAmount, 1, tomatoControl.maxHealth);
        }
        else
            return false;

        return true;
    }
}
