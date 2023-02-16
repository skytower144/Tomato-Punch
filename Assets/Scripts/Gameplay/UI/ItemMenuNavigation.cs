using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ItemMenuNavigation : MonoBehaviour
{
    const int MAX_SLOT_VIEW = 7;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private GameObject uiGuide, arrowGuide, upArrow, downArrow, itemPrompt, usedPrompt;
    [SerializeField] private Color32 highlightTextColor, defaultTextColor;
    private ItemSlotUI[] slotList;
    private int slot_number = 0;
    private float slot_height;
    private float end_of_view;
    private bool isNavigating = false; public bool is_navigating => isNavigating;

    [SerializeField] private bool isConsumableMenu;
    private bool isPrompt = false;
    private List<ItemQuantity> targetItemList;

    void Start()
    {
        if (isConsumableMenu)
            targetItemList = inventory.consumableItems;
        else
            targetItemList = inventory.otherItems;
        
        slot_height = inventory.inventory_UI.itemslot_prefab.GetComponent<RectTransform>().rect.height;
    }

    void OnEnable()
    {
        slot_number = 0;
        isNavigating = itemIcon.enabled = false;

        itemDescription.text = UIControl.instance.uiTextDict["InventoryGuide"];
        UIControl.instance.SetFontData(itemDescription, "Item_Description");

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

        else if(isNavigating && !isPrompt)
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
                if (isConsumableMenu && (slotList.Length > 0)) {
                    PromptUseItem();
                }
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
                inventory.inventory_UI.DisplayItemInfo(targetItemList[0].item, itemDescription, itemIcon);
            }
            else {
                itemIcon.enabled = false;
                slotList[slot_number].Deselect(defaultTextColor);
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
        
        // Considering when slotList.Length equals to 0
        slot_number = Mathf.Clamp(slot_number, 0, Mathf.Clamp(slotList.Length - 1, 0, slotList.Length - 1));

        if (prev_num == slot_number)
            return;

        HandleScroll();
        slotList[prev_num].Deselect(defaultTextColor);
        slotList[slot_number].Select(highlightTextColor);
        inventory.inventory_UI.DisplayItemInfo(targetItemList[slot_number].item, itemDescription, itemIcon);
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

    private void PromptUseItem()
    {
        isPrompt = true;
        GameObject temp = Instantiate(itemPrompt, transform);
        temp.GetComponent<RectTransform>().localPosition = new Vector2(-399, -2.2f);
        temp.GetComponent<ConfirmPrompt>().InitializeData(UseItem, CancelPrompt, "UseItemPrompt");
    }
    private void CancelPrompt()
    {
        isPrompt = false;
    }

    private void UseItem()
    {
        Item item = targetItemList[slot_number].item;
        ItemUseInfo info = item.Use(GameManager.gm_instance.battle_system.tomato_control);

        if (info.isUsed) {
            inventory.RemoveItem(item, targetSlotNumber: slot_number);
            GameManager.gm_instance.battle_system.tomatostatus.OnEnable();
            PopupPrompt(info.effectInfo, info.reactAnimName);
        }
        CancelPrompt();
    }
    private void PopupPrompt(string main_text, string animTag)
    {
        GameObject temp = Instantiate(usedPrompt, transform);
        PromptPopup popup = temp.GetComponent<PromptPopup>();
        popup.popupText.text = main_text;
        popup.animTag = animTag;
    }

    public IEnumerator UpdateSlotValues()
    {
        yield return new WaitForEndOfFrame();
        slotList = itemSlotParent.GetComponentsInChildren<ItemSlotUI>(true);
        UpdateEndOfViewValue();

        yield return new WaitForEndOfFrame();

        slot_number = Mathf.Clamp(slot_number, 0, Mathf.Clamp(slotList.Length - 1, 0, slotList.Length - 1));

        if (slotList.Length == 0)
            itemIcon.enabled = false;
        else if ((slotList.Length > 0) && isNavigating) {
            slotList[slot_number].Select(highlightTextColor);
            inventory.inventory_UI.DisplayItemInfo(targetItemList[slot_number].item, itemDescription, itemIcon);
        }
    }
}
