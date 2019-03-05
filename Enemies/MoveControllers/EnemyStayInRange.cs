using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStayInRange : MonoBehaviour, IEnemyMoveController
{
    //makes enemy stay on the edge of its range

    //if in range, stay still, otherwise, move towards player
    public Vector3 getDestination(Vector3 currentPosition, Vector3 playerPosition, float range) {
        if (Vector3.Magnitude(currentPosition - playerPosition) < (range - 1)) return currentPosition;
        else return playerPosition;
    }
}
