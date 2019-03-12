using System.Collections;
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
    private BossController bossController; //used to check if the boss is dead or not
    [SerializeField] private float rotationSpeed; //the speed the attack rotates at
    [SerializeField] private float phase2Speed; //speed of the projectiles fired in phase 2
    [SerializeField] private float phase2FireRate; //speed of fire in phase2
    [SerializeField] private float phase2RotationSpeed; //the speed the attack rotates at

    //initialising
    void Start() {
        bossController = GetComponent<BossController>();
    }

    //controls the attack
    void Update() {
        if (!bossController.IsDead() && Time.time < attackEndTime) {
            if (Time.time > nextFire) {
                for (int i = 0; i < castPoints.Length; i++) {
                    CastProjectile(i); 
                }
                transform.RotateAround(transform.position, transform.up, rotationSpeed);
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

    //Makes the attack more aggressive
    public void Phase2() {
        speed = phase2Speed;
        fireRate = phase2FireRate;
        rotationSpeed = phase2RotationSpeed;
    }

    //stops the attack
    public void Stop() {
        attackEndTime = Time.time - 0.1f;
        animator.SetBool(animatorBool, false);
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
