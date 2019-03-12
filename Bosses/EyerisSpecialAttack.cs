using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyerisSpecialAttack : MonoBehaviour, IBossSpecialAttack
{
    [SerializeField] private GameObject minion; //Eyeris spawns flying eyeball minions that seek the player
    [SerializeField] private Transform[] castPoints; //possible points to spawn the minion at
    private Transform enemyList; //all enemies are spawned under this transform


    //initialising vairables
    void Start() {
        enemyList = GameObject.Find("EnemiesList").transform;
    }

    //casts the special attack
    public void OnCast() {
        int castPoint = Random.Range(0, castPoints.Length);
        Instantiate(minion, castPoints[castPoint].position - new Vector3(0f, 0.5f, 0f), castPoints[castPoint].rotation, enemyList);
    }
}
