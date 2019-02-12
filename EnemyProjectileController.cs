using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour {

	/* Controls the movement, and effects of enemy projectiles */

	private float speed; //movement speed of the projectile

	private float damage; //damage of the projectile
	private float lifetime = 5f; //lifetime of the projectile
	private Vector3 velocity; //the projectiles velocity
	private Rigidbody body; //reference to the projectiles rigidbody
	private SpriteRotationController sprite; //the RotationController of the spells sprite

	//initialises rigidbody and velocity
	void Start () {
		body = this.GetComponent<Rigidbody> ();
		velocity = transform.forward * speed;
		body.velocity = velocity;
		sprite = GetComponentInChildren<SpriteRotationController> ();
	}

	//controls the projectiles movement
	void Update(){

		transform.LookAt (transform.position + body.velocity);
		sprite.SetRotation(transform.rotation.eulerAngles.y);

		lifetime -= Time.deltaTime;
		if (lifetime <= 0f) Destroy (gameObject);

		body.velocity = velocity;

		if (body.velocity.magnitude > 0.1) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (body.velocity), Time.deltaTime * 0.01f);
		} 
	}

	//determines what the projectile does when it hits a trigger collider
	void OnTriggerEnter(Collider col){
		ObstructionController other = col.GetComponent<ObstructionController> ();
		if (other.player) {
			other.GetComponent<PlayerStatController> ().DealDamage (damage);
		}

		if (other.obstructing) {			
				Destroy (gameObject);
		}
	}

	//determines what the projectile does when it hits a physics collider
	void OnCollisionEnter(Collision col){
		Collider colider = col.collider;
		ObstructionController other = colider.GetComponent<ObstructionController> ();
		if (other.obstructing) {
				Destroy (gameObject);
		} else if (other.projectile) {
			Physics.IgnoreCollision (colider, this.GetComponent<Collider> ());
		}
	}

	//sets the speed of the projectile
	public void SetSpeed(float newSpeed){
		speed = newSpeed;
	}

	//multiplies the speed of the projectile by the supplied parameter
	public void ModifySpeed(float multiplier){
		speed *= multiplier;
	}

	//sets the damage of the projectile
	public void SetDamage(float newDamage){
		damage = newDamage;
	}

	//multiplies the damage of the projectile by the supplied parameter
	public void ModifyDamage(float multiplier){
		damage *= multiplier;
	}
}
