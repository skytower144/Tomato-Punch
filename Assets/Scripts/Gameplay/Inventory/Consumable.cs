using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    [Header("Food Type")]
    public FoodType foodType;

    [Header("HP")]
    public int restoreAmount;
    public bool restoreFullHp;

    public override ItemUseInfo Use(tomatoControl tomatoControl)
    {
        ItemUseInfo info = new ItemUseInfo();
        info.isUsed = true;
        info.reactAnimName = ReturnMatchingReaction();

        if (restoreFullHp || restoreAmount > 0) {
            tomatoControl.currentHealth = Mathf.Clamp(tomatoControl.currentHealth + restoreAmount, 1, tomatoControl.maxHealth);
            string popuptext = UIControl.instance.uiTextDict["RestoreHealth"];
            info.effectInfo = popuptext.Replace("?", restoreAmount.ToString());
        }
        else
            return null;
        
        return info;
    }

    public string ReturnMatchingReaction()
    {
        switch (foodType) {
            case FoodType.Munch:
                return "munching";
            
            case FoodType.Sip:
                return "sipping";
            
            default:
                break;
        }
        return "munching";
    }
}

public enum FoodType { Munch, Sip, Favorite }
