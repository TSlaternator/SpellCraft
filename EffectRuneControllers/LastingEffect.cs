using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastingEffect : MonoBehaviour, IEffectRune {
    float DoTChance = 0.08f; //the chance of the effect applying
    float DoTDamage = 0.2f;

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {

        if (Random.Range(0f, 1f) < DoTChance) {
            enemy.GetComponent<EnemyStatController>().ApplyDoT(controller.GetDamageType(), controller.GetPower() * DoTDamage);
        }
    }

    //Has a chance to apply the Rune's effect on the boss
    public void ApplyEffect(BossStatController boss, SpellEffectController controller) {
        if (Random.Range(0f, 1f) < DoTChance) {
            boss.ApplyDoT(controller.GetDamageType(), controller.GetPower() * DoTDamage);
        }
    }

    //Has a chance to apply the Runes effect on the boss' minion
    public void ApplyEffect(EyerisMinionController minion, SpellEffectController controller) {
        if (Random.Range(0f, 1f) < DoTChance) {
            minion.ApplyDoT(controller.GetDamageType(), controller.GetPower() * DoTDamage);
        }
    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        DoTChance += 0.08f;
        DoTDamage += 0.1f;
    }
}
