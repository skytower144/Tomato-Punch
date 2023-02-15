using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, x_text, countText;
    [SerializeField] private Image slotImage;
    private Color tempColor;
    private string itemBaseName;

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(itemBaseName)) {
            UIControl.instance.SetFontData(nameText, "Item_Slot_Name");
            nameText.text = UIControl.instance.uiTextDict[itemBaseName];
        }
    }

    public void SetData(ItemQuantity itemQuantity)
    {
        UIControl.instance.SetFontData(nameText, "Item_Slot_Name");

        tempColor = slotImage.color;
        itemBaseName = itemQuantity.item.ItemName;
        nameText.text = UIControl.instance.uiTextDict[itemBaseName];
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
