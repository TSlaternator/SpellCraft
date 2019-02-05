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
	[SerializeField] private Sprite[] baseIcons; //icons for each base spell
	[SerializeField] private float[] baseCooldowns; //cooldowns for each base spell
	[SerializeField] private float[] baseManaCosts; //mana costs for each base spell
	[SerializeField] private float[] basePowers; //powers for each base spell
	[SerializeField] private float[] baseSpeeds; //speeds for each base spell
	[SerializeField] private float[] baseAccuracies; //accuracies for each base spell
	[SerializeField] private float[] baseImpacts; //impacts for each base spell
	[SerializeField] private float[] baseCritChances; //crit chance for each base spell
	[SerializeField] private float[] baseCritMultipliers; //crit multiplier for each base spell

	[SerializeField] private Sprite[] formIcons; //icons for each spell form
	[SerializeField] private float[] formManaCosts; //mana costs for each spell form
	[SerializeField] private float[] formCooldowns; //cooldowns for each spell form
	[SerializeField] private float[] formPowers; //powers of each spell form
	[SerializeField] private float[] formSpeeds; //speeds of each spell form
	[SerializeField] private float[] formAccuracies; //accuracies for each spell form
	[SerializeField] private float[] formImpacts; //impacts for each spell form
	[SerializeField] private float[] formCritChances;  //crit chances for each spell form
	[SerializeField] private float[] formCritMultipliers; //crit multipliers for each spell form
	[SerializeField] private Sprite[] effectIcons; //icons for each effect
	[SerializeField] private float[] effectManaCosts; //mana costs for each sprite
	[SerializeField] private Sprite[] kineticIcons; //icons for each kinetic 
	[SerializeField] private float[] kineticManaCosts; //mana costs for each kinetic
	[SerializeField] private Sprite[] augmentIcons; //icons for each augment
	[SerializeField] private float[] augmentManaCosts; //mana costs for each augment
	[SerializeField] private Sprite blankIcon; //blank icon
	private string[] spellDetails; //holds the current runes used in the spell (displayed in the GUI)
	[SerializeField] private Text[] spellText; //the text objects to hold the spell details
	private int currentDetail; //current detail being changed

	private int currentRunePage; //the current rune page
	private int baseSpell;  //the base spell being used
	private int spellForm; //the spell form being used
	private int[] spellEffects; //the spell effets being used
	private int[] spellAugments; //the spell augments being used
	private int[] spellKinetics; //the spell kinetics being used
	private Sprite currentIcon; //the current Icon of the spell
	private float cooldown; //the current cooldown of the spell
	private float manaCost; //the current mana cost of the spell
	private float accuracy; //the current accuracy of the spell
	private float power; //the current power of the spell
	private float speed; //the current speed of the spell
	private float stagger; //the current impact of the spell
	private float critChance; //the current crit chance of the spell 
	private float critPower; //the current crit power of the spell
	private float[] spellStats; //holds the 8 variables above
	private float powerMultiplier; //TODO Figure out what this is
	private int currentEffectSlot; //the current effect slot 
	private int currentAugmentSlot; //the current augment slot
	private int currentKineticSlot; //the current kinetic slot
	private int currentIconSlot; //the current icon slot
	private bool basePicked; //if the base spell has been picked
	private bool formPicked; //if the spell form has been picked
	private bool iconPicked; //if a spell icon has been picked

	// Use this for initialization
	void Start () {
		currentRunePage = 0;
		baseSpell = 0;
		spellForm = -1;
		spellEffects = new int[3];
		spellAugments = new int[3];
		spellKinetics = new int[3];
		spellDetails = new string[6];
		currentIcon = blankIcon;

		for (int i = 0; i < 3; i++) {
			spellEffects [i] = -1;
			spellAugments [i] = -1;
			spellKinetics [i] = -1;
		}

		spellStats = new float[8];
	}

	//casts the spell if a base, form and icon have been picked, assigning it to the current spell slot in the SpellController
	public void Cast(){
		if (basePicked) {
			if (formPicked) {
				if (iconPicked) {
					CalculateSpellStats ();
					spellController.Cast (baseSpell, spellForm, spellKinetics, spellAugments, spellEffects, spellStats, currentIcon);
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
		formPicked = true;
		ClearSpell ();
		icons [currentIconSlot].sprite = formIcons [formID];
		spellForm = formID;
		spellDetails [1] = formName;
		CalculateSpellStats ();
	}

	//sets an augment of the spell
	public void SetAugment(int augmentID, string augmentName){
		spellAugments [currentAugmentSlot] = augmentID;
		icons [currentIconSlot].sprite = augmentIcons [augmentID];
		spellDetails [currentDetail] = augmentName;
		CalculateSpellStats ();
	}

	//sets a kinetic of the spell
	public void SetKinetic(int kineticID, string kineticName){
		spellKinetics [currentKineticSlot] = kineticID;
		icons [currentIconSlot].sprite = kineticIcons [kineticID];
		spellDetails [currentDetail] = kineticName;
		CalculateSpellStats ();
	}

	//sets an effect of the spell
	public void SetEffect(int effectID, string effectName){
		spellEffects [currentEffectSlot] = effectID;
		icons [currentIconSlot].sprite = effectIcons [effectID];
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
		hexGrids [baseSpell].SetActive (false);
		baseSpell = newHexGridNumber;
		hexGrids [baseSpell].SetActive (true);
		ClearSpell ();
	}

	//clears the current spell
	public void ClearSpell(){

		spellForm = 0;

		for (int i = 0; i < 3; i++) {
			spellEffects [i] = -1;
			spellAugments [i] = -1;
			spellKinetics [i] = -1;
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
		hexGrids [baseSpell].SetActive (false);
		baseSpell = 0;
		spellForm = 0;
		for (int i = 0; i < 3; i++) {
			spellEffects [i] = -1;
			spellAugments [i] = -1;
			spellKinetics [i] = -1;
		}
		SwitchRunePage (0);
		for (int i = 0; i < icons.Length; i++) {
			if (i % 6 != 0) {
				icons [i].sprite = blankIcon; 
			}
		}
		CalculateSpellStats ();
		currentIcon = blankIcon;
		displayIcon.sprite = blankIcon;

		for(int i = 0; i < 6; i++){
			spellDetails[i] = "";
		}
		UpdateText();
	}

	//calculates the mana cost of the spell
	private void CalculateManaCost(){

		if (spellForm != -1) {
			manaCost = formManaCosts [spellForm] * baseManaCosts [baseSpell];
			for (int i = 0; i < 3; i++) {
				if (spellEffects [i] != -1)
					manaCost *= effectManaCosts [spellEffects [i]];
				if (spellKinetics [i] != -1)
					manaCost *= kineticManaCosts [spellKinetics [i]];
				if (spellAugments [i] != -1)
					manaCost *= augmentManaCosts [spellAugments [i]];
			}
		}
	}

	//calculates the spells stats
	private void CalculateSpellStats(){
		
		CalculateManaCost ();
		cooldown = formCooldowns [spellForm] * baseCooldowns [baseSpell];
		power = formPowers[spellForm] * basePowers [baseSpell];
		speed = formSpeeds[spellForm] * baseSpeeds [baseSpell];
		accuracy = formAccuracies[spellForm] * baseAccuracies [baseSpell];
		stagger = formImpacts[spellForm] * baseImpacts [baseSpell];
		critChance = formCritChances[spellForm] * baseCritChances [baseSpell];
		critPower = formCritMultipliers[spellForm] * baseCritMultipliers [baseSpell];

		for (int i = 0; i < 3; i++) {
			switch (spellAugments [i]) {
			case 0: power *= 1.25f; break;
			case 1: power *= 0.8f; break;
			case 2: speed *= 1.5f; break;
			case 3: speed *= 0.67f; break;
			case 4: accuracy -= 2f; break;
			case 5: accuracy += 2f; break;
			case 6: stagger *= 2f; break;
			case 7: stagger *= 0.5f; break;
			case 8: critChance *= 2f; break;
			case 9: critChance *= 0.5f; break;
			case 10: critPower *= 2f; break;
			case 11: critPower *= 0.5f; break;
			case 12:
				power *= 1.1f; 
				speed *= 1.2f;
				accuracy -= 1f;
				stagger *= 1.5f;
				critChance *= 1.5f;
				critPower *= 1.5f; break;
			case 13: 
				power *= 0.91f; 
				speed *= 0.82f;
				accuracy += 1f; 
				stagger *= 0.67f;
				critChance *= 0.67f;
				critPower *= 0.67f; break;	
			case 14: cooldown *= 1.5f; break;
			case 15: cooldown *= 0.67f; break;
			}
		}

		spellStats [0] = manaCost;
		spellStats [1] = cooldown;
		spellStats [2] = power;
		spellStats [3] = speed;
		spellStats [4] = 100f - accuracy;
		spellStats [5] = stagger;
		spellStats [6] = critChance;
		spellStats [7] = critPower;

		UpdateText ();
	}

	//updates the spell text in the crafting UI
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
		currentIcon = playerIcons [iconID];
		displayIcon.sprite = currentIcon;
	}

	//sets on of the spell details in the crafting UI
	public void SetDetail(int detailID){
		currentDetail = detailID;
	}
}
