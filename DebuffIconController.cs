using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuffIconController : MonoBehaviour {

	/* Controls Debuff Icons above enemies */

	[SerializeField] private Transform location; //Location of the enemy
	[SerializeField] private Image image; //the Image object to contain the icon

	//moves the debuff with the enemy
	void Update(){
			transform.position = location.position;
	}

	//sets the duration of the debuff
	public void SetLifetime(float lifetime){
		Destroy (gameObject, lifetime);
	}

	//Sets the icon of the debuff
	public void SetSprite(Sprite icon){
		image.sprite = icon;
	}

	//sets the transform reference of the enemy
	public void SetLocation(Transform moveLocation){
		location = moveLocation;
	}
}
