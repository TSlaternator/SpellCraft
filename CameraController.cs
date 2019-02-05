using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	/* Controls the movement of the main camera */

	[SerializeField] private GameObject player; //reference to the player object
	private Vector3 offset; //cameras offset from the player object
	private PlayerController playerController; //reference to the player controller script
	private PlayerRotationController rotationController;

	void Start () {
		offset = transform.position - player.transform.position; //aquiring initial offset
		//playerController = player.GetComponent<PlayerController> ();
		rotationController = player.GetComponentInChildren<PlayerRotationController> ();
	}

	// Controls the cameras position each frame 
	void LateUpdate () {
		//transform.position = player.transform.position + offset + playerController.GetMouseOffset() * 0.15f; 
		transform.position = player.transform.position + offset + rotationController.GetMouseOffset() * 0.15f;
	}
}
