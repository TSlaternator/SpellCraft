using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	/* Controls the movement and rotation of the player */

	[SerializeField] private SpellController spellController; //reference to the players spell controller
	[SerializeField] private float moveSpeed; //movement speed of the player
	[SerializeField] private Camera cam; //main camera in the scene

	private Rigidbody body; //reference to the players rigidbody
	private Vector3 moveInput; //movement key input
	private Vector3 moveVelocity; //velocity obtained from moveInput * moveSpeed
	private Vector3 mousePosition; //the position of the mouse
	private Vector3 mouseOffset; //offset of the mouse position from the player position
    private Vector3 startPosition; //position of the player when the map spawns in

	//initialises components
	void Start () {
		body = GetComponent<Rigidbody> ();
		spellController = GetComponent<SpellController> ();
        transform.position = startPosition;
	}

	//determines the players movement and rotation
	void Update () {
		if (!spellController.GetMeditating ()) {
			moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical"));
			moveVelocity = moveInput * moveSpeed;


			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			Plane playerPlane = new Plane (Vector3.up, transform.position);
			float rayLength; 

			if (!PauseMenu.isPaused) {
				if (playerPlane.Raycast (ray, out rayLength)) {
					Vector3 lookPoint = ray.GetPoint (rayLength);
					mouseOffset = new Vector3 (lookPoint.x - transform.position.x, 0f, lookPoint.z - transform.position.z);
					transform.LookAt (new Vector3 (lookPoint.x, transform.position.y, lookPoint.z));
				} 
			}
		} else {
			moveInput = new Vector3 (0f, 0f, 0f);
			moveVelocity = moveInput * moveSpeed;
		}
	}

    //sets the start position of the player
    public void setStartPosition(Vector3 position) {
        startPosition = position;
    }

	//applies movement to the player
	void FixedUpdate (){
		body.velocity = moveVelocity;
	}

	//returns the offset of the mouse from the player 
	public Vector3 GetMouseOffset(){
		return mouseOffset;
	}
}
