﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpiralAttack : MonoBehaviour, IBossAttack
{

    [SerializeField] private float castDuration; //duration of the attack
    [SerializeField] private float damage; //damage of the projectile
    [SerializeField] private float speed; //speed of the projectile
    [SerializeField] private float accuracy; //accuracy of the projectile
    [SerializeField] private GameObject spellObject; //the projectiles gameobject
    [SerializeField] private Transform[] castPoints; //cast points of the projectile
    [SerializeField] private float fireRate; //fire rate of the spell
    [SerializeField] private string animatorBool; //the animation condition in the animator
    [SerializeField] private Animator animator; //animator attached to the AI
    private float attackEndTime; //when the attack will end
    private float nextFire; //when the attack will fire the next projectile(s)

    //controls the attack
    void Update() {
        if (Time.time < attackEndTime) {
            if (Time.time > nextFire) {
                for (int i = 0; i < castPoints.Length; i++) {
                    CastProjectile(i); 
                }
                transform.RotateAround(transform.position, transform.up, 5f);
                nextFire += fireRate;
            }
        }
    }

    //controls the animation
    private IEnumerator Animate() {
        animator.SetBool(animatorBool, true);
        yield return new WaitForSeconds(castDuration);
        animator.SetBool(animatorBool, false);
    }

    //Casts the attack
    public void OnCast() {
        attackEndTime = Time.time + castDuration;
        nextFire = Time.time;
        StartCoroutine(Animate());
    }

    //Gets the duration of the attack
    public float getCastDuration() {
        return castDuration;
    }

    //shoots a projectile
    public void CastProjectile(int castPoint) {
        GameObject spell = Instantiate(spellObject, castPoints[castPoint].position, castPoints[castPoint].rotation);
        EnemyProjectileController spellEffect = spell.GetComponent<EnemyProjectileController>();
        spell.transform.Rotate(CalculateSpread());
        spellEffect.SetSpeed(speed);
        spellEffect.SetDamage(damage);
    }

    //calculates the spread of the projectile (based on its accuracy)
    private Vector3 CalculateSpread() {
        Vector3 rotation = new Vector3(0f, Random.Range(-accuracy, accuracy), 0f);
        return rotation;
    }
}
