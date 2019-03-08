using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    [SerializeField] private IBossAttack[] attacks; //the attacks this boss can make
    [SerializeField] private int[] attackChances; //the chance of each attack being chosen
    [SerializeField] private IBossAttack lastAttack; //the last attack this boss made
    [SerializeField] private float attackCooldown; //how long the boss will wait between attacks
    [SerializeField] private Animator animator; //animator of the boss
    [SerializeField] private NavMeshAgent agent; //the navmesh agent controlling this boss
    private bool phase2; //whether the boss is in its second phase or not
    private float nextAttack; //when the boss will make its next attack
    private float nextCooldown; //when the boss will next be in cooldown
    private Vector3 destination; //where the boss is headed

    //initialising variables
    private void Start() {
        nextCooldown = Time.time;
        nextAttack = Time.time + attackCooldown;
        attacks = GetComponents<IBossAttack>();
        lastAttack = attacks[0];
        animator.SetBool("IsMoving", true);
        destination = transform.position;
    }

    //controls boss behaviour
    private void Update() {
        if (Time.time > nextAttack) { //select next attack and update cooldown time
            SelectAttack();
            nextCooldown = Time.time + lastAttack.getCastDuration();
            nextAttack = nextCooldown + attackCooldown;
            agent.SetDestination(transform.position);
            destination = transform.position;
        } else if (Time.time > nextCooldown && Vector3.Magnitude(transform.position - destination) < 1f) { //if in cooldown, move 
            destination = getDestination();
            agent.SetDestination(destination);
        }
    }

    //gets a new destination for the boss
    private Vector3 getDestination() {
        Vector3 newDestination = new Vector3(0f, 0f, 0f);
        bool destinationFound = false;
        NavMeshHit hit;
        while (!destinationFound) {
            newDestination = transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * 5f;
            newDestination.y = 1f;
            if (NavMesh.SamplePosition(newDestination - new Vector3(0f, 1f, 0f), out hit, 0.1f, NavMesh.AllAreas)) {
                destination = newDestination;
                destinationFound = true;
            }
        }
        return newDestination;
    }

    //Selects which attack to use next
    private void SelectAttack() {
        bool attackSelected = false; //controls the loop
        int attackChoice = -1; //which attack is being chosen
        while (!attackSelected) {
            int attackChance = Random.Range(0, 100);
            //loop to fairly select which attack to use 
            while (attackChance >= 0) attackChance -= attackChances[++attackChoice];

            if (attacks[attackChoice] != lastAttack && attackChances[attackChoice] >= 0) {
                attackSelected = true;
                //Loop to alter chances of the attacks coming up (ones cast frequently become less likely to be selected in future)
                for (int i = 0; i < attackChances.Length; i++) {
                    if (i == attackChoice) attackChances[i] -= 5;
                    else attackChances[i]++;
                }
                lastAttack = attacks[attackChoice]; 
                attacks[attackChoice].OnCast(); //uses the attack
            }
        }
    }
}
