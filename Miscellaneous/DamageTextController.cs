using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextController : MonoBehaviour {

	/* Controls damage numbers when an enemy is hit */

	[SerializeField] private Text text; //the text object to display damage numbers
	[SerializeField] private float moveSpeed; //how fast the text moves
	[SerializeField] private float lifeTime; //how long the text lasts

	//sets the lifetime of the text object
	void Start () {
		Destroy (gameObject, lifeTime);
	}
		
	//moves the text each frame
	void Update () {
		transform.position = new Vector3 (transform.position.x, transform.position.y + moveSpeed * Time.deltaTime, transform.position.z);	
	}

	//sets the text of the text object
	public void SetText(string newText){
		text.text = newText;
	}
}
