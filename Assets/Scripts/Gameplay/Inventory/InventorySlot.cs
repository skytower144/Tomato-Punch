using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon, thisSlotImage;
    [SerializeField] private Sprite selectedSlot, normal_defaultSlot;
    private Item item;
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
    }
    public void normal_DeselectSlot()
    {
        thisSlotImage.sprite = normal_defaultSlot;
    }
}
