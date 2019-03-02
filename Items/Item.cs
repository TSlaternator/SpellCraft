using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName = "New Item"; //name of the item
    [SerializeField] private Sprite icon = null; //sprite that will be shown in the inventory
    [SerializeField] private int sellPrice = 0; //how much the item sells for
    [SerializeField] private int buyPrice = 0; //how much the shop will sell the item for
    [SerializeField] private string type = "consumable"; //what type of item it is
    [SerializeField] private Stat[] stats; //stat changes this consumable will apply 
    [SerializeField] private int equipmentSlot; //equipment slot this item fits into (if applicable)
    [SerializeField] private float duration; //duration of this consumable's buff (if applicable)
    [SerializeField] private PlayerInventoryController inventory; //used when the item is consumed
    [SerializeField] private string description; //description of the item
    [SerializeField] private int scrollID = -1; //which scroll this is (if applicable)

    //sets the inventory controller up
    public void setInventoryController(PlayerInventoryController controller) {
        inventory = controller;
    }

    //what to do when the item is picked up
    public void OnPickup() {
        Debug.Log("Picked up: " + itemName + " (" + type + ")");
    }

    //what to do when the item is used in the inventory
    public void OnUse(ItemQuickSlotController quickSlot) {
        if (type == "equippable") inventory.EquipItem(this);
        else if (type == "persistentConsumable") {
            inventory.ConsumeItem(this);
            inventory.RemoveItem(this);
        } else if (type == "consumable") {
            quickSlot.AddItem(this);
        }
    }

    //gets the name of the item
    public string getName() {
        return itemName;
    }

    //gets the sprite of the item
    public Sprite getSprite() {
        return icon;
    }

    //gets the sell price of the item
    public float getSellPrice() {
        return sellPrice;
    }

    //gets the buy price of the item
    public float getBuyPrice() {
        return buyPrice;
    }

    //gets the type of the item
    public string getType() {
        return type;
    }

    //gets the items description
    public string getDescription() {
        return description;
    }

    //gets the items equipment slot
    public int getEquipmentSlot() {
        return equipmentSlot;
    }

    //gets the items stats
    public Stat[] getStats() {
        return stats;
    }

    //gets a specific stat
    public Stat getStat(int stat) {
        return stats[stat];
    }
    
    //gets the duration of the item
    public float getDuration() {
        return duration;
    }

    //gets the scrollID of the item
    public int getScrollID() {
        return scrollID;
    }
}

[System.Serializable]
public struct Stat {
    public string stat; //the stat to affect
    public float modifier; //how much to affect it by
}
