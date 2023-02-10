using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, x_text, countText;
    [SerializeField] private Image slotImage;
    private Color tempColor;
    private string itemBaseName;
    
    void Start()
    {
        tempColor = slotImage.color;
    }

    void OnEnable()
    {
        UIControl.instance.SetFontData(nameText, "Item_Slot_Name");
        string itemName = UIControl.instance.uiTextDict[itemBaseName];
        nameText.text = itemName;
    }

    public void SetData(ItemQuantity itemQuantity)
    {
        itemBaseName = itemQuantity.item.ItemName;

        UIControl.instance.SetFontData(nameText, "Item_Slot_Name");
        string itemName = UIControl.instance.uiTextDict[itemBaseName];
        nameText.text = itemName;
        countText.text = $"{itemQuantity.count}";
    }

    public void Deselect(Color32 textColor)
    {
        tempColor.a = 0f;
        slotImage.color = tempColor;

        nameText.color = x_text.color = countText.color = textColor;
    }

    public void Select(Color32 textColor)
    {
        tempColor.a = 1f;
        slotImage.color = tempColor;

        nameText.color = x_text.color = countText.color = textColor;
    }
}
