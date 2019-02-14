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
    [SerializeField] private Image[] icons;

    [SerializeField] private SpellBase[] baseRunes; //holds details of each spell base rune (fire, ice, rock etc)
    [SerializeField] private SpellBase[] formRunes; //holds details of each spell form rune
    [SerializeField] private SpellModifier[] effectRunes; //holds details of each spell effect rune
    [SerializeField] private SpellModifier[] kineticRunes; //holds details of each spell kinetic rune
    [SerializeField] private SpellModifier[] augmentRunes; //holds details of each spell augment rune

    [SerializeField] private Sprite blankIcon; //blank icon
    private string[] spellDetails = new string[8]; //holds the current runes used in the spell (displayed in the GUI)
    [SerializeField] private Text[] spellText; //the text objects to hold the spell details
    private int currentDetail; //current detail being changed
    private int currentRunePage = 0; //the current rune page

    [SerializeField] private Spell spell; //current spell being crafted

    private int currentEffectSlot; //the current effect slot 
	private int currentAugmentSlot; //the current augment slot
	private int currentKineticSlot; //the current kinetic slot
	private int currentIconSlot; //the current icon slot
	private bool basePicked; //if the base spell has been picked
	private bool formPicked; //if the spell form has been picked
	private bool iconPicked; //if a spell icon has been picked

    //casts the spell if a base, form and icon have been picked, assigning it to the current spell slot in the SpellController
    public void Cast(){
		if (basePicked) {
			if (formPicked) {
				if (iconPicked) {
					CalculateSpellStats ();
					spellController.Cast (spell);
					pauseMenu.Resume ();
					ResetSpell ();
				} else toolTipText.text = "Please pick an Icon first";
			} else toolTipText.text = "Please pick a spell form first";
		} else toolTipText.text = "Please pick a base spell first";
	}

	//sets the rune page to the base spells page
	public void BaseSpell(int iconSlot){
		SwitchRunePage (0);
		currentIconSlot = iconSlot;
	}

	//sets the rune page to the spell forms page
	public void Form(int iconSlot){
		if (basePicked) {
			SwitchRunePage (1);
			currentIconSlot = iconSlot;
		} else {
			toolTipText.text = "Please pick a base spell first...";
		}
	}

	//sets the rune page to the spell kinetics page
	public void Kinetic(int kineticSlot, int iconSlot){
		if (basePicked) {
			if (formPicked) {
				SwitchRunePage (2);
				currentKineticSlot = kineticSlot;
				currentIconSlot = iconSlot;
			} else {
				toolTipText.text = "Please pick a spell form first...";
			}
		} else {
			toolTipText.text = "Please pick a base spell first...";
		}
	}

	//sets the rune page to the spell effects page
	public void Effect(int effectSlot, int iconSlot){
		if (basePicked) {
			if (formPicked) {
				SwitchRunePage (3);
				currentEffectSlot = effectSlot;
				currentIconSlot = iconSlot;
			} else {
				toolTipText.text = "Please pick a spell form first...";
			}
		} else {
			toolTipText.text = "Please pick a base spell first...";
		}
	}

	//sets the rune page to the spell augments page
	public void Augment(int augmentSlot, int iconSlot){
		if (basePicked) {
			if (formPicked) {
				SwitchRunePage (4);
				currentAugmentSlot = augmentSlot;
				currentIconSlot = iconSlot;
			} else {
				toolTipText.text = "Please pick a spell form first...";
			}
		} else {
			toolTipText.text = "Please pick a base spell first...";
		}
	}

	//sets the base spell 
	public void SetBase(int baseID, string baseName){
		basePicked = true;
		SwitchHexGrid (baseID);
		spellDetails [0] = baseName;
		CalculateSpellStats ();
	}

	//sets the spell form of the spell
	public void SetForm(int formID, string formName){
		ClearSpell ();
        formPicked = true;
        icons [currentIconSlot].sprite = formRunes[formID].icon;
        spell.spellForm = formID;
        spellDetails [1] = formName;
		CalculateSpellStats ();
	}

	//sets an augment of the spell
	public void SetAugment(int augmentID, string augmentName){
        spell.spellAugments[currentAugmentSlot] = augmentID;
        icons [currentIconSlot].sprite = augmentRunes[augmentID].icon;
		spellDetails [currentDetail] = augmentName;
		CalculateSpellStats ();
	}

	//sets a kinetic of the spell
	public void SetKinetic(int kineticID, string kineticName){
        spell.spellKinetics[currentKineticSlot] = kineticID;
        icons [currentIconSlot].sprite = kineticRunes[kineticID].icon;
		spellDetails [currentDetail] = kineticName;
		CalculateSpellStats ();
	}

	//sets an effect of the spell
	public void SetEffect(int effectID, string effectName){
        spell.spellEffects[currentEffectSlot] = effectID;
        icons [currentIconSlot].sprite = effectRunes[effectID].icon;
		spellDetails [currentDetail] = effectName;
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
	private void SwitchHexGrid(int newHexGridNumber){
		hexGrids [spell.spellBase].SetActive (false);
        spell.spellBase = newHexGridNumber;
		hexGrids [spell.spellBase].SetActive (true);
		ClearSpell ();
	}

	//clears the current spell
	public void ClearSpell(){

        formPicked = false;
        spell.spellForm = -1;

		for (int i = 0; i < 3; i++) {
            spell.spellEffects[i] = -1;
            spell.spellKinetics[i] = -1;
            spell.spellAugments[i] = -1;
        }

        for (int i = 0; i < icons.Length; i++) {
			if (i % 6 != 0) {
				icons [i].sprite = blankIcon; 
			}
		}

		for(int i = 1; i < 6; i++){
			spellDetails[i] = "";
		}
		UpdateText();
	}

	//resets the current spell
	public void ResetSpell(){
		basePicked = false;
		formPicked = false;
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
		CalculateSpellStats ();
		spell.icon = blankIcon;
		displayIcon.sprite = blankIcon;

		for(int i = 0; i < 6; i++){
			spellDetails[i] = "";
		}
		UpdateText();
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
        if (basePicked && formPicked) {

            CalculateManaCost();
            spell.cooldown = formRunes[spell.spellForm].cooldown * baseRunes[spell.spellBase].cooldown;
            spell.power = formRunes[spell.spellForm].power * baseRunes[spell.spellBase].power;
            spell.speed = formRunes[spell.spellForm].speed * baseRunes[spell.spellBase].speed;
            spell.accuracy = formRunes[spell.spellForm].accuracy * baseRunes[spell.spellBase].accuracy;
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
	}

	//updates the spell text in the crafting UI
    //**TODO: Make this show spell stats (damage, accuracy etc etc)
	private void UpdateText(){
		int currentDetail = 0;

		for(int i = 0; i < 6; i++){
			spellText[i].text = "";
		}

		for (int i = 0; i < 6; i++) {
			if (spellDetails [i] != "") {
				spellText [currentDetail].text = spellDetails [i];
				currentDetail++;
			}
		}
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

	//sets on of the spell details in the crafting UI
	public void SetDetail(int detailID){
		currentDetail = detailID;
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