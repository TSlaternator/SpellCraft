using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemQuickSlotController : MonoBehaviour
{
    private Item item; //the item in the slot
    [SerializeField] Image icon; //icon to show which item is in the slot
    [SerializeField] Sprite blankIcon; //blank icon for when no consumable is in the slot
    [SerializeField] PlayerInventoryController inventory; //the players inventory
    [SerializeField] GameObject[] scrolls; //array to hold scroll types (can't assign them to the scrolls since they're scriptable objects)
    [SerializeField] GameObject player; //the player object

    //adds an item to the quick slot
    public void AddItem(Item newItem) {
        item = newItem;
        icon.sprite = item.getSprite();
    }

    //uses the item in the quick slot
    public void UseItem() {
        if (item != null) {
            if (item.getScrollID() == -1) {
                if (inventory.ConsumeItem(item, item.getDuration())) {
                    inventory.RemoveItem(item);
                    icon.sprite = blankIcon;
                    item = null;
                }
            } else {
                inventory.RemoveItem(item);
                icon.sprite = blankIcon;
                GameObject scroll = Instantiate(scrolls[item.getScrollID()], player.transform.position, Quaternion.identity, player.transform);
                scroll.GetComponent<IScrollController>().OnCast();
                item = null;
            }
        }
    }
}
