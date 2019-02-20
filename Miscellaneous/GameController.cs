using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	/* Controls overarching game effects */

	[SerializeField] private Texture2D cursorSprite; //new sprite to use as the in game cursor
    [SerializeField] private GameObject chestUI; //UI for chests
    [SerializeField] private PauseMenu pauseMenu; //Pausemenu controller scripts
    [SerializeField] private ChestSlotController[] chestSlots; //chest slots on the UI
    [SerializeField] private PlayerInventoryController inventory; //controls the players inventory

    //sets the cursor to the custom one
	void Start () {
		Cursor.SetCursor (cursorSprite, Vector2.zero, CursorMode.Auto);
	}

    //gets the chest UI object
    public GameObject getChestUI() {
        return chestUI;
    }

    //gets the chest slots
    public ChestSlotController[] getChestSlots() {
        return chestSlots;
    }

    //gets the pause menu
    public PauseMenu getPauseMenu() {
        return pauseMenu;
    }

    //gets the players inventory controller
    public PlayerInventoryController getPlayerInventory() {
        return inventory;
    }
}
