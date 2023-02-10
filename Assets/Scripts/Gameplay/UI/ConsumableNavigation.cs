using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableNavigation : MonoBehaviour
{
    const int MAX_SLOT_VIEW = 7;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private GameObject uiGuide, arrowGuide, upArrow, downArrow;
    [SerializeField] private Color32 highlightTextColor, defaultTextColor;
    private ItemSlotUI[] slotList;
    private int slot_number = 0;
    private float slot_height;
    private float end_of_view;
    private bool isNavigating = false; public bool is_navigating => isNavigating;

    void Start()
    {
        slot_height = inventory.inventory_UI.itemslot_prefab.GetComponent<RectTransform>().rect.height;
        UpdateEndOfViewValue();
    }

    void OnEnable()
    {
        slot_number = 0;
        isNavigating = itemIcon.enabled = false;

        itemDescription.text = UIControl.instance.uiTextDict["InventoryGuide"];
        UIControl.instance.SetFontData(itemDescription, "Item_Description");

        slotList = itemSlotParent.GetComponentsInChildren<ItemSlotUI>(true);

        uiGuide.SetActive(true);
        arrowGuide.SetActive(false);
        ArrowControl();
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
        uiGuide.SetActive(!state);
        arrowGuide.SetActive(state);

        if (slotList.Length > 0) {
            if (state) {
                slotList[0].Select(highlightTextColor);
                itemIcon.enabled = true;
                DisplayItemInfo();
            }
            else {
                slotList[slot_number].Deselect(defaultTextColor);
                itemIcon.enabled = false;
            }
        }

        if (!state) {
            slot_number = 0;
            itemDescription.text = UIControl.instance.uiTextDict["InventoryGuide"];
            ResetSlotListPosition();
            ArrowControl();
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
        
        slot_number = Mathf.Clamp(slot_number, 0, Mathf.Clamp(slotList.Length - 1, 0, slotList.Length));

        if (prev_num == slot_number)
            return;

        HandleScroll();
        slotList[prev_num].Deselect(defaultTextColor);
        slotList[slot_number].Select(highlightTextColor);
        DisplayItemInfo();
    }

    private void HandleScroll()
    {
        float scrollPos = Mathf.Clamp((slot_number - MAX_SLOT_VIEW/2), 0, slot_number) * slot_height;
        
        if (scrollPos > end_of_view)
            return;
            
        itemSlotParent.localPosition = new Vector2(itemSlotParent.localPosition.x, scrollPos);
        ArrowControl();
    }

    private void ArrowControl()
    {
        bool showUpArrow = slot_number > MAX_SLOT_VIEW / 2;
        bool showDownArrow = slot_number + MAX_SLOT_VIEW / 2 < slotList.Length - 1;
        upArrow.SetActive(showUpArrow);
        downArrow.SetActive(showDownArrow);
    }

    private void ResetSlotListPosition()
    {
        itemSlotParent.localPosition = new Vector2(itemSlotParent.localPosition.x, 0f);
    }

    private void UpdateEndOfViewValue()
    {
        end_of_view = slot_height * Mathf.Clamp(slotList.Length - MAX_SLOT_VIEW, 0, slotList.Length);
    }

    private void DisplayItemInfo()
    {
        Item targetItem = inventory.consumableItems[slot_number].item;

        itemIcon.sprite = targetItem.ItemIcon;

        string descriptionTag = targetItem.ItemName + "_Description";
        itemDescription.text = UIControl.instance.uiTextDict[descriptionTag] + DisplayStatusEffect(targetItem);
    }

    public string DisplayStatusEffect(Item item)
    {
        if (item.itemType == ItemType.Consumable) {
            int healAmount = ((Consumable)item).restoreAmount;
            return $"<br><color=#07791A>+ {healAmount} HP</color>";
        }
        return "";
    }
}
