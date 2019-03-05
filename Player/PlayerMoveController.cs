using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour {

	//Controls the movement of the player 

	[SerializeField] private float moveSpeed; //movement speed of the player
	[SerializeField] private Animator animator; //animator of the player sprites
	private SpellController spellController; //reference to the players spell controller
	private Rigidbody body; //reference to the players rigidbody
	private Vector3 moveInput; //movement key input
	private Vector3 moveVelocity; //velocity obtained from moveInput * moveSpeed

	//initialises components
	void Start () {
		body = GetComponent<Rigidbody> ();
		spellController = GetComponentInChildren<SpellController> ();
	}

	//determines the players movement and rotation
	void Update () {
		moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical"));
		moveVelocity = moveInput * moveSpeed;
		animator.SetFloat ("Speed", moveVelocity.magnitude);
	}

	//applies movement to the player
	void FixedUpdate (){
		body.velocity = moveVelocity;
        if (transform.position.y != 0.5f) transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
	}

    //sets the players speed to the amount supplied
    public void setSpeed(float newSpeed) {
        moveSpeed = newSpeed;
    }

    //gets the players moveSpeed
    public float getSpeed() {
        return moveSpeed;
    }

    //gets where the player will be if they keep moving in the same direction
    public Vector3 getFuturePosition() {
        return transform.position + body.velocity / 2;
    }
}
