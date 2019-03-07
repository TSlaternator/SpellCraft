using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EtherealAvoidance : MonoBehaviour, IEnemyAvoidanceController
{
    [SerializeField] private float avoidanceChance; //avoidance chance of the enemy (how likely to use avoidance when hit)
    private bool isAvoiding = false; //whether the enemy is currently avoiding or not
    [SerializeField] private NavMeshAgent agent; //controls movement
    private Vector3 destination; //where the agent is going
    [SerializeField] private AnimationClip agentHit; //the animation that plays when the agent is hit
    [SerializeField] private Collider col; //the collider of the enemy
    private float roomXCenter; //the center x point of the room this is in
    private float roomZCenter; //the center z point of the room this is in
    private int roomWidth; //the width of the room this is in
    private int roomHeight; //the height of the room this is in

    //gives the enemy a reference to the room its in
    public void SetRoom(RoomController room) {
        roomWidth = room.getWidth();
        roomHeight = room.getHeight();
        Vector3 roomPosition = room.transform.position;
        roomXCenter = roomPosition.x;
        roomZCenter = roomPosition.z;
    }

    //controls the duration of the avoidance behaviour
    private void Update() {
        if (isAvoiding && Vector3.Magnitude(transform.position - destination) < 1) {
            isAvoiding = false;
        }
    }

    //what to do when the enemy is hit
    public void OnHit() {
        //(the animation will make the enemy vanish), they will then 'reappear' at this new destination:
        //Calculate point in the room
        //Need to get access to the room
        bool positionFound = false; //will repeat until true!
        float xPos, zPos; //positions to move to
        Vector3 spawnPoint; //point to move the mob to
        NavMeshHit hit; //navmesh hit data (used for debugging)
        while (!positionFound) {
            xPos = Random.Range(roomXCenter - roomWidth / 2, roomXCenter + roomWidth / 2);
            zPos = Random.Range(roomZCenter - roomHeight / 2, roomZCenter + roomHeight / 2);
            spawnPoint = new Vector3(xPos, 0f, zPos);
            if (NavMesh.SamplePosition(spawnPoint, out hit, 0.1f, NavMesh.AllAreas)) {
                destination = hit.position;
                positionFound = true;
                StartCoroutine(Vanish(destination));
            }
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

    //Makes the enemy vanish, then reappear at a new destination
    private IEnumerator Vanish(Vector3 newPosition) {
        isAvoiding = true;
        col.enabled = false;
        yield return new WaitForSeconds(agentHit.length / 2f);
        transform.position = destination;
        yield return new WaitForSeconds(agentHit.length / 2f);
        col.enabled = true;
        isAvoiding = false;
    }
}
