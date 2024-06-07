using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Sprite defaultImage, selectedImage, defaultButton, selectedButton;
    [SerializeField] private TextMeshProUGUI itemName, itemPrice, itemCount, cartText;
    [SerializeField] private Image icon_image, slot_image, button_image;
    [SerializeField] private GameObject countObj, cartArrows, cartLeftArrow, cartRightArrow;
    [SerializeField] private Color32 highlightColor, defaultColor;
    private int price;
    private string itemBaseName; public string item_base_name => itemBaseName;
    private int stockCount = 0; public int stock_count => stockCount;

    public void SetData(ShopItem shopItem, bool sellMode = false)
    {
        UIControl.instance.SetFontData(itemName, "Shop_Slot_Name");

        icon_image.sprite = shopItem.item.ItemIcon;
        
        itemBaseName = shopItem.item.ItemName;
        itemName.text = TextDB.Translate(itemBaseName, TranslationType.UI);

        price = shopItem.itemPrice;
        itemPrice.text = price.ToString();
        
        if (sellMode)
            ShowCount(shopItem.count);
    }

    public void Select()
    {
        slot_image.sprite = selectedImage;
        button_image.sprite = selectedButton;
    }

    public void Deselect()
    {
        slot_image.sprite = defaultImage;
        button_image.sprite = defaultButton;
    }

    public void ShowCount(int count)
    {
        countObj.SetActive(true);
        itemCount.text = count.ToString();
    }

    public void CartArrowToggle(bool state)
    {
        cartArrows.SetActive(state);
        CartArrowControl();
    }

    private void CartArrowControl()
    {
        cartLeftArrow.SetActive(stockCount >= 1);
        cartRightArrow.SetActive((stockCount < 999) && ShopSystem.instance.IsAffordable(price));
    }

    private void ButtonColorControl()
    {
        if (stockCount == 0)
            cartText.color = defaultColor;
        
        else if (stockCount > 0)
            cartText.color = highlightColor;
    }

    public void DecreaseCart()
    {
        if (stockCount == 0)
            return;
        
        stockCount -= 1;
        ShopSystem.instance.UpdateTotalPrice(-price);

        cartText.text = stockCount.ToString();
        CartArrowControl();
        ButtonColorControl();
    }

    public void IncreaseCart()
    {
        if ((stockCount == 999) || !ShopSystem.instance.IsAffordable(price))
            return;
        
        stockCount += 1;
        ShopSystem.instance.UpdateTotalPrice(price);

        cartText.text = stockCount.ToString();
        CartArrowControl();
        ButtonColorControl();
    }

    public void ClearStock()
    {
        stockCount = 0;
        cartText.text = stockCount.ToString();
        CartArrowControl();
        ButtonColorControl();
    }
}
