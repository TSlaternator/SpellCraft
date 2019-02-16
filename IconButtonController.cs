using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconButtonController : MonoBehaviour {

	/* controls the effect of icon buttons that allow the player to choose icons
	 * for their spells */

	[SerializeField] private int iconID; //the ID of this icon
	[SerializeField] private SpellCraftMenu menu; //reference to the spellcraft menu

	//sets the icon of the current spell
	public void SetIcon(){
		menu.SetIcon (iconID);
	}
}
