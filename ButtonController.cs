using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

	/* Controls buttons which determine which hex is being filled */

	[SerializeField] private SpellCraftMenu spellMenu; //reference to the SpellCraftMenu
	[SerializeField] private int buttonID; //ID of the current button
	[SerializeField] private int buttonTypeNumber; //What type of button this is (Effect, Kinetic, etc)
	[SerializeField] private int hexID; //ID of this button within it's hex grid

	//sets the hex being changed to a base hex
	public void Base(){
		spellMenu.BaseSpell (buttonID);
	}
		
	//sets the hex being changed to a form hex
	public void Form(){
		spellMenu.Form (buttonID);
	}
		
	//sets the hex being changed to an effect hex
	public void Effect(){
		spellMenu.Effect (buttonTypeNumber, buttonID);
		spellMenu.SetDetail (hexID);
	}

	//sets the hex being changed to a kinetic hex
	public void Kinetic(){
		spellMenu.Kinetic (buttonTypeNumber, buttonID);
		spellMenu.SetDetail (hexID);
	}

	//sets the hex being changed to an augment hex
	public void Augment(){
		spellMenu.Augment (buttonTypeNumber, buttonID);
		spellMenu.SetDetail (hexID);
	}

}
