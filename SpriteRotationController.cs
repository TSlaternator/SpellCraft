using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotationController : MonoBehaviour {

	/* controls the rotation of projectile sprites */

	[SerializeField] private Transform transform2; //the transform of the sprites parent gameObject

	//sets the rotation of the sprite
	public void SetRotation(float rotationZ){
		transform2.eulerAngles = new Vector3(45f, 0f, -rotationZ);
	}
}
