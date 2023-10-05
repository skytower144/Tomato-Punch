using UnityEngine;

[System.Serializable]
public class CarryGoal : Goal
{
    [SerializeField] private Item targetItem;
    [SerializeField, HideInInspector] public string TargetItemName;

    public override void Init()
    {
        Inventory.instance.onItemPickup -= Pickup;
        Inventory.instance.onItemPickup += Pickup;
    }

    public override void UnsubsribeEvent()
    {
        Inventory.instance.onItemPickup -= Pickup;
    }

    public override bool Evaluate()
    {
        return Inventory.instance.IsCarrying(targetItem, requiredAmount);
    }

    private void Pickup(Item item)
    {
        if (item == targetItem)
            this.currentAmount++;
    }
    public void SerializeItemName()
    {
        TargetItemName = targetItem.ItemName;
    }
    public void DeSerializeItemName()
    {
        targetItem = CountableItemDB.ReturnItemOfName(TargetItemName);
    }
}
