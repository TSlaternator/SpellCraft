using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour {

	/* Controls the players inventory */

	[SerializeField] private float gold; //how much gold the player owns
	[SerializeField] private Text goldText; //HUD value to display gold count
    [SerializeField] private int inventorySize; //how many items to store!
    [SerializeField] private InventoryUIController UI; //UI of the inventory
    private List<Item> items = new List<Item>(); //list of items currently in the inventory

	//initialises HUD element
	void Start(){
		goldText.text = "" + gold;
	}

    //picks an item up (if there's space)
    //returns true if successful, or false if there's not enough space for the item
    public bool PickupItem(Item item) {
        Debug.Log("Picking up item: " + item.name);
        if (items.Count < inventorySize) {
            items.Add(item);
            UI.UpdateUI(items);
            return true;
        } else return false;
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
}
