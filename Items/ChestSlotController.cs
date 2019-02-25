using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestSlotController : MonoBehaviour
{
    [SerializeField] private Sprite blankIcon; //image used when the slot is empty
    [SerializeField] private Image icon; //the image in the UI that will show the items sprite
    [SerializeField] private PlayerInventoryController inventory; //link to the inventory
    [SerializeField] private ChestController chest; //link to the chest holding this slot
    private Item item; //the item in the slot

    //Adds an item to the item slot
    public void AddItem(Item newItem) {
        item = newItem;
        icon.sprite = item.getSprite();
    }

    //Removes the item from the item slot
    public void RemoveItem() {
        if (item != null) {
            item.setInventoryController(inventory);
            if (inventory.PickupItem(item)) chest.RemoveItem(item);
        }
    }

    //empties the slot
    public void EmptySlot() {
        item = null;
        icon.sprite = blankIcon;
    }

    //sets the currently active chest
    public void setChest(ChestController newChest) {
        chest = newChest;
    }
}
