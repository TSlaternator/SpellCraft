﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakeningEffect : MonoBehaviour, IEffectRune {
    int weaknessModifier = 15; //the chance of the effect applying

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        enemy.GetComponent<EnemyStatController>().Weaken(controller.GetDamageType(), weaknessModifier);
    }

    //This effect does nothing to bosses
    public void ApplyEffect(BossStatController boss, SpellEffectController controller) {

    }

    //This effect does nothing to bosses minions
    public void ApplyEffect(EyerisMinionController minion, SpellEffectController controller) {

    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        weaknessModifier += 5;
    }
}
