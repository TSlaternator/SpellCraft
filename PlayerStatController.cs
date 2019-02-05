using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatController : MonoBehaviour {

	/* Controls the players stats and allows things to affect those stats */

	[SerializeField] public float maxHealth; //maximum health of the player
	private float health; //current health of the player
	private float healthPercentage; //current health percentage of the player
	[SerializeField] Image healthBar; //reference to the players health bar
	[SerializeField] Text healthText; //reference to the health text in the HUD

	[SerializeField] float maxMana; //maximum mana of the player
	private float mana; //current mana of the player 
	private float manaPercentage; //current mana percentage of the player
	[SerializeField] Image manaBar; //reference to the players mana bar
	[SerializeField] Text manaText; //reference to the mana text in the HUD
	[SerializeField] float manaRechargeRate; //how fast the players mana regenerates

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
}
