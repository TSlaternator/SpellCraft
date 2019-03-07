using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : MonoBehaviour, IEnemyMoveController
{
    //makes enemy wander around within range of the player
    private Vector3 destination; // where the enemy is headed
    private GameObject player; //the player object
    [SerializeField] private float closestDistance; //the closest the agent will get to the player

    //getting player and initial destination
    void Start() {
        player = GameObject.Find("Player");
        destination = player.transform.position;
    }

    //wanders around within range of the player
    public Vector3 getDestination(Vector3 currentPosition, Vector3 playerPosition, float range) {
        Debug.DrawRay(currentPosition, destination - currentPosition);
        CheckDestination(currentPosition, playerPosition, range);
        return destination;
    }

    //checks if the destination is still valid, if not, gets a new one
    private void CheckDestination(Vector3 currentPosition, Vector3 playerPosition, float range) {
        float distanceToPlayer = Vector3.Magnitude(currentPosition - playerPosition);
        float distanceToDestination = Vector3.Magnitude(currentPosition - destination);
        float destinationToPlayer = Vector3.Magnitude(destination - playerPosition);
        if (distanceToPlayer > range || destinationToPlayer < closestDistance || distanceToDestination < 1f) NewDestination(playerPosition, range);
    }

    //gets a new destination for the enemy
    private void NewDestination(Vector3 playerPosition, float range) {
        bool destinationFound = false;
        float xPos, zPos; //positions to test for pathability
        Vector3 testPoint; //point to set destinatin to
        NavMeshHit hit; //navmesh hit data (used for debugging)
        float moveRange = range - 2; //don't want to move to close to the edge or range, or will need to recalculate more often!
        while (!destinationFound) {
            xPos = Random.Range(playerPosition.x - moveRange, playerPosition.x + moveRange);
            zPos = Random.Range(playerPosition.z - moveRange, playerPosition.z + moveRange);
            testPoint = new Vector3(xPos, 0f, zPos);
            if (NavMesh.SamplePosition(testPoint, out hit, 0.1f, NavMesh.AllAreas)) {
                destination = testPoint;
                destinationFound = true;
            }
        }
    }
}
