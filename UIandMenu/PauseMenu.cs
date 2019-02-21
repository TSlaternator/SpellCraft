using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	/* Controls the pause menu and its effects */

	public static bool isPaused; //whether or not the game is paused
	[SerializeField] private GameObject pauseMenuUI; //reference to the pauseMenu UI panel
	[SerializeField] private GameObject spellMenuUI; //reference to the spell menu UI panel
	[SerializeField] private SpellCraftMenu spellMenu; //reference to the spell crafting menu
	[SerializeField] private Texture2D cursorSprite; //custom cursor sprite used in the game
	[SerializeField] private GameObject hudUI; //reference to the HUD UI
    [SerializeField] private GameObject inventoryUI; //reference to the inventory UI
    [SerializeField] private GameObject shopUI; //reference to the shop UI
    [SerializeField] private SpellController spellController; //reference to the spell controller script
    [SerializeField] private SpellCraftMenu spellCraftMenu; //reference to the spell craft menu script
    [SerializeField] private InventoryUIController inventoryController; //reference to the inventorycontroller script
    [SerializeField] private GameObject ChestUI; //chest UI

	//pauses (or unpauses) the game when ESCAPE is pressed
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (isPaused) {
				Resume ();
			} else {
				Pause();
			}
		}
	}

	//resumes the game
	public void Resume(){
		spellMenu.ResetSpell ();
		pauseMenuUI.SetActive (false);
		spellMenuUI.SetActive (false);
        inventoryUI.SetActive(false);
        ChestUI.SetActive(false);
        inventoryController.CloseInventory();
		hudUI.SetActive (true);
        shopUI.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
		Cursor.SetCursor (cursorSprite, Vector2.zero, CursorMode.Auto);
	}

	//pauses the game
	public void Pause(){
		pauseMenuUI.SetActive (true);
		hudUI.SetActive (false);
        inventoryUI.SetActive(false);
        ChestUI.SetActive(false);
        shopUI.SetActive(false);
		Time.timeScale = 0f;
		isPaused = true;
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
	}

	//opens the spell crafting menu
	public void SpellMenu(){
		spellMenuUI.SetActive (true);
        spellCraftMenu.LoadSpell(spellController.getSpellSlot());
		pauseMenuUI.SetActive (false);
	}

	//quits the game
	public void QuitGame(){
		Application.Quit ();
	}

    //returns if the game is paused or not
    public bool IsPaused() {
        return isPaused;
    }

    //Sets isPaused to true
    public void setPaused(bool paused) {
        isPaused = paused;
    }
}
