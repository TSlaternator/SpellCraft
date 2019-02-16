using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneButtonController : MonoBehaviour {

	/* Controls the effects of rune buttons in the crafting menu */

	[SerializeField] private SpellCraftMenu menu; //reference to the spell crafting menu
	[SerializeField] private bool augment; //true if the rune is an augment
	[SerializeField] private bool kinetic; //true if the rune is a kinetic
	[SerializeField] private bool effect; //true if the rune is an effect
	[SerializeField] private bool form; //true if the rune is a form
	[SerializeField] private bool baseSpell; //true if the rune is a spell base
	[SerializeField] private string name; //the name of the rune
	[SerializeField] private int runeID; //the ID of the rune within its type (augment, kinetic etc)

	//updates the current hex with the rune this button is controlling
	public void PressButton(){
		if (augment) menu.SetAugment (runeID, name);
		else if (kinetic) menu.SetKinetic (runeID, name);
		else if (effect) menu.SetEffect (runeID, name);
		else if (form) menu.SetForm (runeID, name);
		else if (baseSpell) menu.SetBase (runeID, name);
	}
}
