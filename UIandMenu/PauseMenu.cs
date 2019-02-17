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
    [SerializeField] private SpellController spellController; //reference to the spell controller script
    [SerializeField] private SpellCraftMenu spellCraftMenu; //reference to the spell craft menu script

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
		hudUI.SetActive (true);
		Time.timeScale = 1f;
		isPaused = false;
		Cursor.SetCursor (cursorSprite, Vector2.zero, CursorMode.Auto);
	}

	//pauses the game
	public void Pause(){
		pauseMenuUI.SetActive (true);
		hudUI.SetActive (false);
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
}
