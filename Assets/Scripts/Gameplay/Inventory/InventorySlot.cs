using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon, thisSlotImage;
    [SerializeField] private Sprite selectedSlot, selectedSlot2, normal_defaultSlot;
    private Item item;
    public bool isEquipped_1 = false;
    public bool isEquipped_2 = false;
    public void UpdateSlot(Item newItem)
    {
        item = newItem;
        icon.sprite = item.ItemIcon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void SelectSlot()
    {
        thisSlotImage.sprite = selectedSlot;
        isEquipped_1 = true;
        isEquipped_2 = false;
    }
    public void SelectSlot_2()
    {
        thisSlotImage.sprite = selectedSlot2;
        isEquipped_2 = true;
        isEquipped_1 = false;
    }
    public void DeselectSlot()
    {
        thisSlotImage.sprite = normal_defaultSlot;
        
        if (isEquipped_1)
            isEquipped_1 = false;
        else if (isEquipped_2)
            isEquipped_2 = false;
    }

}
