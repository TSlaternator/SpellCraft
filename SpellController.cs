using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellController : MonoBehaviour {

	/* Controls the spells the player is casting */

	[SerializeField] private CameraShaker cam; //the cameraShaker object
	[SerializeField] private PlayerStatController playerStats; //reference to the players stats
	[SerializeField] private SpellSlotController[] spellSlots; //reference to the players spell slots
	[SerializeField] private int currentSlot; //index of the current spell slot

	[SerializeField] private Transform[] castPoints; //cast points of the players spells
	[SerializeField] private GameObject[] projectileSpells; //array of the different spell bases
	[SerializeField] private Transform projectileList; //list to tidy up the interface

    [SerializeField] private Spell currentSpell; //current Spell the player will cast
	private GameObject currentSpellObject; //the current spell being cast

	private float nextCast; //next time a spell can be cast
	private bool casting; //if the player is holding down the cast button
	private bool meditating; //if the player is meditating or not ******************* MAYBE MOVE THIS TO STAT CONTROLLER ***********************
	private bool vengefulSpell; //if a spell is vengeful or not
	private float vengefulModifier = 1f; //the modifier of vengefuls spells (NEED TO RE DO THIS)

	//initialising variables
	void Start(){
		nextCast = Time.time;
		currentSlot = 0;
	}

	//controls the players casting
	void Update () {
		if (!PauseMenu.isPaused) {

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

                //Getting user input
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
				
                //Casts a spell if the player is holding down the cast button, has enough mana, and their spell isnt on cooldown
				if (casting && Time.time >= nextCast && playerStats.GetMana () >= currentSpell.manaCost && currentSpell.spellBase != 8) {

                    //casts a spell of the correct spellForm
					if (currentSpell.spellForm < 8) CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), 1f, 0);
					else if (currentSpell.spellForm < 16) CastBurst ();
					else if (currentSpell.spellForm < 24) CastSpread ();
					else if (currentSpell.spellForm < 32) CastPulse ();

                    //Drain mana and reset cooldown
                    playerStats.DrainMana (currentSpell.manaCost);
					nextCast = Time.time + currentSpell.cooldown;
				} 
			}
		}
	}

	//Overwrites the current spell (called when a spell is crafted in the menu)
	public void Cast(Spell spell){
        currentSpell = spell;
		spellSlots [currentSlot].setSpell (currentSpell);
        SetSpellIcons(currentSlot);

        //Need to make a distinct copy rather than just reference the arrays
        currentSpell.spellEffects = (int[])spell.spellEffects.Clone();
        currentSpell.spellAugments = (int[])spell.spellAugments.Clone();
        currentSpell.spellKinetics = (int[])spell.spellKinetics.Clone();

        if ((currentSpell.spellForm) % 8 == 7) vengefulSpell = true;
		else vengefulSpell = false;
	}

	//Casts a single projectile
	private void CastProjectile(Vector3 castPoint, Vector3 spread, float scale, int castPointID){
		StartCoroutine (cam.CameraShake (transform.position - castPoints[castPointID].position, currentSpell.manaCost/100f, 0.1f));

        currentSpellObject = projectileSpells [currentSpell.spellBase];
		GameObject spell = Instantiate (currentSpellObject, castPoint, castPoints[castPointID].rotation, projectileList);
		spell.transform.localScale *= scale;

		ApplySpellEffects (spell);
		spell.transform.Rotate (spread);
    }

	//casts a three round burst
	private void CastBurst(){
		StartCoroutine (BurstFire ());
	}

	//controls the burst delay
	private IEnumerator BurstFire(){
		CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), 0.8f, 0);
		yield return new WaitForSeconds (0.04f);
		CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), 0.8f, 0);
		yield return new WaitForSeconds (0.04f);
		CastProjectile (GeneratePosition (1f, 0), GenerateSpread (1f), 0.8f, 0);
	}

	//casts a spread of projectiles
	private void CastSpread(){
		for (int i = 0; i < 5; i++) CastProjectile (GeneratePosition(1f, 0), GenerateSpread(1f), 0.67f, 0);
	} 

	//casts a pulse of projectiles
	private void CastPulse(){
		for (int i = 0; i < 8; i++) {
			CastProjectile (GeneratePosition (1f, i), GenerateSpread (1f), 0.67f, i);
		}
	}

	//applies kinetic effects
	private void ApplyKinetics(ProjectileController controller){
		for (int i = 0; i < 3; i++) {
			switch (currentSpell.spellKinetics [i]) {
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

	//applies spell effects
	private void ApplyEffects(SpellEffectController effects){
		for (int i = 0; i < 3; i++) {
			switch (currentSpell.spellEffects [i]) {
			case -1: break;
            case 0: effects.AddEffect(0); break;
			case 1: effects.VorpalEffect (); break;
			case 2: effects.AddEffect(1); break;
			case 3: effects.AddEffect(2); break;
			case 4: effects.AddEffect(3); break;
			case 5: effects.ExecutingEffect (); break;
			case 6: effects.ExplosiveEffect (); break;
			case 7: effects.AddEffect(4); break;
			case 8: effects.AddEffect(5); break;
			case 9: effects.DisintegratingEffect (); break;
			case 10: effects.AddEffect(7); break;
			case 11: effects.AddEffect(8); break;
			case 12: effects.AddEffect(9); break;
			case 13: effects.AddEffect(10); break;
			case 14: effects.RegicideEffect (); break;
			case 15: effects.AddEffect(11); break;
			}
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
		currentSlot = slot;
		SpellSlotController nextSlot = spellSlots [slot];

        Spell spell = nextSlot.getSpell();
        currentSpell = spell;
        currentSpell.spellEffects = (int[])spell.spellEffects.Clone();
        currentSpell.spellAugments = (int[])spell.spellAugments.Clone();
        currentSpell.spellKinetics = (int[])spell.spellKinetics.Clone();

        if ((currentSpell.spellForm) % 8 == 7) vengefulSpell = true;
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

		pController.SetSpeed (currentSpell.speed * vengefulModifier);
        sController.setStats(currentSpell.power * vengefulModifier, currentSpell.impact * vengefulModifier, currentSpell.critChance * vengefulModifier, 
                                currentSpell.critMultiplier * vengefulModifier);
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
		Vector3 rotation = new Vector3(0f, Random.Range(-(100 - currentSpell.accuracy), (100 - currentSpell.accuracy)) * innacuracyMultiplier, 0f);
		return rotation;
	}

	//applies the effects of the spell
	private void ApplySpellEffects(GameObject spell){
		ProjectileController projectileController = spell.GetComponent<ProjectileController> ();
		SpellEffectController effectController = spell.GetComponent<SpellEffectController> ();
		SetStats (projectileController, effectController);
		ApplyKinetics (projectileController);
		ApplyEffects (effectController);
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
