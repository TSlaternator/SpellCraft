using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkingEffect : MonoBehaviour, IEffectRune {
    float markMultiplier = 1.25f; //how much to multiply the damage a marked target takes by

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        enemy.GetComponent<EnemyStatController>().Mark(controller.GetDamageType(), markMultiplier);
    }

    //Has a chance to apply the Rune's effect on the boss
    public void ApplyEffect(BossStatController boss, SpellEffectController controller) {
        boss.Mark(controller.GetDamageType(), markMultiplier);
    }

    //Has a chance to apply the Rune's effect on the boss
    public void ApplyEffect(EyerisMinionController minion, SpellEffectController controller) {
        minion.Mark(controller.GetDamageType(), markMultiplier);
    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        markMultiplier += 0.25f;
    }
}
