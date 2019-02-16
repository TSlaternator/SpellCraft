using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatController : MonoBehaviour {

	/* Controls enemy stats and any effects that interact with the enemies stats
	 * such as damage, debuffs etc */

	[SerializeField] private float maxHP; //maximum HP of the enemy
	[SerializeField] private float fireResistance; //fire resistance of the enemy
	[SerializeField] private float waterResistance; //water resistance of the enemy
	[SerializeField] private float airResistance; //air resistance of the enemy
	[SerializeField] private float earthResistance; //earth resistance of the enemy
	[SerializeField] private float orderResistance; //order resistance of the enemy
	[SerializeField] private float chaosResistance; //chaos resistance of the enemy
	[SerializeField] private GameObject[] FloatingDamageText; //damage numbers used when enemies take damage
	[SerializeField] private Transform textList; //clearnup list to spawn the text under
	[SerializeField] private Transform floatingTextSpawn; //spawn point of the floating text
	[SerializeField] private float textSpawnOffset; //random horizontal offset of the texts spawn point
	[SerializeField] private GameObject manaDrop; //mana orb dropped upon death
	[SerializeField] private GameObject goldDrop; //gold orb dropped upon death
	[SerializeField] private int manaDropped; //how much mana to drop upon death
	[SerializeField] private int goldDropped; //how much gold to drop upon death
	[SerializeField] private Animator animator; //sprite animator
	[SerializeField] private AnimationClip hitAnimation; //animation when hit
	[SerializeField] private Collider col;

	private bool marked; //if the enemy is marked or not (marked enemies take more damage from certain elements)
	private int markType; //the element that marked the enemy (will take more damage from all elements except this one)
	private float markMultiplier; //the multiplier applied to mark damage
	private float markDuration; //the duration of the mark effect

	private bool alchemised; //if the enemy is being alchemised, it drops more gold as it dies
	private float alchemisedDuration; //the duration of the alchemised effect
	private float alchemyModifier; //the multiplier for gold dropped

	private bool crystalised; //if the enemy is being crystalised, it drops more mana as it dies
	private float crystalisedDuration; //the duraction of the crystalised effect
	private float crystaliseModifier; // the multiplier for mana dropped

	private float currentHP; //current hp of the enemy
	private float[] resistances; //holds the resistances of the enemy
	private Transform gameController; //transform always facing forwards (used to spawn damage text)

	private bool disintegrating; //if the enemy is disintegrating, it drops no rewards
	private float disintegrateDuration; //how long until the enemy is no longer disintegrating

	private bool weakened; //if the enemy is weak to an element
	private float weaknessModifier; //how much to effect the resistance
	private int weaknessTo; //what element the enemy is weak to
	private float weaknessDuration; //how long the enemy is weakened for

	private bool hasDoT;  //if the enemy has a DoT effect on it
	private float DoTDuration; //the duration of the DoT

	private bool isDead;

	//initialises variables and references
	void Start(){
		currentHP = maxHP;
		resistances = new float[] { fireResistance, waterResistance, airResistance, earthResistance, orderResistance, chaosResistance, 0f};
		textList = GameObject.Find ("DamageNumbersList").transform;
		gameController = GameObject.Find ("GameController").transform;
	}

	//controls time based effects such as marks
	void Update(){
		if (!isDead) {
			if (marked && Time.time > markDuration)
				marked = false;
			if (alchemised && Time.time > alchemisedDuration)
				alchemised = false;
			if (crystalised && Time.time > crystalisedDuration)
				crystalised = false;
			if (disintegrating && Time.time > disintegrateDuration)
				disintegrating = false;
			if (weakened && Time.time > weaknessDuration)
				EndWeakness ();
			if (hasDoT && Time.time > DoTDuration)
				hasDoT = false;
		}
	}

	//applies damage to the enemy
	public void ApplyDamage(float damage, int damageType, bool isCrit, bool isDoT){
		float realDamage;
		if (marked && damageType != markType) realDamage = damage * markMultiplier * (1 - (resistances [damageType] / 100));
		else realDamage = damage * (1 - (resistances [damageType] / 100));
		realDamage = Mathf.Round (realDamage);
		currentHP -= realDamage;
		if (isDoT) DamageText (realDamage, 2);
		else if (isCrit) DamageText (realDamage, 1);
		else DamageText (realDamage, 0);

		StartCoroutine (HitAnimation());

		if (currentHP <= 0) Die ();
	}

	//kills the enemy, dropping rewards
	private void Die(){
        col.enabled = false;
		animator.SetBool ("IsDead", true);
		isDead = true;
		gameObject.GetComponent<EnemyController> ().isDead = true;
		if (!disintegrating) {
			if (alchemised)
				DropPickups ((int)(goldDropped * alchemyModifier), goldDrop);
			else
				DropPickups (goldDropped, goldDrop);
			if (crystalised)
				DropPickups ((int)(manaDropped * crystaliseModifier), manaDrop);
			else
				DropPickups (manaDropped, manaDrop);
		}
        gameObject.transform.SetParent(null);
		StartCoroutine (CorpseTime ());
	}

    //How long the enemy will stay visible as a corpse before despawning
	private IEnumerator CorpseTime(){
		yield return new WaitForSeconds (5f);
		Destroy (gameObject);
	}

	//drops pickups upon death
	private void DropPickups(int amount, GameObject pickup){
		for (int i = 0; i < amount; i++) {
			Instantiate (pickup, transform.position + GenerateOffset(), transform.rotation);
		}
	}


	//drains mana from the enemy
	public void ManaDrain(){
		Instantiate (manaDrop, transform.position + GenerateOffset (), transform.rotation);
	}

	//generates a random offset to spawn pickups at
	private Vector3 GenerateOffset(){
		float offsetX = Random.Range (-0.5f, 0.5f);
		float offsetZ = Random.Range (-0.5f, 0.5f);
		return new Vector3 (offsetX, 0f, offsetZ);
	}

	//spawns the damage number text
	private void DamageText(float damageNumber, int textType){
		GameObject text;
		float offsetX = Random.Range (-textSpawnOffset, textSpawnOffset);
		Vector3 textSpawnPosition = new Vector3 (floatingTextSpawn.position.x + offsetX, floatingTextSpawn.position.y, floatingTextSpawn.position.z);
		text = Instantiate(FloatingDamageText[textType], textSpawnPosition, gameController.rotation, textList);
		text.GetComponent<DamageTextController> ().SetText (damageNumber.ToString());
	}

	//marks the enemy with an element, buffing all other elements' damage
	public void Mark(int type, float multiplier){
		marked = true;
		markType = type;
		markMultiplier = multiplier;
		markDuration = Time.time + 5f;
	}

	//causes the enemy to drop more gold as it dies
	public void Alchemise(float multiplier){
		alchemised = true;
		alchemisedDuration = Time.time + 1f;
		alchemyModifier = multiplier;
	}

	//causes the enemy to drop more mana as it dies
	public void Crystalise(float multiplier){
		crystalised = true;
		crystalisedDuration = Time.time + 1f;
		crystaliseModifier = multiplier;
	}

	//returns the health percentage
	public float GetHealthPercent(){
		return currentHP / maxHP;
	}

	//returns the current health
	public float GetHealth(){
		return currentHP;
	}

	//Takes extra damage but doesn't drop pickups
	public void Disintegrate(){
		disintegrating = true;
		disintegrateDuration = Time.time + 5f;
	}

	//Lowers resistances
	public void Weaken(int weakElement, int weakModifier){
		if (!weakened) {
			weakened = true;
			weaknessTo = weakElement;
			weaknessModifier = weakModifier;
			resistances [weaknessTo] -= weaknessModifier;
		}
		weaknessDuration = Time.time + 5f;
	}

	//Ends the weakness effect
	private void EndWeakness(){
		weakened = false;
		resistances [weaknessTo] += weaknessModifier;
		weaknessModifier = 1f;
	}

	//Applies a DoT to the enemy
	public void ApplyDoT(int damageType, float damage){
		Debug.Log ("DOT");
		if (!hasDoT) {
			hasDoT = true;
			StartCoroutine (DoT (damageType, damage, 1f));
		}
		DoTDuration = Time.time + 5.5f;
	}

	//Applies the DoT's effects
	private IEnumerator DoT(int damageType, float damage, float interval){
		while (hasDoT) {
			yield return new WaitForSeconds (interval);
			ApplyDamage(damage, damageType, false, true);
		}
	}

    //Plays an animation when hit
	private IEnumerator HitAnimation(){
		animator.SetBool ("IsHit", true);
		yield return new WaitForSeconds (hitAnimation.length -0.01f);
		animator.SetBool ("IsHit", false);
	}
}
