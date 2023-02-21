using UnityEngine;

[System.Serializable]
public class CarryGoal : Goal
{
    [SerializeField] private Item targetItem;
    public override bool Evaluate()
    {
        return Inventory.instance.IsCarrying(targetItem, requiredAmount);
    }
}
