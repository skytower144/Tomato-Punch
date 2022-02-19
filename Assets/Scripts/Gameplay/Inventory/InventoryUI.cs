using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    private InventorySlot[] slots;
    private Inventory inventory;
    public void activateUI()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = slotParent.GetComponentsInChildren<InventorySlot>(true);
        // "Should Components on inactive GameObjects be included in the found set?" -> (true)
    }
    void UpdateUI() // ACTUAL item change in Inventory.cs -> invoke UpdateUI -> VISIBLE Update & Clear slots.
    {
        if (inventory.itemType_num == 1)
        {
            for (int i=0; i<slots.Length; i++)
            {
                if (i < inventory.normalEquip.Count){
                    slots[i].UpdateSlot(inventory.normalEquip[i]);
                }
                else {
                    slots[i].ClearSlot();
                }
            }
        }
        
    }
    
}
