using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField] private Sprite blankIcon; //image used when the slot is empty
    [SerializeField] private Image icon; //the image in the UI that will show the items sprite
    [SerializeField] private Button trashButton; //button deletes items when used
    [SerializeField] private PlayerInventoryController inventory; //link to the inventory
    [SerializeField] private Text itemDescription; //description UI object
    [SerializeField] private Text[] itemStats; //stats UI objects
    private Item item; //the item in the slot

    //Adds an item to the icon slot
    public void AddItem(Item newItem) {
        trashButton.interactable = true;
        item = newItem;
        icon.sprite = item.getSprite();
    }

    //Uses the current item (if applicable)
    public void UseItem() {
        if (item != null) {
            item.OnUse();
            ResetDescription();
        }
    }

    //Removes the item in the slot (called when delete button is pressed)
    public void RemoveItem() {
        inventory.RemoveItem(item);
    }

    //Removes an item from the icon slot
    public void EmptySlot() {
        trashButton.interactable = false;
        item = null;
        icon.sprite = blankIcon;
    }

    //Brings up a tooltip when being hovered over
    public void OnPointerEnter(PointerEventData eventData) {
        if (item != null) {
            itemDescription.text = "'" + item.getName() + "' : " + item.getDescription();
            for(int i = 0; i < item.getStats().Length; i++) {
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
