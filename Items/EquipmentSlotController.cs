using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{

    [SerializeField] private Item item = null; //the item in the equipment slot
    [SerializeField] private Image background; //the background image of the equipment slot
    [SerializeField] private Sprite usedSlot; //background sprite for when the slot is being used
    [SerializeField] private Image icon; //the sprite of the equipment slot
    [SerializeField] private Text itemDescription; //holds the description of the currently moused-over object
    [SerializeField] private Text[] itemStats; //holds the stats of the currently moused-over object

    //sets the equipment of the slot
    public Item setEquipment(Item newEquipment) {
        if (newEquipment.getName() == item.getName()) {
            AlreadyEquippedItem();
            return newEquipment;
        } else {
            background.sprite = usedSlot;
            Item unequippedItem = item;
            item = newEquipment;
            icon.sprite = item.getSprite();
            return unequippedItem;
        }
    }

    //gets the equipment of the slot
    public Item getEquipment() {
        return item;
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
        itemDescription.text = "";
        for (int i = 0; i < 8; i++) {
            itemStats[i].text = "";
        }
    }

    //called if the player tries to equip something they alread have equipped!
    public void AlreadyEquippedItem() {
        itemDescription.text = "That item is already equipped!";
    }
}
