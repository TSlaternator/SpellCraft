using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour {

	/* Controls the players inventory */

	[SerializeField] private float gold; //how much gold the player owns
	[SerializeField] private Text goldText; //HUD value to display gold count

	//initialises HUD element
	void Start(){
		goldText.text = "" + gold;
	}

	//adds / deducts gold from the player
	public void EditGold(int change){
		gold += change;
		goldText.text = "" + gold;
	}
}
