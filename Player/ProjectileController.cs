using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

    /* Controls the movement of player projectiles */

    [SerializeField] private SpellEffectController effectController; //the SpellEffectController of the projectile
    private SpriteRotationController sprite; //the RotationController of the spells sprite
    private float speed; //the speed of the projectile
    [SerializeField] private float turnrate; //the turnrate of the projectile
    private Camera cam;

    private bool guided; //if the projectile is guided, it will follow the cursor
    private float lifetime = 5f; //the lifetime of the projectile
    private bool piercing; //if the projectile is piercing, it will pierce through enemies
    private int pierceCount; //how many enemies the projectile can pierce through
    private bool ethereal; //if the projectile is ethereal, it can pierce walls
    private int etherealTier; //the tier of the etheral effect (higher tiers are less restricted by walls)
    private bool evaporating; //low tier ethereal projectiles evaporate after moving through a wall, reducing their lifetime
    private bool rebounding; //if the projectile is rebounding it will bounce off of walls
    private int reboundCount; //how manny times the projectile can rebound
    private bool seeking; //if the projectile is seeking, it can seek out enemies
    private bool chaining; //if the projectile is chaining, it will chain to nearby enemies after hitting one
    private GameObject enemiesList; //the list of enemies in the room
    private int enemyCount; //how many enemies are in the room
    private GameObject target; //the target of seeking / chaining
    private bool targetFound; //if the target has been assigned or not
    private int seekingTier; //the tier of seeking (higher tiers prioritise close targets)
    private int chainingTargets; //how many times the projectile will chain
    private Vector3 velocity; //the velocity of the projectile
    private Rigidbody body; //the rigidbody of the projectile
    private bool blocking; //if the projectils is blocking, it will block other projectiles
    private int blockCount; //how many projectiles it will block
    private bool latching; //might get rid TODO
    private bool latched; //might get rid TODO
    [SerializeField] private bool twoD; //used whilst transitioning from 3D system to 2D

    //initialising projectiles
    void Start() {
        body = this.GetComponent<Rigidbody>();
        velocity = transform.forward * speed;
        body.velocity = velocity;
        enemiesList = GameObject.Find("EnemiesList");
        sprite = GetComponentInChildren<SpriteRotationController>();
        cam = GameObject.Find("CameraPosition").GetComponentInChildren<Camera>();
    }

    //applies movement effects
    void Update() {

        transform.LookAt(transform.position + body.velocity);
        //Debug.Log ("ROTATION: " + transform.rotation.x + ", " + transform.rotation.y + ", " + transform.rotation.z);
        if (twoD) sprite.SetRotation(transform.rotation.eulerAngles.y);

        if (guided && seeking) ExclusiveSeeking();

        if (guided) GuideProjectile();

        if (seeking && !targetFound) FindTarget();
        if (seeking) SeekTarget();

        if (evaporating) lifetime -= 2 * Time.deltaTime;
        else lifetime -= Time.deltaTime;

        if (lifetime <= 0f) Destroy(gameObject);

        body.velocity = velocity;

        if (body.velocity.magnitude > 0.1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(body.velocity), Time.deltaTime * 0.01f);
        }
    }

    //what the projectile does when hitting a trigger collider
    void OnTriggerEnter(Collider col) {
        if (col.tag != "Item") {
            ObstructionController other = col.GetComponent<ObstructionController>();
            if (other.enemy) {
                other.GetComponent<EnemyStatController>().ApplyDamage(effectController.GetDamage(other.GetComponent<EnemyStatController>()), effectController.GetDamageType(), effectController.GetCrit(), false);
                other.GetComponent<EnemyController>().ApplyImpact(effectController.GetImpact());
                effectController.ApplyEffects(other);
                if (chaining && chainingTargets > 0) {
                    guided = false;
                    targetFound = false;
                    effectController.ModifyDamage(0.67f);
                    if (FindOtherTarget(other.gameObject) == false)
                        seeking = true;
                    else {
                        DestroyThis();
                    }
                }
            }

            if (other.obstructing) {
                if (piercing && pierceCount > 0 && other.piercable) {
                    if (chaining && chainingTargets > 0)
                        chainingTargets--;
                    else {
                        pierceCount--;
                        seeking = false;
                    }
                } else if (chaining && chainingTargets > 0) {
                    chainingTargets--;
                } else {
                    DestroyThis();
                }
            }
        }
    }

    //what too do when the projectile hits a non-trigger collider
    void OnCollisionEnter(Collision col) {
        Collider colider = col.collider;
        ObstructionController other = colider.GetComponent<ObstructionController>();
        if (other.obstructing) {
            if (rebounding && reboundCount > 0 && other.reboundable) {
                reboundCount--;
                Rebound(col);
            } else if (ethereal && !other.blockingEthereal) {
                Physics.IgnoreCollision(colider, this.GetComponent<Collider>());
                if (etherealTier < 2)
                    evaporating = true;
            } else {
                DestroyThis();
            }
        } else if (other.enemyProjectile) {
            if (blocking && blockCount > 0) {
                Destroy(other.gameObject);
                blockCount--;
                if (blockCount == 0) {
                    DestroyThis();
                }
            } else {
                Physics.IgnoreCollision(colider, this.GetComponent<Collider>());
            }
        } else {
            Physics.IgnoreCollision(colider, this.GetComponent<Collider>());
        }
    }

    //sets the speed of the projectile
    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    //modifies the speed of the projectile
    public void ModifySpeed(float multiplier) {
        speed *= multiplier;
    }

    //sets the projectile as piercing
    public void PiercingKinetic() {
        piercing = true;
        pierceCount += 1;
    }

    //sets the projectile as rebounding
    public void ReboundingKinetic() {
        rebounding = true;
        reboundCount += 2;
    }

    //rebounds the projectile
    public void Rebound(Collision col) {
        Vector3 direction = Vector3.Reflect(velocity.normalized, col.contacts[0].normal);
        velocity = direction.normalized * speed;
    }

    //sets the projectile as seeking
    public void SeekingKinetic() {
        seekingTier++;
        seeking = true;
    }

    //sets the projectile as chaining
    public void ChainingKinetic() {
        chaining = true;
        chainingTargets++;
    }

    //sets the projectile as guided
    public void GuidedKinetic() {
        guided = true;
        turnrate *= 1.5f;
    }

    //sets the projectile as ethereal
    public void EtherealKinetic() {
        ethereal = true;
        etherealTier++;
    }

    //sets the projectile as latching
    public void LatchingKinetic() {
        latching = true;
    }

    //sets the projectile as blocking
    public void BlockingKinetic() {
        blocking = true;
        blockCount++;
    }

    //finds a target to seek
    private void FindTarget() {
        enemyCount = enemiesList.transform.childCount;
        if (seekingTier == 1) target = enemiesList.transform.GetChild(Random.Range(0, enemyCount)).gameObject;
        else if (seekingTier == 2) {
            target = enemiesList.transform.GetChild(0).gameObject;
            float minDistance = Vector3.Distance(enemiesList.transform.GetChild(0).position, transform.position);
            for (int i = 1; i < enemyCount; i++) {
                if (Vector3.Distance(enemiesList.transform.GetChild(i).position, transform.position) < minDistance) {
                    target = enemiesList.transform.GetChild(i).gameObject;
                    minDistance = Vector3.Distance(enemiesList.transform.GetChild(i).position, transform.position);
                }
            }
        }

        targetFound = true;
    }

    //finds a target that isnt the last target
    private bool FindOtherTarget(GameObject lastTarget) {

        int targetID = 0;
        bool searchFailed = false;

        enemyCount = enemiesList.transform.childCount;
        if (enemyCount > 0) target = enemiesList.transform.GetChild(targetID).gameObject;
        else searchFailed = true;

        if (target == lastTarget) {
            if (enemyCount > 1) {
                targetID = 1;
                target = enemiesList.transform.GetChild(targetID).gameObject;
            } else {
                searchFailed = true;
            }
        }

        if (!searchFailed) {
            float minDistance = Vector3.Distance(enemiesList.transform.GetChild(targetID).position, transform.position);
            for (int i = targetID; i < enemyCount; i++) {
                if (enemiesList.transform.GetChild(i).gameObject != lastTarget) {
                    if (Vector3.Distance(enemiesList.transform.GetChild(i).position, transform.position) < minDistance) {
                        target = enemiesList.transform.GetChild(i).gameObject;
                        minDistance = Vector3.Distance(enemiesList.transform.GetChild(i).position, transform.position);
                    }
                }
            }
        }

        if (!searchFailed) {
            targetFound = true;
            velocity = (new Vector3(target.transform.position.x, 0f, target.transform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z)) * speed;
        }

        return searchFailed;
    }

    //seeks the current target
    private void SeekTarget() {
        Vector3 desiredVelocity = (new Vector3(target.transform.position.x, 0f, target.transform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z));
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        velocity += Vector3.ClampMagnitude(desiredVelocity - body.velocity, turnrate);
    }

    //seeks the cursor
    private void GuideProjectile() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        float rayLength;

        if (playerPlane.Raycast(ray, out rayLength)) {
            Vector3 targetPoint = ray.GetPoint(rayLength);

            Vector3 desiredVelocity = (new Vector3(targetPoint.x, 0f, targetPoint.z) - new Vector3(transform.position.x, 0f, transform.position.z));
            desiredVelocity.Normalize();
            desiredVelocity *= speed;

            velocity += Vector3.ClampMagnitude(desiredVelocity - body.velocity, turnrate);
        }
    }

    //can only seek a target, or the cursor, chooses one
    private void ExclusiveSeeking() {
        if (Random.Range(0f, 1f) > 0.5f) {
            seeking = false;
        } else {
            guided = false;
            turnrate *= 0.67f;
        }
    }

    //destroys the projectile, applying its effects
    private void DestroyThis() {
        effectController.ApplyDeathEffects();
    }
}
