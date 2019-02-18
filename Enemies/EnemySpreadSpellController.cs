using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpreadSpellController : MonoBehaviour, IEnemySpellController
{
    /* Implementation of the IEnemySpellController interface to control enemy spells
      * that fire a single projectile */

    [SerializeField] private float damage; //damage of the projectile
    [SerializeField] private float speed; //speed of the projectile
    [SerializeField] private float accuracy; //accuracy of the projectile
    [SerializeField] private GameObject spellObject; //the projectiles gameobject
    [SerializeField] private Transform[] castPoints; //cast point of the projectile

    private bool enfeebled; //if the enemy is enfeebled
    private float enfeebleModifier; //the magnitude of the effect
    private float enfeebleDuration; //how long enfeebling lasts

    //keeps track of statuses
    void Update() {
        if (enfeebled && Time.time > enfeebleDuration) {
            enfeebled = false;
            damage *= 1.5f;
        }
    }

    //shoots the projectile
    public void Shoot() {
        for (int i = 0; i < castPoints.Length; i++) {
            GameObject spell = Instantiate(spellObject, castPoints[i].position, castPoints[i].rotation);
            EnemyProjectileController spellEffect = spell.GetComponent<EnemyProjectileController>();
            spell.transform.Rotate(CalculateSpread());
            spellEffect.SetSpeed(speed);
            spellEffect.SetDamage(damage);
        }
    }

    //calculates the spread of the projectile (based on its accuracy)
    private Vector3 CalculateSpread() {
        Vector3 rotation = new Vector3(0f, Random.Range(-accuracy, accuracy), 0f);
        return rotation;
    }

    //reduces the enemies damage for a short duration
    public void Enfeeble() {
        if (!enfeebled) {
            enfeebled = true;
            damage /= 1.5f;
        }

        enfeebleDuration = Time.time + 5f;
    }
}
