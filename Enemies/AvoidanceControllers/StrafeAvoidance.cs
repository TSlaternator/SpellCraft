using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StrafeAvoidance : MonoBehaviour, IEnemyAvoidanceController
{
    [SerializeField] private float avoidanceChance; //avoidance chance of the enemy (how likely to use avoidance when hit)
    private bool isAvoiding = false; //whether the enemy is currently avoiding or not
    [SerializeField] private NavMeshAgent agent; //controls movement
    private Vector3 destination; //where the agent is going
    private Transform player; //the players position
    [SerializeField] private float strafeDistance; //how far to strafe

    //gives the enemy a reference to the room its in
    public void SetRoom(RoomController room) {
        //Not neccessary for this implementation
    }

    //gets the players transform
    private void Start() {
        player = GameObject.Find("Player").transform;
    }

    //controls the duration of the avoidance behaviour
    private void Update() {
        if (isAvoiding && Vector3.Magnitude(transform.position - destination) < 1) {
            isAvoiding = false;
        } 
    }

    //what to do when the enemy is hit
    public void OnHit() {
        //this implementation tries strafing (moving perpendicular(ish)) to the player, to dodge further spells
        int attempts = 0; //stops infinite loop in the case that no dodge location can be found
        bool destinationFound = false; //sentinal value for the loop
        NavMeshHit hit; //navmesh hit data (used for debugging)
        //Try Picking a point a certain distance left/right thats pathable (within a sphere)
        while (!destinationFound && attempts < 10) {
            if (NavMesh.SamplePosition(transform.position + getAvoidancePoint(), out hit, 1f, NavMesh.AllAreas)) {
                destination = hit.position;
                agent.SetDestination(hit.position);
                destinationFound = true;
                isAvoiding = true;
            }
            attempts++;
        }
    }


    //returns the chance to use avoidance
    public float AvoidanceChance() {
        return avoidanceChance;
    }

    //returns true if currently avoiding
    public bool IsAvoiding() {
        return isAvoiding;
    }

    //gets a point to try and strafe to
    private Vector3 getAvoidancePoint() {
        bool direction = (Random.Range(0f, 1f) > 0.5); //whether to dodge left, or right
        Vector3 point = player.position - transform.position;
        point.y = 0;
        point.Normalize();
        if (direction) point = Quaternion.Euler(0, 90, 0) * point;
        else point = Quaternion.Euler(0, -90, 0) * point;
        point *= strafeDistance;
        return point;
    }
}
