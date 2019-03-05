using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNoAvoidance : MonoBehaviour, IEnemyAvoidanceController
{
    [SerializeField] private float avoidanceChance; //avoidance chance of the enemy (how likely to use avoidance when hit)
    private bool isAvoiding = false; //whether the enemy is currently avoiding or not
    [SerializeField] private NavMeshAgent agent; //controls movement
    private Vector3 destination; //where the agent is going

    //what to do when the enemy is hit
    public void OnHit() {
        //this implementation does not avoid, and so this method is empty
    }

    //returns the chance to use avoidance
    public float AvoidanceChance() {
        return avoidanceChance;
    }

    //returns true if currently avoiding
    public bool IsAvoiding() {
        return isAvoiding;
    }
}
