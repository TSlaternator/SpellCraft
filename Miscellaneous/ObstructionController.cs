using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionController : MonoBehaviour {

	/* Controls the properties of all obstructing objects 
	 * (anything with a collider) in the game */

	public bool obstructing; //whether the obstruction blocks other objects
	public bool piercable; //whether the obstruction is piercable by piercing spells
	public bool reboundable; //whether the obstruction can rebound projectiles
	public bool enemy; //whether the obstruction is an enemy
    public bool enemyProjectile; //whether the obstruction is an enemy projectile;
	public bool projectile; //whether the obstruction is a projectile
	public bool blockingEthereal; //whether the obstruction is piercable by ethereal spells
	public bool player; //whether the obstruction is the player
    public bool boss; //whether the obstruction is a boss
    public bool minion; //whether the obstruction is a boss' minion
}
