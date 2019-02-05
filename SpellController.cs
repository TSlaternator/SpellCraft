using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellController : MonoBehaviour {

	/* Controls the spells the player is casting */

	[SerializeField] private CameraShaker camera; //the cameraShaker object
	[SerializeField] private PlayerStatController playerStats; //reference to the players stats
	[SerializeField] private SpellSlotController[] spellSlots; //reference to the players spell slots
	[SerializeField] private int currentSlot; //index of the current spell slot

	[SerializeField] private Transform[] castPoints; //cast points of the players spells
	[SerializeField] private GameObject[] projectileSpells; //array of the different spell bases
	[SerializeField] private Transform projectileList; //list to tidy up the interface

	private GameObject currentSpell; //the current spell being cast
	private int currentBase; //the base of the current spell
	private int currentForm; //the form of the current spell
	private int[] currentKinetics = new int[3]; //the kinetics of the current spell
	private int[] currentAugments = new int[3]; //the augments of the current spell
	private int[] currentEffects = new int[3]; //the effects of the current spell
	private float[] currentStats = new float[8]; //the stats of the current spell
	private int consecutiveCasts; //how many times the current spell has been cast consecutively
	private float cooldown; //the cooldown of the current spell
	private float manaCost; //the mana cost of the current spell
	private float baseAccuracy; //the accuracy of the current spell before modifiers
	private float realAccuracy; //the accuracy of the current spell after modifiers

	/* DO I EVEN WANT THIS STUFF???? */
	private bool[] consecutiveStats = new bool[8];
	private int[] consecutiveStatFactors = new int[8];
	private float[] consecutiveLerpSmall = new float[11];
	private float[] consecutiveLerpMedium = new float[11];
	private float[] consecutiveLerpLarge = new float[11];
	private float[] consecutiveLerpInverse = new float[11];
	/* MAYBE TIERS OF AUGMENTS INSTEAD?? */

	private float nextCast; //next time a spell can be cast
	private bool casting; //if the player is holding down the cast button
	private float lastCast; //last time a spell was cast
	private bool meditating; //if the player is meditating or not ******************* MAYBE MOVE THIS TO STAT CONTROLLER ***********************
	private bool vengefulSpell; //if a spell is vengeful or not
	private float vengefulModifier = 1f; //the modifier of vengefuls spells (NEED TO RE DO THIS)

	//initialising variables
	void Start(){
		currentBase = 8;
		currentForm = 0;

		for (int i = 0; i < 3; i++) {
			currentKinetics [i] = -1;
			currentAugments [i] = -1;
			currentEffects [i] = -1;
		}

		for (int i = 0; i < 11; i++) {
			consecutiveLerpSmall [i] = Mathf.Lerp (1f, 1.35f, i / 10f);
			consecutiveLerpMedium [i] = Mathf.Lerp (1f, 1.7f, i / 10f);
			consecutiveLerpLarge [i] = Mathf.Lerp (1f, 2.4f, i / 10f);
			consecutiveLerpInverse [i] = Mathf.Lerp (1f, 0.5f, i / 10f);
		}

		lastCast = Time.time;
		nextCast = Time.time;
		cooldown = 1f;
		manaCost = 0f;
		baseAccuracy = 5f;
		realAccuracy = baseAccuracy;
		currentSlot = 0;
	}

	//controls the players casting
	void Update () {
		if (!PauseMenu.isPaused) {
			
			if (Time.time - lastCast > 5f) consecutiveCasts = 0;


			if (Input.GetButton ("KeyboardR")) {
				if (!meditating) {
					meditating = true; 
					playerStats.SetManaRecharge (100f);
				}
			} else if (meditating) {
				meditating = false;
				playerStats.SetManaRecharge (0.01f);
			}

			if (!meditating) {

				if (Input.GetButtonDown ("KeyboardR")) {
					meditating = true;
					casting = false;
				} else if (Input.GetMouseButtonDown (0)) {
					casting = true;
				} else if (Input.GetMouseButtonUp (0)) {
					casting = false;
				} else if (Input.GetButtonDown ("Keyboard1")) {
					SwitchSpellSlot (0);
				} else if (Input.GetButtonDown ("Keyboard2")) {
					SwitchSpellSlot (1);
				} else if (Input.GetButtonDown ("Keyboard3")) {
					SwitchSpellSlot (2);
				} else if (Input.GetButtonDown ("Keyboard4")) {
					SwitchSpellSlot (3);
				} else if (Input.GetButtonDown ("Keyboard5")) {
					SwitchSpellSlot (4);
				} else if (Input.GetButtonDown ("KeyboardQ")) {
					PrevSpell ();
				} else if (Input.GetButtonDown ("KeyboardE")) {
					NextSpell ();
				}
				
				if (casting && Time.time >= nextCast && playerStats.GetMana () >= manaCost && currentBase != 8) {

					if (currentForm < 8) {
						CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), true, 1f, 0);
						lastCast = Time.time;
					} else if (currentForm < 16) {
						CastBurst ();
						lastCast = Time.time;
					} else if (currentForm < 24) {
						CastSpread ();
						lastCast = Time.time;
					} else if (currentForm < 32) {
						CastPulse ();
						lastCast = Time.time;
					}

					if (consecutiveStats [0]) { 
						switch (consecutiveStatFactors [0]) {
						case -2:
							playerStats.DrainMana (manaCost * consecutiveLerpInverse [consecutiveCasts] * consecutiveLerpInverse [consecutiveCasts]);
							break;
						case -1:
							playerStats.DrainMana (manaCost * consecutiveLerpInverse [consecutiveCasts]);
							break;
						case 1:
							playerStats.DrainMana (manaCost * consecutiveLerpInverse [10 - consecutiveCasts]);
							break;
						case 2:
							playerStats.DrainMana (manaCost * consecutiveLerpInverse [10 - consecutiveCasts] * consecutiveLerpInverse [10 - consecutiveCasts]);
							break;
						}
					} else
						playerStats.DrainMana (manaCost);

					if (consecutiveStats [1]) {
						switch (consecutiveStatFactors [1]) {
						case -2:
							nextCast = Time.time + cooldown * consecutiveLerpInverse [consecutiveCasts] * consecutiveLerpInverse [consecutiveCasts];
							break;
						case -1:
							nextCast = Time.time + cooldown * consecutiveLerpInverse [consecutiveCasts];
							break;
						case 1:
							nextCast = Time.time + cooldown * consecutiveLerpInverse [10 - consecutiveCasts];
							break;
						case 2:
							nextCast = Time.time + cooldown * consecutiveLerpInverse [10 - consecutiveCasts] * consecutiveLerpInverse [10 - consecutiveCasts];
							break;
						}
					} else
						nextCast = Time.time + cooldown;

				} 
			}
		}
	}

	//Overwrites the current spell (called when a spell is crafted in the menu)
	public void Cast(int spellBase, int spellForm, int[] spellKinetics, int[] spellAugments, int[] spellEffects, float[] spellStats, Sprite spellIcon){

		spellSlots [currentSlot].SetSpell (spellBase, spellForm, spellKinetics, spellAugments, spellEffects, spellStats, spellIcon);

		currentBase = spellBase;
		currentForm = spellForm;
		manaCost = spellStats [0];
		cooldown = spellStats [1];
		baseAccuracy = spellStats [4];

		for (int i = 0; i < 3; i++) {
			currentKinetics[i] = spellKinetics[i];
			currentAugments[i] = spellAugments[i];
			currentEffects[i] = spellEffects[i];
		}

		for (int i = 0; i < 8; i++) {
			currentStats [i] = spellStats [i];
			consecutiveStats [i] = false;
			consecutiveStatFactors [i] = 0;
		}

		if ((currentForm + 1) % 8 == 0) vengefulSpell = true;
		else  vengefulSpell = false;

		consecutiveCasts = 0;
	}

	//Casts a single projectile
	private void CastProjectile(Vector3 castPoint, Vector3 spread, bool consecutiveCast, float scale, int castPointID){
		StartCoroutine (camera.CameraShake (transform.position - castPoints[castPointID].position, currentStats[0]/100f, 0.1f));
		for (int i = 0; i < 8; i++) consecutiveStatFactors [i] = 0;

		currentSpell = projectileSpells [currentBase];
		GameObject spell = Instantiate (currentSpell, castPoint, castPoints[castPointID].rotation, projectileList);
		spell.transform.localScale *= scale;

		ApplySpellEffects (spell);
		spell.transform.Rotate (spread);

		if (consecutiveCast) ConsecutiveCast ();
	}

	//casts a three round burst
	private void CastBurst(){
		StartCoroutine (BurstFire ());
	}

	//controls the burst delay
	private IEnumerator BurstFire(){
		CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), false, 0.8f, 0);
		yield return new WaitForSeconds (0.04f);
		CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), false, 0.8f, 0);
		yield return new WaitForSeconds (0.04f);
		CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), true, 0.8f, 0);
	}

	//casts a sread of projectiles
	private void CastSpread(){
		for (int i = 0; i < 4; i++) CastProjectile (GeneratePosition(1f, 0), GenerateSpread(1f), false, 0.67f, 0);
		CastProjectile (GeneratePosition(1f, 0), GenerateSpread(1f), true, 0.67f, 0);
	} 

	//casts a pulse of projectiles
	private void CastPulse(){
		for (int i = 0; i < 8; i++) {
			if (i == 7) CastProjectile (GeneratePosition (1f, i), GenerateSpread (1f), true, 0.67f, i);
			else CastProjectile (GeneratePosition (1f, i), GenerateSpread (1f), false, 0.67f, i);
		}
	}

	//applies kinetic effects
	private void ApplyKinetics(ProjectileController controller){
		for (int i = 0; i < 3; i++) {
			switch (currentKinetics [i]) {
			case -1: break;
			case 0: controller.LatchingKinetic(); break;
			case 1: controller.BlockingKinetic (); break;
			case 2: controller.EtherealKinetic (); break;
			case 3: controller.GuidedKinetic (); break;
			case 4: controller.PiercingKinetic (); break;
			case 5: controller.ReboundingKinetic (); break;
			case 6: controller.SeekingKinetic (); break;
			case 7: controller.ChainingKinetic (); break;
			}
		}
	}

	//applies spell augments (Refactor This)
	private void ApplyAugments(ProjectileController controller, SpellEffectController effectController, GameObject spell){
		for (int i = 0; i < 3; i++) {
			switch (currentAugments [i]) {
			case -1: break;
			case 16: RepeatingPowerAugment (); break;
			case 17: SporadicPowerAugment (); break;
			case 18: RepeatingSpeedAugment (); break;
			case 19: SporadicSpeedAugment (); break;
			case 20: RepeatingAccuracyAugment (); break;
			case 21: SporadicAccuracyAugment (); break;
			case 22: RepeatingStaggerAugment (); break;
			case 23: SporadicStaggerAugment (); break;
			case 24: RepeatingCritChanceAugment (); break;
			case 25: SporadicCritChanceAugment (); break;
			case 26: RepeatingCritPowerAugment (); break;
			case 27: SporadicCritPowerAugment (); break;
			case 28: RepeatingManaCostAugment (); break;
			case 29: SporadicManaCostAugment (); break;
			case 30: RepeatingCooldownAugment (); break;
			case 31: SporadicCooldownAugment (); break;
			case 32: AttenuatingPowerAugment (effectController); break;
			case 33: DampeningPowerAugment (effectController); break;
			case 34: AttenuatingStaggerAugment (effectController); break;
			case 35: DampeningStaggerAugment (effectController); break;
			case 36: AttenuatingCritChanceAugment (effectController); break;
			case 37: DampeningCritChanceAugment (effectController); break;
			case 38: AttenuatingCritPowerAugment (effectController); break;
			case 39: DampeningCritPowerAugment (effectController); break;
			}
		}
		if (consecutiveStats [2]) ConsecutivePower (effectController);
		if (consecutiveStats [3]) ConsecutiveSpeed (controller);
		if (consecutiveStats [4]) ConsecutiveAccuracy ();
		if (consecutiveStats [5]) ConsecutiveStagger (effectController);
		if (consecutiveStats [6]) ConsecutiveCritChance (effectController);
		if (consecutiveStats [7]) ConsecutiveCritPower (effectController);
	}

	private void RepeatingManaCostAugment(){
		consecutiveStats[0] = true;
		consecutiveStatFactors[0] -= 1;
	}

	private void SporadicManaCostAugment(){
		consecutiveStats[0] = true;
		consecutiveStatFactors[0] += 1;
	}

	private void RepeatingCooldownAugment(){
		consecutiveStats[1] = true;
		consecutiveStatFactors[1] -= 1;
	}

	private void SporadicCooldownAugment(){
		consecutiveStats[1] = true;
		consecutiveStatFactors[1] += 1;
	}

	private void RepeatingPowerAugment(){
		consecutiveStats [2] = true;
		consecutiveStatFactors [2] += 1;
	}

	private void SporadicPowerAugment(){
		consecutiveStats [2] = true;
		consecutiveStatFactors [2] -= 1;
	}
		
	private void ConsecutivePower(SpellEffectController spell){
		switch (consecutiveStatFactors [2]) {
		case -2: spell.ModifyDamage(consecutiveLerpSmall[10 - consecutiveCasts] * consecutiveLerpSmall[10 - consecutiveCasts]); break;
		case -1: spell.ModifyDamage(consecutiveLerpSmall[10 - consecutiveCasts]); break;
		case 1: spell.ModifyDamage(consecutiveLerpSmall[consecutiveCasts]); break;
		case 2: spell.ModifyDamage(consecutiveLerpSmall[consecutiveCasts] * consecutiveLerpSmall[consecutiveCasts]); break;
		}
	}
		
	private void RepeatingSpeedAugment(){
		consecutiveStats [3] = true;
		consecutiveStatFactors [3] += 1;
	}

	private void SporadicSpeedAugment(){
		consecutiveStats [3] = true;
		consecutiveStatFactors [3] -= 1;
	}

	private void ConsecutiveSpeed(ProjectileController controller){
		switch (consecutiveStatFactors [3]) {
		case -2: controller.ModifySpeed(consecutiveLerpMedium[10 - consecutiveCasts] * consecutiveLerpSmall[10 - consecutiveCasts]); break;
		case -1: controller.ModifySpeed(consecutiveLerpMedium[10 - consecutiveCasts]); break;
		case 1: controller.ModifySpeed(consecutiveLerpMedium[consecutiveCasts]); break;
		case 2: controller.ModifySpeed(consecutiveLerpMedium[consecutiveCasts] * consecutiveLerpSmall[consecutiveCasts]); break;
		}
	}

	private void RepeatingAccuracyAugment(){
		consecutiveStats[4] = true;
		consecutiveStatFactors[4] += 1;
	}

	private void SporadicAccuracyAugment(){
		consecutiveStats[4] = true;
		consecutiveStatFactors[4] -= 1;
	}

	private void ConsecutiveAccuracy(){
		realAccuracy = baseAccuracy;

		switch (consecutiveStatFactors [4]) {
		case -2: realAccuracy -= 2 * consecutiveLerpLarge [10 - consecutiveCasts]; break;
		case -1: realAccuracy -= consecutiveLerpLarge [10 - consecutiveCasts]; break;
		case 1: realAccuracy -= consecutiveLerpLarge [consecutiveCasts]; break;
		case 2: realAccuracy -= 2 * consecutiveLerpLarge [consecutiveCasts]; break;
		}
	}

	private void RepeatingStaggerAugment(){
		consecutiveStats [5] = true;
		consecutiveStatFactors [5] += 1;
	}

	private void SporadicStaggerAugment(){
		consecutiveStats [5] = true;
		consecutiveStatFactors [5] -= 1;
	}

	private void ConsecutiveStagger(SpellEffectController spell){
		switch (consecutiveStatFactors [5]) {
		case -2: spell.ModifyStagger(consecutiveLerpLarge[10 - consecutiveCasts] * consecutiveLerpSmall[10 - consecutiveCasts]); break;
		case -1: spell.ModifyStagger(consecutiveLerpLarge[10 - consecutiveCasts]); break;
		case 1: spell.ModifyStagger(consecutiveLerpLarge[consecutiveCasts]); break;
		case 2: spell.ModifyStagger(consecutiveLerpLarge[consecutiveCasts] * consecutiveLerpSmall[consecutiveCasts]); break;
		}
	}

	private void RepeatingCritChanceAugment(){
		consecutiveStats [6] = true;
		consecutiveStatFactors [6] += 1;
	}

	private void SporadicCritChanceAugment(){
		consecutiveStats [6] = true;
		consecutiveStatFactors [6] -= 1;
	}

	private void ConsecutiveCritChance(SpellEffectController spell){
		switch (consecutiveStatFactors [6]) {
		case -2: spell.ModifyCritChance(consecutiveLerpLarge[10 - consecutiveCasts] * consecutiveLerpSmall[10 - consecutiveCasts]); break;
		case -1: spell.ModifyCritChance(consecutiveLerpLarge[10 - consecutiveCasts]); break;
		case 1: spell.ModifyCritChance(consecutiveLerpLarge[consecutiveCasts]); break;
		case 2: spell.ModifyCritChance(consecutiveLerpLarge[consecutiveCasts] * consecutiveLerpSmall[consecutiveCasts]); break;
		}
	}

	private void RepeatingCritPowerAugment(){
		consecutiveStats [7] = true;
		consecutiveStatFactors [7] += 1;
	}

	private void SporadicCritPowerAugment(){
		consecutiveStats [7] = true;
		consecutiveStatFactors [7] -= 1;
	}

	private void ConsecutiveCritPower(SpellEffectController spell){
		switch (consecutiveStatFactors [7]) {
		case -2: spell.ModifyCritPower(consecutiveLerpLarge[10 - consecutiveCasts] * consecutiveLerpSmall[10 - consecutiveCasts]); break;
		case -1: spell.ModifyCritPower(consecutiveLerpLarge[10 - consecutiveCasts]); break;
		case 1: spell.ModifyCritPower(consecutiveLerpLarge[consecutiveCasts]); break;
		case 2: spell.ModifyCritPower(consecutiveLerpLarge[consecutiveCasts] * consecutiveLerpSmall[consecutiveCasts]); break;
		}
	}

	private void AttenuatingPowerAugment(SpellEffectController spell){
		spell.SetAttenuatingDamage (1);
	}

	private void DampeningPowerAugment(SpellEffectController spell){
		spell.SetAttenuatingDamage (-1);
	}

	private void AttenuatingStaggerAugment(SpellEffectController spell){
		spell.SetAttenuatingStagger (1);	
	}

	private void DampeningStaggerAugment(SpellEffectController spell){
		spell.SetAttenuatingStagger (-1);	
	}

	private void AttenuatingCritChanceAugment(SpellEffectController spell){
		spell.SetAttenuatingCritChance (1);
	}

	private void DampeningCritChanceAugment(SpellEffectController spell){
		spell.SetAttenuatingCritChance (-1);
	}

	private void AttenuatingCritPowerAugment(SpellEffectController spell){
		spell.SetAttenuatingCritPower (1);
	}

	private void DampeningCritPowerAugment(SpellEffectController spell){
		spell.SetAttenuatingCritPower (-1);
	}

	//applies spell effects
	private void ApplyEffects(ProjectileController controller, SpellEffectController effects){
		for (int i = 0; i < 3; i++) {
			switch (currentEffects [i]) {
			case -1: break;
			case 0: effects.HypnotisingEffect (); break;
			case 1: effects.VorpalEffect (); break;
			case 2: effects.MarkingEffect (); break;
			case 3: effects.SnaringEffect (); break;
			case 4: effects.AlchemisingEffect (); break;
			case 5: effects.ExecutingEffect (); break;
			case 6: effects.ExplosiveEffect (); break;
			case 7: effects.CrystalisingEffect (); break;
			case 8: effects.StunningEffect (); break;
			case 9: effects.DisintegratingEffect (); break;
			case 10: effects.SlowingEffect (); break;
			case 11: effects.EnfeeblingEffect (); break;
			case 12: effects.LastingEffect (); break;
			case 13: effects.WeakeningEffect (); break;
			case 14: effects.RegicideEffect (); break;
			case 15: effects.ManaDrainEffect (); break;
			}
		}
	}

	private void ConsecutiveCast(){
		if (consecutiveCasts < 10) {
			consecutiveCasts++;
		}
	}

	//switches to the previous spell slot
	private void PrevSpell(){
		currentSlot--;
		if (currentSlot < 0) currentSlot = 4;
		SwitchSpellSlot (currentSlot);
	}

	//switches to the nect spell slot
	private void NextSpell(){
		currentSlot++;
		if (currentSlot > 4) currentSlot = 0;
		SwitchSpellSlot (currentSlot);
	}

	//switches spell slots to a specific index
	private void SwitchSpellSlot(int slot){
		consecutiveCasts = 0;
		for (int i = 0; i < 8; i++) {
			consecutiveStats [i] = false;
			consecutiveStatFactors [i] = 0;
		}

		currentSlot = slot;
		SpellSlotController nextSlot = spellSlots [slot];
		currentBase = nextSlot.GetSpellBase ();
		currentForm = nextSlot.GetSpellForm ();
		currentKinetics = nextSlot.GetSpellKinetics ();
		currentAugments = nextSlot.GetSpellAugments (); 
		currentEffects = nextSlot.GetSpellEffects ();
		for (int i = 0; i < 8; i++) {
			currentStats[i] = nextSlot.GetSpellStats (i);
		}

		manaCost = currentStats [0];
		cooldown = currentStats [1];
		baseAccuracy = currentStats [4];

		if ((currentForm + 1) % 8 == 0) vengefulSpell = true;
		else  vengefulSpell = false;

		SetSpellIcons (slot);
	}
		
	//sets the spell icons in the HUD
	private void SetSpellIcons(int slot){
		int nextSlot = slot + 1;
		if (nextSlot > 4) nextSlot = 0;
		int prevSlot = slot - 1;
		if (prevSlot < 0) prevSlot = 4;

		spellSlots [slot].ChangeSprite ();
		spellSlots [prevSlot].ChangePreviousSprite ();
		spellSlots [nextSlot].ChangeNextSprite ();

	}

	//sets the stats of the spell
	private void SetStats(ProjectileController pController, SpellEffectController sController){
		if (vengefulSpell) vengefulModifier = 1f + 0.1f * MissingHealthModifier ();
		else vengefulModifier = 1f;

		pController.SetSpeed (currentStats [3] * vengefulModifier);
		sController.SetPower (currentStats [2] * vengefulModifier);
		sController.SetStagger (currentStats [5] * vengefulModifier);
		sController.SetCritChance (currentStats [6] * vengefulModifier);
		sController.SetCritPower (currentStats [7] * vengefulModifier);
	}

	//generates a random offset for the spell
	private Vector3 GeneratePosition(float innacuracyMultiplier, int castPoint){
		Vector3 normal = castPoints[castPoint].position;
		Vector3 offsetForward = (castPoints [castPoint].position - transform.position) * Random.Range (0, 0.1f) * innacuracyMultiplier;
		Vector3 offsetSideways = Quaternion.Euler(0, 90, 0) * (castPoints [castPoint].position - transform.position) * Random.Range (-0.1f, 0.1f) * innacuracyMultiplier;
		return normal + offsetForward + offsetSideways;
	}

	//generates a random offset for the spell, with different forward and sideways offsets
	private Vector3 GeneratePosition(float innacuracyMultiplierForwards, float innacuracyMultiplierSideways, int castPoint){
		Vector3 normal = castPoints[castPoint].position;
		Vector3 offsetForward = (castPoints [castPoint].position - transform.position) * Random.Range (0, 0.1f) * innacuracyMultiplierForwards;
		Vector3 offsetSideways = Quaternion.Euler(0, 90, 0) * (castPoints [castPoint].position - transform.position) * Random.Range (-0.1f, 0.1f) * innacuracyMultiplierSideways;
		return normal + offsetForward + offsetSideways;
	}

	//generates the innacuracy spread of the projectile
	private Vector3 GenerateSpread(float innacuracyMultiplier){
		Vector3 rotation = new Vector3(0f, Random.Range(-realAccuracy, realAccuracy) * innacuracyMultiplier, 0f);
		return rotation;
	}

	//applies the effects of the spell
	private void ApplySpellEffects(GameObject spell){
		realAccuracy = baseAccuracy;
		ProjectileController projectileController = spell.GetComponent<ProjectileController> ();
		SpellEffectController effectController = spell.GetComponent<SpellEffectController> ();
		SetStats (projectileController, effectController);
		ApplyKinetics (projectileController);
		ApplyAugments (projectileController, effectController, spell);
		ApplyEffects (projectileController, effectController);
	}

	//TODO CHANGE THIS SO ITS IN THE STAT CONTROLLER
	public bool GetMeditating(){
		return meditating;
	}

	//used for Vengeful spells
	private int MissingHealthModifier(){
		float HP = playerStats.GetHealthPercent ();
		int modifier;

		if (HP <= 0.2f) modifier = 5;
		else if (HP <= 0.4f) modifier = 4;
		else if (HP <= 0.6f) modifier = 3;
		else if (HP <= 0.8f) modifier = 2;
		else modifier = 1;

		return modifier;
	}
}
