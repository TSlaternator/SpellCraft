using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFacingController : MonoBehaviour {

	/* Class to control facing of sprites so they face the camera */

	[SerializeField] private Transform orientation; //orientation the sprites should face

	//initialises variables
	void Start(){
		orientation = GameObject.Find("SpriteFacing").transform;
	}

	//ensures the sprite is facing the right way
	void Update(){
		transform.rotation = orientation.rotation;
	}
}
