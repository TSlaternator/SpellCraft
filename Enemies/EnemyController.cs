 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    /* Controls enemy agents and their actions */

    [SerializeField] private IEnemyAvoidanceController avoidanceController; //controls avoidance mechanic
    [SerializeField] private IEnemyMoveController moveController; //controls movement mechanics
    [SerializeField] private IEnemySpellController spellController; //controls casting
    [SerializeField] private Transform castPoint; //cast point of the enemy
	[SerializeField] private float fireRate; // the fireRate of the enmy
	[SerializeField] private float range; //the range the enemy can fire to
	[SerializeField] private float staggerResist; //how resistant the enemy is to  staggering
	[SerializeField] private NavMeshAgent agent; //controls movement
	[SerializeField] private Transform textList; //clearnup list to spawn the text under
	[SerializeField] private Transform floatingDebuffPosition; //spawn point of the floating debuffIcon
	[SerializeField] private GameObject debuffIcon; //the debuffIcon canvas spawned to show debuffs
	[SerializeField] private Animator animator; //sprite animator
	private DebuffIconController currentDebuff; //the current debuff instance
	private GameObject debuff; //the debuff gameObject (used to call methods)
	private Transform gameController; //transform always facing forwards (used to spawn damage text)
	private GameObject player; //the player
    private PlayerMoveController playerMovement; //the movement controller of the player
	private float nextFire; //next time the agent can fire
	private bool knockback; //whether the agent is being knocked back
	private Vector3 knockbackDirection; //direction of the knockback
	private float knockbackDuration; //duration of the knockback
	private bool snared; //whether the agent is snared
	[SerializeField] private Sprite snareIcon; //the icon of the snare debuff
	private bool sleeping; //whether the agent is asleep
	[SerializeField] private Sprite sleepIcon; //the sleep debuff icon
	private DebuffIconController currentSleepIcon; //used to Destroy() the icon when the agent wakes
	private bool wakeable; //if the agent is wakeable or not
	private bool stunned; //if the agent is stunned
	[SerializeField] private Sprite stunIcon; //the icon of the stun debuff
	private bool slowed; //whether the agent is slowed
	[SerializeField] private Sprite slowIcon; //the slow debuff icon
	private bool moving; //whether the sprite is moving
	[SerializeField] private AnimationClip castAnimation; //the cast animation of the enemy
	public bool isDead; //whether the enemy is dead or not
    private Vector3 destination; //where the enemy is going
    [SerializeField] private bool predictive; //whether the enemy predicts the players movement or not

	//initialising varibles
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		spellController = GetComponent<IEnemySpellController> ();
        moveController = GetComponent<IEnemyMoveController>();
        avoidanceController = GetComponent<IEnemyAvoidanceController>();
		nextFire = Time.time + 3f; //small delay before firing so they don't shoot the second the player enters the room
		gameController = GameObject.Find ("GameController").transform;
        playerMovement = player.GetComponent<PlayerMoveController>();
	}

	// controls debuffs and when they end, as well as the agents FSM
	void Update () {
		if (!isDead) {
			moving = false;

			if (!sleeping && !stunned) {
				if (knockback) {
					nextFire += Time.deltaTime;
				} else if (snared) {
					agent.SetDestination (transform.position);
					if (Vector3.Magnitude (getPlayerPosition() - transform.position) < range && LineOfSight()) Cast ();
				} else if (!avoidanceController.IsAvoiding()) {
					if (Vector3.Magnitude (getPlayerPosition() - transform.position) < range && LineOfSight()) {
                        agent.SetDestination(transform.position);
                        Cast();
					} else {
                        agent.SetDestination(moveController.getDestination(transform.position, player.transform.position, range)); 
                        moving = true;
                    }
				}
				transform.LookAt (getPlayerPosition());
			} else {
				nextFire += Time.deltaTime;
			}

			animator.SetBool ("IsMoving", moving);
		}
	}

	//casts a spell (at the player)
	private void Cast(){
		if (Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			StartCoroutine (CastAnimation ());
		}
	}

    //plays the cast animation
	private IEnumerator CastAnimation(){
		animator.SetBool ("IsCasting", true);
		yield return new WaitForSeconds (castAnimation.length / 2f);
		spellController.Shoot ();
		yield return new WaitForSeconds (castAnimation.length / 2f -0.1f);
		animator.SetBool ("IsCasting", false);
	}

	//applies spell impact to the agent
	public void ApplyImpact(float impact){
		if (sleeping && wakeable) Wake ();
		knockbackDuration = impact * (1 - staggerResist);
		//StartCoroutine (Knockback (knockbackDuration));
	}

	//while knocked back, the agent won't be able to cast / move
	private IEnumerator Knockback(float duration){
		knockback = true;
		yield return new WaitForSeconds (duration);
		knockback = false;
	}

	//snares the agent, preventing it from moving
	public void ApplySnare(){
        StopCoroutine(SnareEffect(5f));
        debuff = Instantiate (debuffIcon, floatingDebuffPosition.position, gameController.rotation, textList);
		currentDebuff = debuff.GetComponent<DebuffIconController> ();
		currentDebuff.SetLifetime (5f);
		currentDebuff.SetSprite (snareIcon);
		currentDebuff.SetLocation (floatingDebuffPosition);
        StartCoroutine(SnareEffect(5f));
	}

    //snares the enemy
    private IEnumerator SnareEffect(float duration) {
        snared = true;
        yield return new WaitForSeconds(duration);
        snared = false;
    }

	//puts the agent to sleep, preventing it from doing anything
	public void ApplySleep(){
		if (!sleeping) {
			agent.SetDestination (transform.position);
			debuff = Instantiate (debuffIcon, floatingDebuffPosition.position, gameController.rotation, textList);
			currentSleepIcon = debuff.GetComponent<DebuffIconController> ();
			currentSleepIcon.SetSprite (sleepIcon);
			currentSleepIcon.SetLocation (floatingDebuffPosition);
			sleeping = true;
			StartCoroutine (AwakeImmunity ());
		}
	}

	//wakes a sleeping agent up
	private void Wake(){
		Destroy (currentSleepIcon.gameObject);
		sleeping = false;
	}

	//controls a breif period where the agent will stay asleep even it hit
	private IEnumerator AwakeImmunity(){
		wakeable = false;
		yield return new WaitForSeconds (0.5f);
		wakeable = true;
	}

	//applies a stun to the agent, preventing it from doing anything
	public void ApplyStun(){
        StopCoroutine(StunEffect(3f));
        debuff = Instantiate (debuffIcon, floatingDebuffPosition.position, gameController.rotation, textList);
		currentDebuff = debuff.GetComponent<DebuffIconController> ();
		currentDebuff.SetLifetime (3f);
		currentDebuff.SetSprite (stunIcon);
		currentDebuff.SetLocation (floatingDebuffPosition);
        StartCoroutine(StunEffect(3f));
	}

    //stuns the enemy
    private IEnumerator StunEffect(float duration) {
        stunned = true;
        yield return new WaitForSeconds(duration);
        stunned = false;
    }

	//applies a slow to the agent, slowing its movement and fire rate
	public void ApplySlow(){
        StopCoroutine(SlowEffect(5f));
		debuff = Instantiate (debuffIcon, floatingDebuffPosition.position, gameController.rotation, textList);
		currentDebuff = debuff.GetComponent<DebuffIconController> ();
		currentDebuff.SetLifetime (5f);
		currentDebuff.SetSprite (slowIcon);
		currentDebuff.SetLocation (floatingDebuffPosition);
        StartCoroutine(SlowEffect(5f));
	}

    //slows the enemy
    private IEnumerator SlowEffect(float duration) {
        slowed = true;
        fireRate *= 1.5f;
        agent.speed /= 2f;
        agent.angularSpeed /= 2f;
        yield return new WaitForSeconds(duration);
        slowed = false;
        fireRate /= 1.5f;
        agent.speed *= 2f;
        agent.angularSpeed *= 2f;
    }

    //returns true if the enemy can see the player, false if not
    public bool LineOfSight() {
        bool LOS = false;
        RaycastHit hit;

        if (Physics.Raycast(castPoint.position, getPlayerPosition() - castPoint.position, out hit, range)) {
            if (hit.collider.gameObject.tag == "Player") LOS = true;
        }

        return LOS;
    }

    //called when the enemy dies
    public void Die() {
        isDead = true;
        moving = false;
        agent.SetDestination(transform.position);
        GetComponent<Collider>().enabled = false;
        agent.enabled = false;
        if (sleeping) Wake();
    }

    //gets the players position, or future position
    private Vector3 getPlayerPosition() {
        if (predictive) return playerMovement.getFuturePosition();
        else return player.transform.position;
    }
}
