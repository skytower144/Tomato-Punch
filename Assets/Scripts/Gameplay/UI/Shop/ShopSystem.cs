using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using DG.Tweening;

public enum ShopInteraction { ConfirmPurchase, ExitShop, IsShopping, ContinueShopping }

public class ShopSystem : MonoBehaviour
{
    const int MAX_SLOT_VIEW = 4;
    private GameManager gameManager;
    private PlayerMovement playerMovement;
    private tomatoStatus tomato_status;
    private Inventory inventory;
    public static ShopSystem instance { get; private set; }

    [System.NonSerialized] public ShopInteraction shopInteraction = ShopInteraction.IsShopping;
    public Action proceedAction;

    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription, clearText, totalPriceText, playerMoneyText;
    [SerializeField] private GameObject itemSlotPrefab, arrowBundle, upArrow, downArrow, clearGuide, dollarEffect;
    [SerializeField] public List<ShopItem> shopItems = new List<ShopItem>(); // Initialize when instantiated
    [SerializeField] private ShopSlot[] slotList;

    private int totalPrice = 0; public int total_price => totalPrice;

    private int slot_number = 0;
    private float slot_height;
    private float end_of_view;

    private float DECREASE_SPEED = 90f;
    private float shrinkingPlayerMoney, finalPlayerMoney;
    private bool runMoneyTimer = false;

    private bool isNavigating = false; public bool is_navigating => isNavigating;
    [System.NonSerialized] public bool isSellMode = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Shop System in the scene.");
            return;
        }
        instance = this;

        gameManager = GameManager.gm_instance;
        playerMovement = gameManager.player_movement;
        inventory = gameManager.playerInventory;
        tomato_status = GameManager.gm_instance.battle_system.tomatostatus;

        slot_height = itemSlotPrefab.GetComponent<RectTransform>().rect.height;
    }

    public IEnumerator PrepareUI()
    {
        shopInteraction = ShopInteraction.IsShopping;

        DOTween.Rewind("ShopAppear");
        DOTween.Play("ShopAppear");

        UIControl.instance.SetFontData(itemDescription, "Shop_Item_Description");
        inventory.inventory_UI.DisplayItemInfo(shopItems[0].item, itemDescription, itemIcon);

        UIControl.instance.SetFontData(clearText, "Shop_Clear_Text");
        clearText.text = UIControl.instance.uiTextDict["Shop_Clear_Text"];
        slot_number = 0;

        yield return StartCoroutine(UpdateSlots());

        ClearCart();
        ShowPlayerMoney();

        UpdateEndOfViewValue();
        slotList[0].Select();
        slotList[0].CartArrowToggle(true);
        ArrowControl();

        isNavigating = true;
    }

    void OnDisable()
    {
        slotList[slot_number].Deselect();
        ResetSlotListPosition();
        DeleteSlots();
        shopItems = new List<ShopItem>();
    }

    void Update()
    {
        if(isNavigating && (shopInteraction == ShopInteraction.IsShopping))
        {
            if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                gameManager.DetectHolding(UINavigate);
            }
            else if (gameManager.WasHolding)
            {
                gameManager.holdStartTime = float.MaxValue;
            }
            else if(playerMovement.Press_Key("LeftPage"))
            {
                ClearCart();
            }
            else if(playerMovement.Press_Key("Interact") && (total_price > 0))
            {
                if (runMoneyTimer)
                    StopAnimateMoney();
                PromptPayment();
            }
            else if(playerMovement.Press_Key("Cancel"))
            {
                shopInteraction = ShopInteraction.ExitShop;
                proceedAction?.Invoke();
            }
        }

        if (runMoneyTimer)
        {
            AnimatePlayerMoney();
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

        else if (direction == InputDir.LEFT)
            slotList[slot_number].DecreaseCart();

        else if (direction == InputDir.RIGHT)
            slotList[slot_number].IncreaseCart();
        
        slot_number = Mathf.Clamp(slot_number, 0, Mathf.Clamp(shopItems.Count - 1, 0, shopItems.Count));

        if (prev_num == slot_number)
            return;

        HandleScroll();
        slotList[prev_num].Deselect();
        slotList[slot_number].Select();
        inventory.inventory_UI.DisplayItemInfo(shopItems[slot_number].item, itemDescription, itemIcon);

        slotList[prev_num].CartArrowToggle(false);
        slotList[slot_number].CartArrowToggle(true);
    }
    IEnumerator UpdateSlots()
    {
        if (itemSlotParent.childCount > 0)
            DeleteSlots();
        
        for (int i = 0; i < shopItems.Count; i++) {
            GameObject itemSlot = Instantiate(itemSlotPrefab, itemSlotParent);
            itemSlot.GetComponent<ShopSlot>().SetData(shopItems[i], isSellMode);
        }
        yield return new WaitForEndOfFrame();
        slotList = itemSlotParent.GetComponentsInChildren<ShopSlot>(true);
    }

    private void DeleteSlots()
    {
        foreach (Transform child in itemSlotParent) {
            Destroy(child.gameObject);
        }
        slotList = new ShopSlot[0];
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
        bool showDownArrow = slot_number + MAX_SLOT_VIEW / 2 < shopItems.Count - 1;
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

    public bool IsAffordable(int viewingItemPrice)
    {
        return tomato_status.CheckEnoughMoney(totalPrice + viewingItemPrice);
    }
    private void ShowPlayerMoney(bool isAnimate = false)
    {
        if (!isAnimate) {
            playerMoneyText.text = tomato_status.playerMoney.ToString();
            return;
        }

        shrinkingPlayerMoney = tomato_status.playerMoney;
        finalPlayerMoney = shrinkingPlayerMoney - totalPrice;
        playerMoneyText.color = new Color32(207, 58, 68, 255);
        runMoneyTimer = true;
    }

    private void AnimatePlayerMoney()
    {
        shrinkingPlayerMoney -= Time.deltaTime * DECREASE_SPEED;

        if (shrinkingPlayerMoney <= finalPlayerMoney)
        {
            StopAnimateMoney();
        }
        playerMoneyText.text = shrinkingPlayerMoney.ToString("F0");
    }

    private void StopAnimateMoney()
    {
        runMoneyTimer = false;
        shrinkingPlayerMoney = finalPlayerMoney;
        playerMoneyText.color = new Color32(67, 35, 35, 255);
        playerMoneyText.text = shrinkingPlayerMoney.ToString("F0");
    }

    public void UpdateTotalPrice(int amount)
    {
        totalPrice += amount;
        totalPriceText.text = totalPrice.ToString();
    }
    private void ClearCart()
    {
        totalPrice = 0;
        totalPriceText.text = totalPrice.ToString();

        for (int i = 0; i < slotList.Length; i++) {
            slotList[i].ClearStock();
        }
    }

    public void PurchaseOneItem(string itemAndPrice)
    {
        string[] itemInfo = itemAndPrice.Split('-');
        if (itemInfo.Length != 2)
            return;
        
        string itemName = itemInfo[0].Trim();
        int itemPrice = int.Parse(itemInfo[1].Trim());

        tomatoStatus tomatostatus = GameManager.gm_instance.battle_system.tomatostatus;
        Inventory playerInventory = GameManager.gm_instance.playerInventory;
        
        if (tomatostatus.CheckEnoughMoney(itemPrice)) {
            playerInventory.AddItem(Item.ReturnMatchingItem(itemName));
            tomatostatus.UpdatePlayerMoney(-itemPrice);
        }   
    }

    private void PromptPayment()
    {
        clearGuide.SetActive(false);
        arrowBundle.SetActive(false);
        slotList[slot_number].CartArrowToggle(false);

        shopInteraction = ShopInteraction.ConfirmPurchase;
        proceedAction?.Invoke();
    }

    public void ProceedPayment()
    {
        for (int i = 0; i < slotList.Length; i++) {
            if (slotList[i].stock_count > 0)
                inventory.AddItem(Item.ReturnMatchingItem(slotList[i].item_base_name), slotList[i].stock_count);
        }
        ShowPlayerMoney(true);
        tomato_status.UpdatePlayerMoney(-totalPrice);

        GameObject temp = Instantiate(dollarEffect, transform);
        Destroy(temp, 0.8f);

        ClearCart();
    }

    public void ContinueShopping()
    {
        clearGuide.SetActive(true);
        arrowBundle.SetActive(true);
        slotList[slot_number].CartArrowToggle(true);

        shopInteraction = ShopInteraction.IsShopping;
    }
}
