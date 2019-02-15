using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakeningEffect : MonoBehaviour, IEffectRune {
    int weaknessModifier = 15; //the chance of the effect applying

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        if (Random.Range(0f, 1f) > weaknessModifier) enemy.GetComponent<EnemyStatController>().Weaken(controller.GetDamageType(), weaknessModifier);
    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        weaknessModifier += 5;
    }
}
