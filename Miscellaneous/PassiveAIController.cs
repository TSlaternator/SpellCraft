using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PassiveAIController : MonoBehaviour {

    [SerializeField] private NavMeshAgent agent; //controls movement
    private Vector3 destination; //where the agent is headed
    private bool active = false; //turned true once the agent is initialised
    private float xCenter, zCenter;
    private int width, height;

    //wanders around the room
    void Update() {
        if (active && Vector3.Magnitude(agent.transform.position - destination) < 0.5) {
            NewDestination();
        }
    }

    //initialises the agent
    public void InitialiseAgent(float roomCenterX, float roomCenterZ, int roomWidth, int roomHeight) {
        xCenter = roomCenterX;
        zCenter = roomCenterZ;
        width = roomWidth;
        height = roomHeight;
        NewDestination();
        active = true;
    }

    //gets a new destination for the agent
    private void NewDestination() {
        bool destinationFound = false;
        float xPos, zPos; //positions to test for pathability
        Vector3 testPoint; //point to set destinatin to
        NavMeshHit hit; //navmesh hit data (used for debugging)
        while (!destinationFound) {
            xPos = Random.Range(xCenter - width / 2, xCenter + width / 2);
            zPos = Random.Range(zCenter - height / 2, zCenter + height / 2);
            testPoint = new Vector3(xPos, 0f, zPos);
            if (NavMesh.SamplePosition(testPoint, out hit, 0.1f, NavMesh.AllAreas)) {
                destination = testPoint;
                agent.SetDestination(destination);
                destinationFound = true;
            }
        }
    }
}
