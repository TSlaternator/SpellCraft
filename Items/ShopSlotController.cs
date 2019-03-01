using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopSlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
    {
    [SerializeField] private Sprite blankIcon; //image used when the slot is empty
    [SerializeField] private Image icon; //the image in the UI that will show the items sprite
    [SerializeField] private PlayerInventoryController inventory; //link to the inventory
    [SerializeField] private ShopController shop; //link to the chest holding this slot
    [SerializeField] private Text price; //displays the items price
    [SerializeField] private Text shopKeeperText; //displays messages from the shopkeeper
    [SerializeField] private GameController gameController; //the game controller class
    [SerializeField] private Text itemDescription; //description UI object
    [SerializeField] private Text[] itemStats; //stats UI objects
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

    //Brings up a tooltip when being hovered over
    public void OnPointerEnter(PointerEventData eventData) {
        if (item != null) {
            itemDescription.text = "'" + item.getName() + "' : " + item.getDescription();
            for (int i = 0; i < item.getStats().Length; i++) {
                itemStats[i].text = item.getStat(i).stat + ": " + item.getStat(i).modifier;
            }
        }
    }

    //Removes the tooltip when the mouse moves away
    public void OnPointerExit(PointerEventData eventData) {
        ResetDescription();
    }

    //resets the items descriptions and stats panel in the UI
    public void ResetDescription() {
        itemDescription.text = "";
        for (int i = 0; i < 8; i++) {
            itemStats[i].text = "";
        }
    }
}
