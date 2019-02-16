using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypnosisEffect : MonoBehaviour, IEffectRune {
    float sleepChance = 0.025f; //the chance of the effect applying

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        if (Random.Range(0f, 1f) < sleepChance) enemy.GetComponent<EnemyController>().ApplySleep();
    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        sleepChance += 0.025f;
    }
}
