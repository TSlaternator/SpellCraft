using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

	//Controls the doors in my game, primarily opening and closing them

	[SerializeField] private GameObject leftDoor; //the left door gameObject
	[SerializeField] private GameObject rightDoor; //the right door gameObject
	private bool isOpen; //whether the door is open or not
    private bool isLocked; //whether the door is locked or not
    private int doorFacing; //which wall the door is on (used to determine open/close direction)
    private Collider playerCollider;

    //get the players collider
    private void Start() {
        playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();
    }

    //controls what happens when something collides with the door
    void OnCollisionEnter(Collision other){
		if (isOpen) {
			Physics.IgnoreCollision (other.collider, this.GetComponent<Collider> ());
		}

		if (!isLocked && !isOpen && other.gameObject.tag == "Player") {
			Physics.IgnoreCollision (other.collider, this.GetComponent<Collider> ());
        }
	} 

    //controls what happens when the player collides with the door
    private void OnTriggerEnter(Collider other) {
        if (!isLocked && !isOpen && other.gameObject.tag == "Player") Open();
    }

    //controls what happens then the player stops colliding with the door
    private void OnTriggerExit(Collider other) {
        if (isOpen && other.gameObject.tag == "Player")  Close();
    }

    //opens the door
    private void Open(){
		isOpen = true;
        if (doorFacing % 2 == 0) {
            RotateOpen(-1, rightDoor);
            RotateOpen(1, leftDoor);
        } else {
            RotateOpenSide(-1, rightDoor);
            RotateOpenSide(1, leftDoor);
        }
	}

    //closes the door
    private void Close() {
        isOpen = false;
        if (doorFacing % 2 == 0) {
            RotateClosed(1, rightDoor);
            RotateClosed(-1, leftDoor);
        } else {
            RotateClosedSide(1, rightDoor);
            RotateClosedSide(-1, leftDoor);
        }
        Physics.IgnoreCollision(playerCollider, this.GetComponent<Collider>(), false);
    }

	//rotates the door sprites open
	private void RotateOpen(int direction, GameObject door){
        door.transform.Rotate (0, 90 * direction, 0);
        door.transform.Translate(0f, -0.3f, -0.5f);
        if (doorFacing == 0) door.transform.Translate(0f, 0.8f, 0f);
    }

    //rotates sideways facing doors open
    private void RotateOpenSide(int direction, GameObject door) {
        door.transform.GetChild(0).gameObject.SetActive(false);
        door.transform.Rotate(0, 90 * -direction, 0);
        door.transform.Translate(-0.5f * direction, 0f, -0.4f);
    }

    //rotates the door sprites closed
    private void RotateClosed(int direction, GameObject door) {
        if (doorFacing == 0) door.transform.Translate(0f, -0.8f, 0f);
        door.transform.Translate(0f, 0.3f, 0.5f);
        door.transform.Rotate(0, 90 * direction, 0);
    }

    //rotates side doors closed
    private void RotateClosedSide(int direction, GameObject door) {
        door.transform.GetChild(0).gameObject.SetActive(true);
        door.transform.Translate(-0.5f * direction, 0f, 0.4f);
        door.transform.Rotate(0, 90 * -direction, 0);
    }

    //sets the door facing
    public void setFacing(int facing) {
        doorFacing = facing;
    }

    //Locks the door
    public void LockDoor() {
        isLocked = true;
        if (isOpen) {
            isOpen = false;
            Close();
        } 
    }

    //Unlocks the door
    public void UnlockDoor() {
        isLocked = false;
    }
}
