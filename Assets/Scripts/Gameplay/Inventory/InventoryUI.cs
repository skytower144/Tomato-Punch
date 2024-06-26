using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;

    [Header("All")]
    [SerializeField] private Transform consumableListParent, otherItemParent;
    [SerializeField] private GameObject itemSlotPrefab;
    public GameObject itemslot_prefab => itemSlotPrefab;

    [Header("MATO")]
    [SerializeField] private tomatoControl tomatocontrol;
    [SerializeField] private Image left_equip1, left_equip2, left_super;
    [SerializeField] Transform slotParent, super_slotParent;
    private InventorySlot[] normalSlots;
    private InventorySlot[] superSlots;
    private int selected_1 = -1;
    private int selected_2 = -1;
    private int selected_s = -1;

    public (int, int, int) ReturnSlotIndex()
    {
        return (selected_1, selected_2, selected_s);
    }
    public void RecoverSlotIndex(int idx_1, int idx_2, int idx_s)
    {
        selected_1 = idx_1;
        selected_2 = idx_2;
        selected_s = idx_s;
    }

    public void InitiatlizeInventoryUI()
    {
        inventory = Inventory.instance;
        
        inventory.onItemChangedCallback -= UpdateUI;
        inventory.onItemChangedCallback += UpdateUI;

        normalSlots = slotParent.GetComponentsInChildren<InventorySlot>(true);
        superSlots = super_slotParent.GetComponentsInChildren<InventorySlot>(true);
        // "Should Components on inactive GameObjects be included in the found set?" -> (true)
    }
    private void UpdateUI(ItemType item_type) // ACTUAL item change in Inventory.cs -> invoke UpdateUI -> (VISIBLE) Update & Clear slots.
    {
        switch (item_type) {
            case ItemType.Consumable:
                UpdateConsumableSlots();
                break;

            case ItemType.Other:
                UpdateOtherItemSlots();
                break;

            case ItemType.NormalEquip:
                UpdateNormalSlots();
                break;
            
            case ItemType.SuperEquip:
                UpdateSuperSlots();
                break;
            
            default:
                break;
        }
    }

    public void ClearAllEquipSlots()
    {
        for (int i=0; i<normalSlots.Length; i++)
        {
            normalSlots[i].ClearSlot();
            normalSlots[i].DeselectSlot();
        }
        for (int i=0; i<superSlots.Length; i++)
        {
            superSlots[i].ClearSlot();
            superSlots[i].DeselectSlot();
        }
    }

    private void UpdateNormalSlots()
    {
        for (int i=0; i<normalSlots.Length; i++)
        {
            if (i < inventory.normalEquip.Count){
                normalSlots[i].UpdateSlot(inventory.normalEquip[i]);

                if (normalSlots[i].isEquipped_1)
                    normalSlots[i].SelectSlot();
                else if(normalSlots[i].isEquipped_2)
                    normalSlots[i].SelectSlot_2();
            }
            else {
                normalSlots[i].ClearSlot();
                normalSlots[i].DeselectSlot();
            }
        }
    }

    private void UpdateSuperSlots()
    {
        for (int i=0; i<superSlots.Length; i++)
        {
            if (i < inventory.superEquip.Count){
                superSlots[i].UpdateSlot(inventory.superEquip[i]);

                if (superSlots[i].isEquipped_1)
                    superSlots[i].SelectSlot();
            }
            else {
                superSlots[i].ClearSlot();
                superSlots[i].DeselectSlot();
            }
        }
    }
    public void UpdateEquipSlots(int left, int right, int super)
    {
        UpdateNormalSlots();
        UpdateSuperSlots();

        left_equip1.enabled = false;
        left_equip2.enabled = false;
        left_super.enabled = false;

        tomatocontrol.tomatoEquip[0] = null;
        tomatocontrol.tomatoEquip[1] = null;
        tomatocontrol.tomatoSuperEquip = null;
        
        AddColor_Left(left);
        AddColor_Right(right);
        AddColor_S(super);
    }

    public void UpdateConsumableSlots()
    {
        foreach (Transform child in consumableListParent) {
            Destroy(child.gameObject);
        }

        foreach (ItemQuantity itemQuantity in inventory.consumableItems) {
            GameObject itemSlot = Instantiate(itemSlotPrefab, consumableListParent);
            itemSlot.GetComponent<ItemSlotUI>().SetData(itemQuantity);
        }

        GameManager.gm_instance.UpdateConsumableSlots();
    }

    public void UpdateOtherItemSlots()
    {
        foreach (Transform child in otherItemParent) {
            Destroy(child.gameObject);
        }

        foreach (ItemQuantity itemQuantity in inventory.otherItems) {
            GameObject itemSlot = Instantiate(itemSlotPrefab, otherItemParent);
            itemSlot.GetComponent<ItemSlotUI>().SetData(itemQuantity);
        }

        GameManager.gm_instance.UpdateOtherItemSlots();
    }

    public void DisplayItemInfo(Item targetItem, TextMeshProUGUI targetText, Image targetImage)
    {
        targetImage.sprite = targetItem.ItemIcon;

        string descriptionTag = targetItem.ItemName + "_Description";
        targetText.text = TextDB.Translate(descriptionTag, TranslationType.UI) + DisplayStatusEffect(targetItem);
    }

    private string DisplayStatusEffect(Item item)
    {
        if (item.itemType == ItemType.Consumable) {
            int healAmount = ((Consumable)item).restoreAmount;
            return $"<br><color=#07791A>+ {healAmount} HP</color>";
        }
        return "";
    }

//EQUIPMENT FUNCTIONS -----------------------------------------------------------------------------------------------------
    //NORMAL SLOTS
    public void AddColor_Left(int num) // INITIATE EQUIP
    {
        if (num == -1)
            return;
        
        if(selected_1 >= 0)
        {
            normalSlots[selected_1].DeselectSlot(); // Unequip currently equipped item.
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
    public void AddColor_Right(int num)
    {
        if (num == -1)
            return;
        
        if(selected_2 >= 0)
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
        if (num == -1)
            return;
        
        if(selected_s >= 0){
            superSlots[selected_s].DeselectSlot();
            tomatocontrol.tomatoSuperEquip = null;
        }
    
        superSlots[num].SelectSlot();
        tomatocontrol.tomatoSuperEquip = (SuperEquip)inventory.superEquip[num];

        if(!left_super.enabled)
            left_super.enabled = true;
        left_super.sprite = ((SuperEquip)inventory.superEquip[num]).superIcon;

        selected_s = num;
    }
}
