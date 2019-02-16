using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PickupController : MonoBehaviour {

	/* Controls various pickups dropped on enemy death, such
	 * as gold, mana crystals etc */

	[SerializeField] GameObject pickup; //this pickup
	[SerializeField] float speed; //speed of this pickup;
	[SerializeField] private NavMeshAgent agent; //navmesh agent of this pickup
	[SerializeField] private int pickupValue; //how much mana/gold this pickup is worth
	[SerializeField] private int pickupType; //the type of this pickup (0 = gold, 1 = mana)

	private PlayerStatController playerStats; //reference to the players stat controller script
	private PlayerInventoryController playerInvent; //reference to the players invent controller script
	private GameObject player; //reference to the player

	//initialises variables
	void Start () {
		player = GameObject.Find ("Player");
		playerStats = player.GetComponent<PlayerStatController> ();
		playerInvent = player.GetComponent<PlayerInventoryController> ();
	}

	//updates target destination
	void Update() {
		agent.SetDestination (player.transform.position);
	} 

	//applies the pickups effect if in contact with the player
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			switch (pickupType) {
			case 0: playerInvent.EditGold (pickupValue); break;
			case 1: playerStats.AddMana(pickupValue); break;
            case 2: playerStats.Heal(pickupValue); break;
            }
		}

		Destroy (gameObject);
	}
}
