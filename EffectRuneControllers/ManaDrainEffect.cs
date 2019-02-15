using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDrainEffect : MonoBehaviour, IEffectRune {
    float manaDrainChance = 0.5f; //the chance of the effect applying

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        if (Random.Range(0f, 1f) >= manaDrainChance) enemy.GetComponent<EnemyStatController>().ManaDrain();
    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        manaDrainChance += 0.25f;
    }
}
