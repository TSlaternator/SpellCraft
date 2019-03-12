using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectController : MonoBehaviour {

	[SerializeField] private float powerVariability; //how much a spells damage can change
	[SerializeField] private int damageType; //the damage type the spell deals
	private float power; //the power of the spell
	private float impact; //the impact of the spell
	private float critChance; //crit chance of the spell
	private float critMultiplier = 1f; //crit multiplier of the spell
	private bool vorpal; //if the spell is vorpal or not
	private float vorpalChance; //the chance to deal a vorpal crit
	private float vorpalDamage = 100f; //damage dealt by a vorpal crit
	private bool executing; //if the spell is executing or not
	private float executeThreshold = 0.1f; //the percentage of enemy health the spell will 1HKO under
	[SerializeField] private GameObject explosion; //the explosion effect of the spell (if its explosive)
	private bool explosive; //if the spell is explosive
	private float explosionDamage = 10f; //how much damage the explosion deals
	private float disintegrateModifier = 1f; //increases damage if the disinitegrating effect is on the spell
	private bool regicide; //will increae damage against bosses: TODO
	private float RegicideMultiplier = 1f; //how much of an increase regicide has
	private bool isCrit; //if the spell crits upon hitting an enemy (changes damage number colours)

    //gets the damage done by the spell (called by enemies when they are hit)
	public float GetDamage(EnemyStatController enemy){

		if (executing && enemy.GetHealthPercent() < executeThreshold) {
			isCrit = false; //damage number colour should be normal
			damageType = 6; //setting damage type to pure so enemy won't survive due to resistance
			return enemy.GetHealth (); //returns current health of the enemy
		} else {

			if (Random.Range (0f, 1f) < critChance) {
				isCrit = true;
				if (vorpal && Random.Range (0f, 1f) < vorpalChance) return (power + vorpalDamage) * (1 + critMultiplier) * (1 + Random.Range (-powerVariability, powerVariability)) * disintegrateModifier;
				else return power * (1 + critMultiplier) * (1 + Random.Range (-powerVariability, powerVariability)) * disintegrateModifier;
			} else {
				isCrit = false;
				if (vorpal && Random.Range (0f, 1f) < vorpalChance) return (power + vorpalDamage) * (1 + Random.Range (-powerVariability, powerVariability)) * disintegrateModifier;
				else return power * (1 + Random.Range (-powerVariability, powerVariability)) * disintegrateModifier;
			}
		}
	}

    //gets the damage done by the spell to bosses(called by bosses when they are hit)
    public float GetDamage(BossStatController enemy) {
        if (Random.Range(0f, 1f) < critChance) {
            isCrit = true;
            if (vorpal && Random.Range(0f, 1f) < vorpalChance) return (power + vorpalDamage) * (1 + critMultiplier) * (1 + Random.Range(-powerVariability, powerVariability))* RegicideMultiplier;
            else return power * (1 + critMultiplier) * (1 + Random.Range(-powerVariability, powerVariability))* RegicideMultiplier;
        } else {
            isCrit = false;
            if (vorpal && Random.Range(0f, 1f) < vorpalChance) return (power + vorpalDamage) * (1 + Random.Range(-powerVariability, powerVariability))* RegicideMultiplier;
            else return power * (1 + Random.Range(-powerVariability, powerVariability))* RegicideMultiplier;
        }
    }

    //sets the stats of the spell (called when it's instantiated
    public void setStats(float power, float impact, float critChance, float critMultiplier) {
        this.power = power;
        this.impact = impact;
        this.critChance = critChance;
        this.critMultiplier = critMultiplier;
    }

    //returns the impact of the spell
	public float GetImpact(){
		return impact;
	}

    //returns the spells damage type
	public int GetDamageType(){
		return damageType;
	}

    //returns if the spell crit or not
	public bool GetCrit(){
		return isCrit;
	}

    //gets the power of the spell
    public float GetPower() {
        return power;
    }

    //Modifies the damage of the spell (called when the spell pierces/chains enemies)
	public void ModifyDamage(float damageMultiplier){
		power *= damageMultiplier;
	}

    //Applies the spells effects when hitting a target
	public void ApplyEffects(ObstructionController enemy){
        IEffectRune[] effects = GetComponents<IEffectRune>();
        for(int i = 0; i < effects.Length; i++) {
            effects[i].ApplyEffect(enemy.gameObject, this);
        }
	}

    //Applies the spells effects when hitting a target
    public void ApplyEffects(BossStatController boss) {
        IEffectRune[] effects = GetComponents<IEffectRune>();
        for (int i = 0; i < effects.Length; i++) {
            effects[i].ApplyEffect(boss, this);
        }
    }

    //Applies the spells effects when hitting a target
    public void ApplyEffects(EyerisMinionController boss) {
        IEffectRune[] effects = GetComponents<IEffectRune>();
        for (int i = 0; i < effects.Length; i++) {
            effects[i].ApplyEffect(boss, this);
        }
    }

    //Destroys the spell, applying any death effects (such as exploding)
    public void ApplyDeathEffects(){
		if (explosive) {
			GameObject explosionController = Instantiate (explosion, transform.position, transform.rotation);
			explosionController.GetComponent<ExplosionController> ().SetDamage (explosionDamage);
		}
		Destroy (gameObject);
	}

    //Adds or Increases an effect of the spell
    public void AddEffect(int effect) {
        IEffectRune currentEffect = null;
        switch (effect) {
            case 0: currentEffect = GetComponent<HypnosisEffect>(); break;
            case 1: currentEffect = GetComponent<MarkingEffect>(); break;
            case 2: currentEffect = GetComponent<SnareEffect>(); break;
            case 3: currentEffect = GetComponent<AlchemisingEffect>(); break;
            case 4: currentEffect = GetComponent<CrystalisingEffect>(); break;
            case 5: currentEffect = GetComponent<StunningEffect>(); break;
            case 6: currentEffect = GetComponent<DisintegratingEffect>(); break;
            case 7: currentEffect = GetComponent<SlowingEffect>(); break;
            case 8: currentEffect = GetComponent<EnfeeblingEffect>(); break;
            case 9: currentEffect = GetComponent<LastingEffect>(); break;
            case 10: currentEffect = GetComponent<WeakeningEffect>(); break;
            case 11: currentEffect = GetComponent<ManaDrainEffect>(); break;
        }
        if (currentEffect != null) currentEffect.IncreasePotency();
        else {
            switch (effect) {
                case 0: currentEffect = gameObject.AddComponent<HypnosisEffect>(); break;
                case 1: currentEffect = gameObject.AddComponent<MarkingEffect>(); break;
                case 2: currentEffect = gameObject.AddComponent<SnareEffect>(); break;
                case 3: currentEffect = gameObject.AddComponent<AlchemisingEffect>(); break;
                case 4: currentEffect = gameObject.AddComponent<CrystalisingEffect>(); break;
                case 5: currentEffect = gameObject.AddComponent<StunningEffect>(); break;
                case 6: currentEffect = gameObject.AddComponent<DisintegratingEffect>(); break;
                case 7: currentEffect = gameObject.AddComponent<SlowingEffect>(); break;
                case 8: currentEffect = gameObject.AddComponent<EnfeeblingEffect>(); break;
                case 9: currentEffect = gameObject.AddComponent<LastingEffect>(); break;
                case 10: currentEffect = gameObject.AddComponent<WeakeningEffect>(); break;
                case 11: currentEffect = gameObject.AddComponent<ManaDrainEffect>(); break;
            }
        }
    }

    //Adds the vorpal effect to the spell
	public void VorpalEffect(){
		vorpal = true;
		vorpalChance += 0.025f;
	}

    //Adds the executing effect to the spell
	public void ExecutingEffect(){
		executing = true;
		executeThreshold += 0.1f;
	}

    //Adds the explosive effect to the spell
	public void ExplosiveEffect(){
		explosive = true;
		explosionDamage *= 1.5f;
	}

    //Adds the disintegratingEffect to the spell
	public void DisintegratingEffect(){
        AddEffect(6);
		disintegrateModifier += 0.2f;
	}

    //Adds the regicide effect to the spell (TODO: Implement this)
	public void RegicideEffect(){
		regicide = true;
		RegicideMultiplier += 0.1f;
		//TODO actually implement this when bosses are made
	}
}
