using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotController : MonoBehaviour
{
    [SerializeField] private Sprite blankIcon; //image used when the slot is empty
    [SerializeField] private Image icon; //the image in the UI that will show the items sprite
    [SerializeField] private PlayerInventoryController inventory; //link to the inventory
    [SerializeField] private ShopController shop; //link to the chest holding this slot
    [SerializeField] private Text price; //displays the items price
    [SerializeField] private Text shopKeeperText; //displays messages from the shopkeeper
    [SerializeField] private GameController gameController; //the game controller class
    private Item item; //the item in the slot

    //Adds an item to the item slot
    public void AddItem(Item newItem) {
        item = newItem;
        icon.sprite = item.getSprite();
        price.text = item.getBuyPrice() + " gold";
    }

    //Removes the item from the item slot
    public void BuyItem() {
        item.setInventoryController(inventory);
        int result = inventory.BuyItem(item);
        if (result == 0) {
            shop.RemoveItem(item);
        }
        shopKeeperText.text = gameController.getShopKeeperQuote(result);
    }

    //empties the slot
    public void EmptySlot() {
        item = null;
        icon.sprite = blankIcon;
        price.text = "";
    }

    //sets the currently active chest
    public void setShop(ShopController newShop) {
        shop = newShop;
    }
}
