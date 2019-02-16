using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSPAWNER : MonoBehaviour {

	/* Class for testing AI, spawns them in */

	[SerializeField] private GameObject enemy; //the enemy to spawn
	[SerializeField] private GameObject spawnList; //reference to the enemy list

	//spawns an enemy is SPACE is pressed
	void Update () {
		if (Input.GetButtonDown("KeyboardSPACE")) SpawnEnemy();
	}

	//spawns an enemy
	private void SpawnEnemy(){
		Instantiate (enemy, transform.position, transform.rotation, spawnList.transform);
	}
}
