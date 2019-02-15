using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotController : MonoBehaviour {

	/* Controls the players spell slots */

	[SerializeField] private int slotID; //the ID of the spell slot
	[SerializeField] private Image spellIcon; //the Image component of the spell slot
	[SerializeField] private Image previousSpell; //the Image component of the previous spell slot
	[SerializeField] private Image nextSpell; //the Image component of the next spell slot
    [SerializeField] private Spell currentSpell; //spell held by this spell slot

    //Sets the spell held by this spell slot
    public void setSpell(Spell spell) {
        currentSpell = spell;
        currentSpell.spellEffects = (int[])spell.spellEffects.Clone();
        currentSpell.spellAugments = (int[])spell.spellAugments.Clone();
        currentSpell.spellKinetics = (int[])spell.spellKinetics.Clone();
    }

    //gets the spell held by this spell slot
    public Spell getSpell() {
        return currentSpell;
    }

	//sets the sprite of the spell in the HUD
	public void ChangeSprite(){
		spellIcon.sprite = currentSpell.icon;
	}

    //sets the sprite of the previous spell in the HUD
	public void ChangePreviousSprite(){
		previousSpell.sprite = currentSpell.icon;
	}

    //sets the sprite of the next spell in the HUD
	public void ChangeNextSprite(){
		nextSpell.sprite = currentSpell.icon;
	}
}
