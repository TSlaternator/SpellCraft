using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationController : MonoBehaviour {

	/* Controls the rotation of the player */

	[SerializeField] private Camera cam; //main camera in the scene
	[SerializeField] private Animator animator; //animator for the player sprites
	private Vector3 mousePosition; //the position of the mouse
	private Vector3 mouseOffset; //offset of the mouse position from the player position

	//determines the players movement and rotation
	void Update () {

		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		Plane playerPlane = new Plane (Vector3.up, transform.position);
		float rayLength; 

		if (!PauseMenu.isPaused) {
			if (playerPlane.Raycast (ray, out rayLength)) {
				Vector3 lookPoint = ray.GetPoint (rayLength);
				GetFacing (lookPoint - transform.position);
				mouseOffset = new Vector3 (lookPoint.x - transform.position.x, 0f, lookPoint.z - transform.position.z);
				transform.LookAt (new Vector3 (lookPoint.x, transform.position.y, lookPoint.z));
			} 
		}
	}

	//returns the offset of the mouse from the player 
	public Vector3 GetMouseOffset(){
		return mouseOffset;
	}

	//gets the facing of the player
	private void GetFacing(Vector3 mouseOffset){
		if (mouseOffset.x > 0) {
			if (mouseOffset.z > 0) {
				animator.SetInteger ("Facing", 2);
			} else {
				animator.SetInteger ("Facing", 0);
			}
		} else {
			if (mouseOffset.z > 0) {
				animator.SetInteger ("Facing", 3);
			} else {
				animator.SetInteger ("Facing", 1);
			}
		}
	}
}
