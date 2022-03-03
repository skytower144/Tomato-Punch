using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Image left_equip1, left_equip2, left_super;
    [SerializeField] Transform slotParent, super_slotParent;
    private InventorySlot[] normalSlots;
    private InventorySlot[] superSlots;
    private int selected_1 = -1;
    private int selected_2 = -1;
    private int selected_s = -1;
    public void activateUI()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        normalSlots = slotParent.GetComponentsInChildren<InventorySlot>(true);
        superSlots = super_slotParent.GetComponentsInChildren<InventorySlot>(true);
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
        else if (inventory.itemType_num == 2)
        {
            for (int i=0; i<superSlots.Length; i++)
            {
                if (i < inventory.superEquip.Count){
                    superSlots[i].UpdateSlot(inventory.superEquip[i]);
                }
            }
        }
    }

//EQUIPMENT FUNCTIONS -----------------------------------------------------------------------------------------------------
    //NORMAL SLOTS
    public void AddColor_1(int num)
    {
        if(selected_1>=0)
        {
            normalSlots[selected_1].DeselectSlot();
            tomatocontrol.tomatoEquip[0] = null;
        }
        if (num == selected_2)
        {
            selected_2 = -1;
            tomatocontrol.tomatoEquip[1] = null;
            left_equip2.enabled = false;
        }
            
        normalSlots[num].SelectSlot();
        tomatocontrol.tomatoEquip[0] = (Equip)inventory.normalEquip[num];

        left_equip1.enabled = true;
        left_equip1.sprite = inventory.normalEquip[num].ItemIcon;

        selected_1 = num;
    }
    public void AddColor_2(int num)
    {
        if(selected_2>=0)
        {
            normalSlots[selected_2].DeselectSlot();
            tomatocontrol.tomatoEquip[1] = null;
        }
        if (num == selected_1){
            selected_1 = -1;
            tomatocontrol.tomatoEquip[0] = null;
            left_equip1.enabled = false;
        }
        normalSlots[num].SelectSlot_2();
        tomatocontrol.tomatoEquip[1] = (Equip)inventory.normalEquip[num];

        left_equip2.enabled = true;
        left_equip2.sprite = inventory.normalEquip[num].ItemIcon;

        selected_2 = num;
    }

    //SUPER SLOTS
    public void AddColor_S(int num)
    {
        if(selected_s>=0){
            superSlots[selected_s].DeselectSlot();
            tomatocontrol.tomatoSuperEquip = null;
        }
    
        superSlots[num].SelectSlot();
        tomatocontrol.tomatoSuperEquip = (SuperEquip)inventory.superEquip[num];
        tomatocontrol.tomatoSuper = tomatocontrol.tomatoSuperEquip.superNumber;
        tomatocontrol.dmg_super = tomatocontrol.tomatoSuperEquip.superDamage;

        if(!left_super.enabled)
            left_super.enabled = true;
        left_super.sprite = inventory.superEquip[num].ItemIcon;

        selected_s = num;
        
    }
    

}
