using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPushPlayer : MonoBehaviour, IEnemyMoveController 
{
    //makes enemy push the player (try and get as close as possible)

    //always moves closer to the player
    public Vector3 getDestination(Vector3 currentPosition, Vector3 playerPosition, float range) {
        return playerPosition;
    }
}
