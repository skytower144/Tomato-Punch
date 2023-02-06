using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableNavigation : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform consumableSlotParent;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Color32 highlightTextColor, defaultTextColor;
    private ItemSlotUI[] slotList;
    private int slot_number = 0;
    private bool isNavigating = false; public bool is_navigating => isNavigating;

    void OnEnable()
    {
        slot_number = 0;
        isNavigating = false;
        slotList = consumableSlotParent.GetComponentsInChildren<ItemSlotUI>(true);
    }

    void Update()
    {
        if(!isNavigating && playerMovement.Press_Key("Interact"))
        {
            ToggleNavigate(true);
        }

        else if(isNavigating)
        {
            if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                gameManager.DetectHolding(UINavigate);
            }
            else if (gameManager.WasHolding)
            {
                gameManager.holdStartTime = float.MaxValue;
            }
            else if(playerMovement.Press_Key("Interact"))
            {
                return;
            }
            else if(playerMovement.Press_Key("Cancel"))
            {
                ToggleNavigate(false);
                return;
            }
        }
    }

    private void ToggleNavigate(bool state)
    {
        isNavigating = state;

        if (slotList.Length > 0) {
            if (state) {
                slotList[0].Select(highlightTextColor);
                DisplayItemInfo();
            }
            else {
                slotList[slot_number].Deselect(defaultTextColor);
                slot_number = 0;
            }
        }
    }

    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();
        
        int prev_num = slot_number;

        if (direction == InputDir.UP)
            slot_number -= 1;
        
        else if (direction == InputDir.DOWN)
            slot_number += 1;
        
        slot_number = Mathf.Clamp(slot_number, 0, slotList.Length - 1);

        if (prev_num == slot_number)
            return;

        slotList[prev_num].Deselect(defaultTextColor);
        slotList[slot_number].Select(highlightTextColor);
        DisplayItemInfo();
    }

    private void DisplayItemInfo()
    {
        Item targetItem = inventory.consumableItems[slot_number].item;
        itemIcon.sprite = targetItem.ItemIcon;

        string descriptionTag = targetItem.ItemName + "_Description";
        itemDescription.text = UIControl.instance.uiTextDict[descriptionTag];
    }
}
