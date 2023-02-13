using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    private ShopSystem shopSystem;
    public List<ShopItem> shopItems = new List<ShopItem>();

    public void ViewShopUI(Action input_action)
    {
        shopSystem = UIControl.instance.ui_shop;
        shopSystem.proceedAction = input_action;

        shopSystem.shopItems = shopItems;

        shopSystem.gameObject.SetActive(true);
        StartCoroutine(shopSystem.PrepareUI());
    }

}

[System.Serializable]
public class ShopItem : ItemQuantity
{
    public int itemPrice;
}
