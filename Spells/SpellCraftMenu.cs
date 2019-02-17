using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCraftMenu : MonoBehaviour {

	/* Controls the crafting menu for spells */

	[SerializeField] private SpellController spellController; //reference to the players SpellController component
	[SerializeField] private PauseMenu pauseMenu; //reference to the pause menu
	[SerializeField] private GameObject[] hexGrids; //holds the individual hex grids
	[SerializeField] private GameObject[] runePages; //hold the individual rune pages
	[SerializeField] private GameObject[] extraRunePages; //TODO REFACTOR THIS
	[SerializeField] private GameObject iconPanel; //panel to hold player choice icons
	[SerializeField] private GameObject runePanel; //panel to hold runes 
	[SerializeField] private GameObject tooltipPanel; //panel to hold tooltips
	[SerializeField] private Sprite[] playerIcons; //holds all the player choice icons
	[SerializeField] private Image displayIcon; //icon being displayed
	[SerializeField] private Text toolTipText; //the tooltip object
    [SerializeField] private Image[] icons; //array of icon slots on the hex grid
    [SerializeField] private HexGrid[] hexes; //array of hexgrid structs (used when loading spells)

    [SerializeField] private SpellBase[] baseRunes; //holds details of each spell base rune (fire, ice, rock etc)
    [SerializeField] private SpellBase[] formRunes; //holds details of each spell form rune
    [SerializeField] private SpellModifier[] effectRunes; //holds details of each spell effect rune
    [SerializeField] private SpellModifier[] kineticRunes; //holds details of each spell kinetic rune
    [SerializeField] private SpellModifier[] augmentRunes; //holds details of each spell augment rune
    [SerializeField] private SpellSlotController[] spellSlots; //holds the quick-slots spells are stored in

    [SerializeField] private Sprite blankIcon; //blank icon
    [SerializeField] private Text[] spellText; //the text objects to hold the spell details
    private int currentRunePage = 0; //the current rune page

    [SerializeField] private Spell spell; //current spell being crafted
    private int currentSpellSlot; //current spell slot to write into

    private int currentEffectSlot; //the current effect slot 
	private int currentAugmentSlot; //the current augment slot
	private int currentKineticSlot; //the current kinetic slot
	private int currentIconSlot; //the current icon slot
	private bool iconPicked; //if a spell icon has been picked

    //casts the spell if a base, form and icon have been picked, assigning it to the current spell slot in the SpellController
    public void Cast(){
		if (iconPicked) {
			CalculateSpellStats ();
            spellSlots[currentSpellSlot].setSpell(spell);
            /*
			spellController.Cast (spell);
			pauseMenu.Resume ();
			ResetSpell (); */
		} else toolTipText.text = "Please pick an Icon first";
	}

    //Exits the menu;
    public void ExitMenu() {
        spellController.SwitchSpellSlot(currentSpellSlot);
        pauseMenu.Resume();
        ResetSpell();
    }

    //Loads the currently equipped spell into the crafting interface
    public void LoadSpell(Spell equippedSpell) {

        if (equippedSpell.icon != null) iconPicked = true;

        //Setting base and form icons in the crafting UI
        spell = equippedSpell;
        SetBase(equippedSpell.spellBase, false);
        SetForm(equippedSpell.spellForm, hexes[spell.spellBase].formID, false);

        //Loading spell variables into the spell being crafted
        spell = equippedSpell;
        spell.spellEffects = (int[])equippedSpell.spellEffects.Clone();
        spell.spellAugments = (int[])equippedSpell.spellAugments.Clone();
        spell.spellKinetics = (int[])equippedSpell.spellKinetics.Clone();

        //Setting crafting UI elements to those of the spell loaded
        for (int i = 0; i < 3; i++) {
            if (spell.spellEffects[i] != -1) SetEffect(spell.spellEffects[i], hexes[spell.spellBase].effectIDs[i]);
            if (spell.spellKinetics[i] != -1) SetKinetic(spell.spellKinetics[i], hexes[spell.spellBase].kineticIDs[i]);
            if (spell.spellAugments[i] != -1) SetAugment(spell.spellAugments[i], hexes[spell.spellBase].augmentIDs[i]);
        }

        if (spell.icon != null) displayIcon.sprite = spell.icon;
        CalculateSpellStats();
    }

    //Loads the spell of the specified spell slot
    public void LoadSpell(int spellSlot) {
        Debug.Log("Loading spell: " + spellSlot);

        currentSpellSlot = spellSlot;
        Spell equippedSpell = spellSlots[spellSlot].getSpell();

        if (equippedSpell.icon != null) iconPicked = true;

        //Setting base and form icons in the crafting UI
        spell = equippedSpell;
        SetBase(equippedSpell.spellBase, false);
        SetForm(equippedSpell.spellForm, hexes[spell.spellBase].formID, false);

        //Loading spell variables into the spell being crafted
        spell = equippedSpell;
        spell.spellEffects = (int[])equippedSpell.spellEffects.Clone();
        spell.spellAugments = (int[])equippedSpell.spellAugments.Clone();
        spell.spellKinetics = (int[])equippedSpell.spellKinetics.Clone();

        //Setting crafting UI elements to those of the spell loaded
        for (int i = 0; i < 3; i++) {
            if (spell.spellEffects[i] != -1) SetEffect(spell.spellEffects[i], hexes[spell.spellBase].effectIDs[i]);
            if (spell.spellKinetics[i] != -1) SetKinetic(spell.spellKinetics[i], hexes[spell.spellBase].kineticIDs[i]);
            if (spell.spellAugments[i] != -1) SetAugment(spell.spellAugments[i], hexes[spell.spellBase].augmentIDs[i]);
        }

        if (spell.icon != null) displayIcon.sprite = spell.icon;
        CalculateSpellStats();
    }

    //sets the rune page to the base spells page
    public void BaseSpell(int iconSlot){
		SwitchRunePage (0);
		currentIconSlot = iconSlot;
	}

	//sets the rune page to the spell forms page
	public void Form(int iconSlot){
		SwitchRunePage (1);
		currentIconSlot = iconSlot;
	}

	//sets the rune page to the spell kinetics page
	public void Kinetic(int kineticSlot, int iconSlot){
		SwitchRunePage (2);
		currentKineticSlot = kineticSlot;
		currentIconSlot = iconSlot;
	}

	//sets the rune page to the spell effects page
	public void Effect(int effectSlot, int iconSlot){
		SwitchRunePage (3);
		currentEffectSlot = effectSlot;
		currentIconSlot = iconSlot;
	}

	//sets the rune page to the spell augments page
	public void Augment(int augmentSlot, int iconSlot){
		SwitchRunePage (4);
		currentAugmentSlot = augmentSlot;
		currentIconSlot = iconSlot;
	}

	//sets the base spell 
    public void SetBase(int baseID, bool calculateStats) {
        SwitchHexGrid(baseID, calculateStats);
        if (calculateStats) CalculateSpellStats();
    }

	//sets the spell form of the spell
    public void SetForm(int formID, int iconID, bool calculateStats) {
        if (iconID != -1) icons[iconID].sprite = formRunes[formID].icon;
        else {
            icons[currentIconSlot].sprite = formRunes[formID].icon;
            spell.spellForm = formID;
        }
        if (calculateStats) CalculateSpellStats();
    }

	//sets an augment of the spell
    public void SetAugment(int augmentID, int iconID) {
        if (iconID != -1) icons[iconID].sprite = augmentRunes[augmentID].icon;
        else {
            icons[currentIconSlot].sprite = augmentRunes[augmentID].icon;
            spell.spellAugments[currentAugmentSlot] = augmentID;
        }
        CalculateSpellStats();
    }

	//sets a kinetic of the spell
    public void SetKinetic(int kineticID, int iconID) {
        if (iconID != -1) icons[iconID].sprite = kineticRunes[kineticID].icon;
        else {
            icons[currentIconSlot].sprite = kineticRunes[kineticID].icon;
            spell.spellKinetics[currentKineticSlot] = kineticID;
        }
        CalculateSpellStats();
    }

	//sets an effect of the spell
    public void SetEffect(int effectID, int iconID) {
        if (iconID != -1) icons[iconID].sprite = effectRunes[effectID].icon;
        else {
            icons[currentIconSlot].sprite = effectRunes[effectID].icon;
            spell.spellEffects[currentEffectSlot] = effectID;
        }
        CalculateSpellStats ();
    }

	//switches the current rune page being displayed
	private void SwitchRunePage(int newPageNumber){
		runePages [currentRunePage].SetActive (false);
		for (int i = 0; i < extraRunePages.Length; i++) {
			extraRunePages [i].SetActive (false);
		}
		currentRunePage = newPageNumber;
		runePages [currentRunePage].SetActive (true);
		IconPanelInactive ();
	}

	//switches the current hex grid being displayed
	private void SwitchHexGrid(int newHexGridNumber, bool clearSpell){
        for (int i = 0; i < hexGrids.Length; i++) hexGrids[i].SetActive(false);
        spell.spellBase = newHexGridNumber;
		hexGrids [spell.spellBase].SetActive (true);
		if(clearSpell) ClearSpell ();
	}

	//clears the current spell
	public void ClearSpell(){

        //clearing runes
		for (int i = 0; i < 3; i++) {
            spell.spellEffects[i] = -1;
            spell.spellKinetics[i] = -1;
            spell.spellAugments[i] = -1;
        }

        //cleaning ui elements
        for (int i = 0; i < icons.Length; i++) {
			if (i % 6 != 0) {
				icons [i].sprite = blankIcon; 
			}
		}

        //updating ui text
        SetForm(spell.spellForm, hexes[spell.spellBase].formID, false);
		UpdateText();
	}

	//resets the current spell
	public void ResetSpell(){
		iconPicked = false;
		hexGrids [spell.spellBase].SetActive (false);
        spell.spellBase = 0;
        spell.spellForm = -1;
        for (int i = 0; i < 3; i++) {
            spell.spellEffects[i] = -1;
            spell.spellKinetics[i] = -1;
            spell.spellAugments[i] = -1;
        }
        SwitchRunePage (0);
		for (int i = 0; i < icons.Length; i++) {
			if (i % 6 != 0) {
				icons [i].sprite = blankIcon; 
			}
		}
		spell.icon = blankIcon;
		displayIcon.sprite = blankIcon;
	}

	//calculates the mana cost of the spell
	private void CalculateManaCost(){

		if (spell.spellForm != -1) {
			spell.manaCost = formRunes[spell.spellForm].manaCost * baseRunes[spell.spellBase].manaCost;
			for (int i = 0; i < 3; i++) {
				if (spell.spellEffects [i] != -1) spell.manaCost *= effectRunes[spell.spellEffects[i]].manaCost;
                if (spell.spellKinetics [i] != -1) spell.manaCost *= kineticRunes[spell.spellKinetics[i]].manaCost;
                if (spell.spellAugments [i] != -1) spell.manaCost *= augmentRunes[spell.spellAugments[i]].manaCost;
            }
		}
	}

	//calculates the spells stats
	private void CalculateSpellStats(){

        CalculateManaCost();
        spell.cooldown = formRunes[spell.spellForm].cooldown * baseRunes[spell.spellBase].cooldown;
        spell.power = formRunes[spell.spellForm].power * baseRunes[spell.spellBase].power;
        spell.speed = formRunes[spell.spellForm].speed * baseRunes[spell.spellBase].speed;
        spell.accuracy = formRunes[spell.spellForm].accuracy + baseRunes[spell.spellBase].accuracy;
        spell.impact = formRunes[spell.spellForm].impact * baseRunes[spell.spellBase].impact;
        spell.critChance = formRunes[spell.spellForm].critChance * baseRunes[spell.spellBase].critChance;
        spell.critMultiplier = formRunes[spell.spellForm].critMultiplier * baseRunes[spell.spellBase].critMultiplier;

        for (int i = 0; i < 3; i++) {
            switch (spell.spellAugments[i] % 8) {
                case 0: spell.power *= augmentRunes[spell.spellAugments[i]].modifier; break; //power rune 
                case 1: spell.speed *= augmentRunes[spell.spellAugments[i]].modifier; break; //speed rune
                case 2: spell.accuracy += augmentRunes[spell.spellAugments[i]].modifier; break; //accuracy rune
                case 3: spell.impact *= augmentRunes[spell.spellAugments[i]].modifier; break; //impact rune
                case 4: spell.critChance += augmentRunes[spell.spellAugments[i]].modifier; break; //crit chance rune
                case 5: spell.critMultiplier += augmentRunes[spell.spellAugments[i]].modifier; break; //crit multiplier rune
                case 6: spell.manaCost *= augmentRunes[spell.spellAugments[i]].modifier; break; //mana cost rune
                case 7: spell.cooldown *= augmentRunes[spell.spellAugments[i]].modifier; break; //cooldown rune
            }
        }

        UpdateText(); //update UI Text
	}

	//updates the spell text in the crafting UI
	private void UpdateText(){
        string powerSuffix;
        if (spell.spellForm <= 7) powerSuffix = "";
        else if (spell.spellForm <= 15) powerSuffix = " x3";
        else if (spell.spellForm <= 23) powerSuffix = " x5";
        else powerSuffix = " x8";

        spellText[0].text = spell.manaCost.ToString("N0");
        spellText[1].text = spell.cooldown.ToString("N2");
        spellText[2].text = spell.power.ToString("N0") + powerSuffix;
        spellText[3].text = spell.accuracy.ToString("N0");
        spellText[4].text = spell.speed.ToString("N0");
        spellText[5].text = spell.impact.ToString("N0");
        spellText[6].text = (spell.critChance * 100).ToString("N0") + "%";
        spellText[7].text = (1 + spell.critMultiplier).ToString("N1") + "x";
    }

	//sets the icon panel to active, and the rune and tooltip panels to inactive
	public void IconPanelActive(){
		iconPanel.SetActive (true);
		tooltipPanel.SetActive (false);
		runePanel.SetActive (false);
	}

	//sets the icon panel to inactive, and the rune and tooltip panels to active
	public void IconPanelInactive(){
		iconPanel.SetActive (false);
		tooltipPanel.SetActive (true);
		runePanel.SetActive (true);
	}

	//sets the icon of the spell
	public void SetIcon(int iconID){
		iconPicked = true;
		spell.icon = playerIcons [iconID];
		displayIcon.sprite = spell.icon;
	}
}

/* -----------------------------
 *      Spell Structs 
 * ----------------------------- */

/* Used for spell bases, and spell forms */
[System.Serializable]
public struct SpellBase {
    public Sprite icon; //icon of the base / form
    public float cooldown; //cooldown of the base / form
    public float manaCost; //mana cost of the base / form
    public float power; //power fo the base / form
    public float speed; //speed of the base / form
    public float accuracy; //accuracy of the base / form
    public float impact; //impact of the abse / form
    public float critChance; //crit chance of the base / form
    public float critMultiplier; //crit multiplier of the base / form
}

/* Used for spell effects, kinetics and augments */
[System.Serializable]
public struct SpellModifier {
    public Sprite icon; //icon of the modifier
    public float manaCost; //mana cost of the modifier
    public float modifier; //any extra multiplier the modifier has 
}

/* Contains all spell parameters / effects */
[System.Serializable]
public struct Spell {
    public Sprite icon; //icon of the spell
    public int spellBase; //base rune of the spell
    public int spellForm; //form rune of the spell
    public int[] spellEffects; //effect runes of the spell
    public int[] spellKinetics; //kinetic runes of the spell
    public int[] spellAugments; //augment runes of the spell
    public float cooldown; //cooldown of the spell
    public float manaCost; //mana cost of the spell
    public float power; //power fo the spell
    public float speed; //speed of the spell
    public float accuracy; //accuracy of the spell
    public float impact; //impact of the spell
    public float critChance; //crit chance of the spell
    public float critMultiplier; //crit multiplier of the spell
}

/* Contains the ID numbers for rune slots (used when importing spells) */
[System.Serializable]
public struct HexGrid {
    public int baseID; //id of the base rune slot
    public int formID; //id of the form rune slot
    public int[] effectIDs; //id of the effect rune slot(s)
    public int[] augmentIDs; //id of the augment rune slot(s)
    public int[] kineticIDs; //id of the kinetic rune slot(s)
}