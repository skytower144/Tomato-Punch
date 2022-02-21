using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    private InventorySlot[] normalSlots;
    private Inventory inventory;
    private int selected_slotNumber = -1;
    public void activateUI()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        normalSlots = slotParent.GetComponentsInChildren<InventorySlot>(true);
        // "Should Components on inactive GameObjects be included in the found set?" -> (true)
    }
    void UpdateUI() // ACTUAL item change in Inventory.cs -> invoke UpdateUI -> VISIBLE Update & Clear slots.
    {
        if (inventory.itemType_num == 1)
        {
            for (int i=0; i<normalSlots.Length; i++)
            {
                if (i < inventory.normalEquip.Count){
                    normalSlots[i].UpdateSlot(inventory.normalEquip[i]);
                }
                else {
                    normalSlots[i].ClearSlot();
                }
            }
        }
        
    }

    public void AddColor(int num)
    {
        normalSlots[num].SelectSlot();
        selected_slotNumber = num;
    }
    public void ClearColor()
    {
        if (selected_slotNumber != -1)
            normalSlots[selected_slotNumber].normal_DeselectSlot();
    }
}
