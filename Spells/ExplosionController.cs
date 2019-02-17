using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

	/* Controls explosions and their effects */

	[SerializeField] int damageType; //the damage type of the explosion
	[SerializeField] float damage; //the damage of the explosion
	[SerializeField] ParticleSystem particles; //the explosions particle system

	//initialises audio clip and explosion damage 
	void Start () {
		particles.Play ();
		StartCoroutine (ApplyDamage ());
	}

	//simulates explosion damage to all nearby enemies
	private IEnumerator ApplyDamage(){
		yield return new WaitForSeconds (0.05f);
		Collider[] hitColliders = Physics.OverlapSphere (transform.position, 2.5f);
		for (int i = 0; i < hitColliders.Length; i++) {
			if (hitColliders [i].GetComponent<ObstructionController> ().enemy) {
				hitColliders [i].GetComponent<EnemyStatController> ().ApplyDamage (damage, damageType, false, false);
			}
		}
		yield return new WaitForSeconds (0.05f);
		Destroy (gameObject);
	}

	//sets the damage of the explosion
	public void SetDamage(float newDamage){
		damage = newDamage;
	}
}
