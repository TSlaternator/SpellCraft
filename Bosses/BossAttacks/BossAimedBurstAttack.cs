﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAimedBurstAttack : MonoBehaviour, IBossAttack {

    [SerializeField] private float castDuration; //duration of the attack
    [SerializeField] private float damage; //damage of the projectile
    [SerializeField] private float speed; //speed of the projectile
    [SerializeField] private float accuracy; //accuracy of the projectile
    [SerializeField] private GameObject spellObject; //the projectiles gameobject
    [SerializeField] private Transform castPoint; //cast point of the projectile
    [SerializeField] private float fireRate; //fire rate of the spell
    [SerializeField] private string animatorBool; //the animation condition in the animator
    [SerializeField] private Animator animator; //animator attached to the AI
    [SerializeField] private AnimationClip attackAnimation; //the clip of the animation
    private float attackEndTime = 0; //when the attack will end
    private float nextFire = 0; //when the attack will fire the next projectile(s)
    private PlayerMoveController player; //the players transform

    //gets the players transform
    void Start() {
        player = GameObject.Find("Player").GetComponent<PlayerMoveController>();
    }

    //controls the attack
    void Update() {
        if (Time.time < attackEndTime) {
            transform.LookAt(player.getFuturePosition());
            if (Time.time > nextFire) {
                StartCoroutine(Cast());
                nextFire += fireRate;
            }
        }
    }

    //Casts the attack
    public void OnCast() {
        attackEndTime = Time.time + castDuration;
        nextFire = Time.time;
    }

    //Gets the duration of the attack
    public float getCastDuration() {
        return castDuration;
    }

    //casts a projectile, with animation
    private IEnumerator Cast() {
        animator.SetBool(animatorBool, true);
        yield return new WaitForSeconds(attackAnimation.length * 0.7f);
        CastProjectile();
        yield return new WaitForSeconds(attackAnimation.length * 0.2f);
        animator.SetBool(animatorBool, false);
    }

    //shoots a projectile
    public void CastProjectile() {
        GameObject spell = Instantiate(spellObject, castPoint.position, castPoint.rotation);
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
