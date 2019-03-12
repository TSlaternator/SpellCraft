using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisintegratingEffect : MonoBehaviour, IEffectRune {
    //Applies the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        enemy.GetComponent<EnemyStatController>().Disintegrate();
    }

    //This effect does nothing to bosses
    public void ApplyEffect(BossStatController boss, SpellEffectController controller) {

    }

    //This effect does nothing to bosses minions
    public void ApplyEffect(EyerisMinionController minion, SpellEffectController controller) {

    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {}
}
