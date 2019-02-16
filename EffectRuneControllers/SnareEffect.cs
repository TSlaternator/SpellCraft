using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnareEffect : MonoBehaviour, IEffectRune {

    float snareChance = 0.05f; //the chance of the effect applying

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        if (Random.Range(0f, 1f) < snareChance) enemy.GetComponent<EnemyController>().ApplySnare();
    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        snareChance += 0.05f;
    }
}
