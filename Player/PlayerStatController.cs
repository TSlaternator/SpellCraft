using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatController : MonoBehaviour {

	/* Controls the players stats and allows things to affect those stats */

	[SerializeField] private float maxHealth; //maximum health of the player
	private float health; //current health of the player
	private float healthPercentage; //current health percentage of the player
	[SerializeField] private Image healthBar; //reference to the players health bar
	[SerializeField] private Text healthText; //reference to the health text in the HUD

	[SerializeField] float maxMana; //maximum mana of the player
	private float mana; //current mana of the player 
	private float manaPercentage; //current mana percentage of the player
	[SerializeField] private Image manaBar; //reference to the players mana bar
	[SerializeField] private Text manaText; //reference to the mana text in the HUD
	[SerializeField] private float manaRechargeRate; //how fast the players mana regenerates
    [SerializeField] private playerMultipliers spellMultipliers; //applied to any spell cast
    [SerializeField] private GameObject buffUI; //UI element to show current buff icons
    [SerializeField] private Image buffIcon; //icon to show current buff

	//initialises stats
	void Start(){
		health = maxHealth;
		mana = maxMana;
        
	}

	//updates current stat percentages and bars, regenerates mana 
	void Update(){
		healthPercentage = health / maxHealth;
		manaPercentage = mana / maxMana;

		healthBar.fillAmount = healthPercentage;
		healthText.text = "" + (int)health;
		manaBar.fillAmount = manaPercentage;
		manaText.text = "" + (int)mana;

		if (!PauseMenu.isPaused && mana < maxMana) {
			RegenerateMana (Time.deltaTime);
		}
	}

	//deals damage to the player
	public void DealDamage(float damage){
		health -= damage;

		if (health < 0f) {
			//TODO
		}
	}

    //Heals the player by the amount provided
    public void Heal(float amount) {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

	//returns the current health of the player
	public float GetHealth(){
		return health;
	}

	//returns the current health percentage
	public float GetHealthPercent(){
		return health / maxHealth;
	}

	//reduces the players current mana
	public void DrainMana(float manaDrained){
		mana -= manaDrained;

		if (mana < 0f) mana = 0f;
	}
		
	//adds mana to the player
	public void AddMana(float amount){
		mana += amount;
		if (mana > maxMana) mana = maxMana;
	}

	//returns the current mana of the player
	public float GetMana(){
		return mana;
	}

	//controls the players mana regeneration
	private void RegenerateMana(float multiplier){
		mana += manaRechargeRate * multiplier;
		if (mana > maxMana) mana = maxMana;
	}

	//sets the players mana recharge rate
	public void SetManaRecharge(float multiplier){
		manaRechargeRate *= multiplier;
	}

    //returns the players spell multipliers
    public playerMultipliers getPlayerMultipliers() {
        return spellMultipliers;
    }

    //buffs the player in some way
    public void BuffPlayer(string stat, float buffAmount) {
        switch (stat) {
            case "power": spellMultipliers.powerMultiplier += buffAmount; break;
            case "speed": spellMultipliers.speedMultiplier += buffAmount; break;
            case "accuracy": spellMultipliers.accuracy += buffAmount; break;
            case "impact": spellMultipliers.impactMultiplier += buffAmount; break;
            case "manaCost": spellMultipliers.manaCostMultiplier += buffAmount; break;
            case "cooldown": spellMultipliers.coolDownMultiplier += buffAmount; break;
            case "critChance": spellMultipliers.critChance += buffAmount; break;
            case "critPower": spellMultipliers.critPower += buffAmount; break;
            case "maxHealth": AddMaxHealth(buffAmount); break;
            case "maxMana": AddMaxMana(buffAmount); break;
            case "health": Heal(buffAmount); break;
            case "mana": AddMana(buffAmount); break;
        }
    }

    //controls the buff icon UI element
    public void setBuffIcon(Sprite icon, bool isActive) {
        buffIcon.sprite = icon;
        buffUI.SetActive(isActive);
    }

    //increases the players maximum health
    public void AddMaxHealth(float amount) {
        maxHealth += amount;
        Heal(amount);
    }

    //increases the players maximum mana pool
    public void AddMaxMana(float amount) {
        maxMana += amount;
        AddMana(amount);
    }
}

//holds player stats that are applied to spells cast
[System.Serializable]
public struct playerMultipliers {
    public float powerMultiplier; //how much to multiply spell power by (base 1x)
    public float speedMultiplier; //how much to multiply spell speed by (base 1x)
    public float accuracy; //how much to add to spell accuracy (base 0)
    public float impactMultiplier; //how much to multiply spell impact by (base 1x)
    public float critChance; //how much to add to spell crit chance (base 0)
    public float critPower; //how much to add to spell crit power (base 0)
    public float manaCostMultiplier; //how much to multiply spell mana cost by (base 1x)
    public float coolDownMultiplier; //how much to multiply spell cooldown by (base 1x)
}
