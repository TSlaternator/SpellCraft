using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyerisMinionController : MonoBehaviour
{

    private GameObject target; //will hold a reference to the player
    [SerializeField] private Rigidbody body; //the minions rigidbody (used to control movement)
    [SerializeField] private float speed; //speed of the minion
    [SerializeField] private float turnrate; //how fast the minion will turn
    [SerializeField] private float damage; //how much damage the minion deals on contact
    [SerializeField] private Animator animator; //animates the sprite
    [SerializeField] private Collider col; //the collider of the minion
    [SerializeField] private AnimationClip hitAnimation; //the animation that plays when the minion is hit
    [SerializeField] private AnimationClip deathAnimation; //the animation that plays whent he minion is killed
    private Vector3 velocity; //the velocity of the minion
    private SpriteRotationController sprite; //the RotationController of the minions sprite
    private bool marked; //if the enemy is marked or not (marked enemies take more damage from certain elements)
    private int markType; //the element that marked the enemy (will take more damage from all elements except this one)
    private float markMultiplier; //the multiplier applied to mark damage
    [SerializeField] private float maxHP; //maximum HP of the minion
    private float currentHP; //current HP of the minion
    [SerializeField] private float fireResistance; //fire resistance of the minion
    [SerializeField] private float waterResistance; //water resistance of the minion
    [SerializeField] private float airResistance; //air resistance of the minion
    [SerializeField] private float earthResistance; //earth resistance of the minion
    [SerializeField] private float orderResistance; //order resistance of the minion
    [SerializeField] private float chaosResistance; //chaos resistance of the minion
    [SerializeField] private GameObject[] FloatingDamageText; //damage numbers used when minion take damage
    [SerializeField] private Transform textList; //clearnup list to spawn the text under
    [SerializeField] private Transform floatingTextSpawn; //spawn point of the floating text
    [SerializeField] private float textSpawnOffset; //random horizontal offset of the texts spawn point
    [SerializeField] private GameObject manaDrop; //mana orb dropped if the player is using a manadrain rune
    private float[] resistances; //holds the resistances of the minion
    private Transform gameController; //transform always facing forwards (used to spawn damage text)

    //Initialising variables
    void Start() {
        target = GameObject.Find("Player");
        velocity = transform.forward * speed;
        body.velocity = velocity;
        sprite = GetComponentInChildren<SpriteRotationController>();
        gameController = GameObject.Find("GameController").transform;
        currentHP = maxHP;
        resistances = new float[] { fireResistance, waterResistance, airResistance, earthResistance, orderResistance, chaosResistance, 0f };
    }

    //updates every frame
    private void Update() {
        SeekTarget();
        transform.LookAt(transform.position + body.velocity);
        sprite.SetRotation(transform.rotation.eulerAngles.y);
        body.velocity = velocity;
        if (body.velocity.magnitude > 0.1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(body.velocity), Time.deltaTime * 0.01f);
        }
    }

    //determines what the projectile does when it hits a trigger collider
    void OnTriggerEnter(Collider col) {
        if (col.tag != "Item") {
            ObstructionController other = col.GetComponent<ObstructionController>();
            if (other.player) {
                other.GetComponent<PlayerStatController>().DealDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    //determines what the projectile does when it hits a physics collider
    void OnCollisionEnter(Collision col) {
        Collider colider = col.collider;
        ObstructionController other = colider.GetComponent<ObstructionController>();
        if (other.obstructing) {
            Destroy(gameObject);
        } else if (other.projectile) {
            Physics.IgnoreCollision(colider, this.GetComponent<Collider>());
        }
    }

    //seeks the current target
    private void SeekTarget() {
        Vector3 desiredVelocity = (new Vector3(target.transform.position.x, 0f, target.transform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z));
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        velocity += Vector3.ClampMagnitude(desiredVelocity - body.velocity, turnrate);
    }

    //applies damage to the minion
    public void ApplyDamage(float damage, int damageType, bool isCrit, bool isDoT) {
        float realDamage;
        if (marked && damageType != markType) realDamage = damage * markMultiplier * (1 - (resistances[damageType] / 100));
        else realDamage = damage * (1 - (resistances[damageType] / 100));
        realDamage = Mathf.Round(realDamage);
        currentHP -= realDamage;
        if (isDoT) DamageText(realDamage, 2);
        else if (isCrit) DamageText(realDamage, 1);
        else DamageText(realDamage, 0);
        StartCoroutine(HitAnimation());
        if (currentHP <= 0) Die();
    }

    //kills the boss, dropping rewards
    private void Die() {
        col.enabled = false;
        animator.SetBool("IsDead", true);
        gameObject.transform.SetParent(null);
        StartCoroutine(CorpseTime());
    }

    //How long the boss will stay visible as a corpse before despawning
    private IEnumerator CorpseTime() {
        yield return new WaitForSeconds(deathAnimation.length + 0.1f);
        Destroy(gameObject);
    }

    //Plays an animation when hit
    private IEnumerator HitAnimation() {
        animator.SetBool("IsHit", true);
        yield return new WaitForSeconds(hitAnimation.length - 0.01f);
        animator.SetBool("IsHit", false);
    }

    //drains mana from the enemy
    public void ManaDrain() {
        Instantiate(manaDrop, transform.position + GenerateOffset(), transform.rotation);
    }

    //generates a random offset to spawn pickups at
    private Vector3 GenerateOffset() {
        float offsetX = Random.Range(-0.5f, 0.5f);
        float offsetZ = Random.Range(-0.5f, 0.5f);
        return new Vector3(offsetX, 0f, offsetZ);
    }

    //spawns the damage number text
    private void DamageText(float damageNumber, int textType) {
        GameObject text;
        float offsetX = Random.Range(-textSpawnOffset, textSpawnOffset);
        Vector3 textSpawnPosition = new Vector3(floatingTextSpawn.position.x + offsetX, floatingTextSpawn.position.y, floatingTextSpawn.position.z);
        text = Instantiate(FloatingDamageText[textType], textSpawnPosition, gameController.rotation, textList);
        text.GetComponent<DamageTextController>().SetText(damageNumber.ToString());
    }

    //marks the enemy with an element, buffing all other elements' damage
    public void Mark(int type, float multiplier) {
        StopCoroutine(MarkEffect(type, multiplier));
        StartCoroutine(MarkEffect(type, multiplier));
    }

    //Coroutine to control mark duration
    public IEnumerator MarkEffect(int type, float multiplier) {
        marked = true;
        markType = type;
        markMultiplier = multiplier;
        yield return new WaitForSeconds(5f);
        marked = false;
    }

    //returns the health percentage
    public float GetHealthPercent() {
        return currentHP / maxHP;
    }

    //returns the current health
    public float GetHealth() {
        return currentHP;
    }

    //Applies a DoT to the enemy
    public void ApplyDoT(int damageType, float damage) {
        StopCoroutine(DoT(damageType, damage, 1f));
        StartCoroutine(DoT(damageType, damage, 1f));
    }

    //Applies the DoT's effects
    private IEnumerator DoT(int damageType, float damage, float interval) {
        float duration = Time.time + 5.5f;
        while (Time.time < duration) {
            yield return new WaitForSeconds(interval);
            ApplyDamage(damage, damageType, false, true);
        }
    }
}
