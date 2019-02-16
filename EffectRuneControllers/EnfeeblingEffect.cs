using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnfeeblingEffect : MonoBehaviour, IEffectRune {
    float enfeebleChance = 0.05f; //the chance of the effect applying

    //Has a chance to apply the Rune's effect on the enemy 
    public void ApplyEffect(GameObject enemy, SpellEffectController controller) {
        if (Random.Range(0f, 1f) < enfeebleChance) enemy.GetComponent<IEnemySpellController>().Enfeeble();
    }

    //Increases the potency of the rune (by increasing its effect, chance to proc, or both)
    public void IncreasePotency() {
        enfeebleChance += 0.05f;
    }
}
