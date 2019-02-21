using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour {

	/* Controls the players inventory */

	[SerializeField] private int gold; //how much gold the player owns
	[SerializeField] private Text goldText; //HUD value to display gold count
    [SerializeField] private int keys; //how many keys the player has
    [SerializeField] private Text keyText; //HUD value to display key count
    [SerializeField] private int inventorySize; //how many items to store!
    [SerializeField] private InventoryUIController UI; //UI of the inventory
    private Item[] equipment; //current equipment of the player
    [SerializeField] private EquipmentSlotController[] equipmentSlots; //slots that hold the players equipment
    [SerializeField] private PlayerStatController playerStats; //reference to the players stat controller
    [SerializeField] private List<Item> items = new List<Item>(); //list of items currently in the inventory
    private bool buffed = false; //whether the player if buffed or not

	//initialises HUD element
	void Start(){
		goldText.text = "" + gold;
        keyText.text = "" + keys;
        equipment = new Item[8];
	}

    //picks an item up (if there's space)
    //returns true if successful, or false if there's not enough space for the item
    public bool PickupItem(Item item) {
        if (items.Count < inventorySize) {
            items.Add(item);
            UI.UpdateUI(items);
            return true;
        } else return false;
    }

    //tries to buy an item
    //returns 0 if purchase is successful, 1 if the player has no room 
    //and 2 if the player doesn't have enough money
    public int BuyItem(Item item) {
        if (items.Count < inventorySize && gold >= item.getBuyPrice()) {
            gold -= (int)item.getBuyPrice();
            items.Add(item);
            UI.UpdateUI(items);
            return 0;
        } else if (items.Count == inventorySize) return 1;
        else return 2;
    }

    //remove an item from the inventory
    public void RemoveItem(Item item) {
        items.Remove(item);
        UI.UpdateUI(items);
    }

	//adds / deducts gold from the player
	public void EditGold(int change){
		gold += change;
		goldText.text = "" + gold;
	}

    //equips the item given
    public void EquipItem(Item item) {
        if (equipment[item.getEquipmentSlot()] != item) {
            if (equipment[item.getEquipmentSlot()] != null) {
                Item unequippedItem = equipmentSlots[item.getEquipmentSlot()].setEquipment(item);
                equipment[item.getEquipmentSlot()] = item;
                ModifyStats(item, 1);
                ModifyStats(unequippedItem, -1);
                RemoveItem(item);
                PickupItem(unequippedItem);
            } else {
                ModifyStats(item, 1);
                equipment[item.getEquipmentSlot()] = item;
                equipmentSlots[item.getEquipmentSlot()].setEquipment(item);
                RemoveItem(item);
            }
        } else {
            equipmentSlots[item.getEquipmentSlot()].AlreadyEquippedItem();
        }
    }

    //consumes a persistent buff item
    public void ConsumeItem(Item item) {
        for(int i = 0; i < item.getStats().Length; i++) {
            ModifyStats(item, 1);
        }
    }

    //consumes a temporary buff item
    public bool ConsumeItem(Item item, float duration) {
        bool consumed = false;
        if (!buffed) {
            consumed = true;
            StartCoroutine(Buff(item, duration));
        }
        return consumed;
    }

    //buffs the player for the duration specified
    public IEnumerator Buff(Item item, float duration) {
        buffed = true;
        ModifyStats(item, 1);
        yield return new WaitForSeconds(duration);
        ModifyStats(item, -1);
        buffed = false;
    }

    //modifies the players stats
    public void ModifyStats(Item item, float multiplier) {
        for (int i = 0; i < item.getStats().Length; i++) {
            playerStats.BuffPlayer(item.getStat(i).stat, item.getStat(i).modifier * multiplier);
        }
    }

    //adds a key to the players inventory
    public void AddKey() {
        keys++;
        keyText.text = "" + keys;
    }

    //returns true if a key can be used
    public bool UseKey() {
        if (keys > 0) {
            keys--;
            keyText.text = "" + keys;
            return true;
        } else return false;
    }

    //gets the players gold count
    public int getGold() {
        Debug.Log("Getting player gold");
        return gold;
    }
}
