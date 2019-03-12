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
    [SerializeField] private float phase2AttackCooldown; //how long the boss will wait between attacks in phase 2
    [SerializeField] private Animator animator; //animator of the boss
    [SerializeField] private NavMeshAgent agent; //the navmesh agent controlling this boss
    [SerializeField] private IBossSpecialAttack specialAttack; //the special attack of this boss
    [SerializeField] private float specialCooldown; //the cooldown of special attacks
    private bool phase2; //whether the boss is in its second phase or not
    private float nextAttack; //when the boss will make its next attack
    private float nextCooldown; //when the boss will next be in cooldown
    private Vector3 destination; //where the boss is headed
    private bool isDead; //whether the boss is dead or not
    private float nextSpecialAttack; //when the boss will use its next special attack

    //initialising variables
    private void Start() {
        nextCooldown = Time.time;
        nextAttack = Time.time + attackCooldown;
        attacks = GetComponents<IBossAttack>();
        lastAttack = attacks[0];
        animator.SetBool("IsMoving", true);
        destination = transform.position;
        specialAttack = GetComponent<IBossSpecialAttack>();
    }

    //controls boss behaviour
    private void Update() {
        if (!isDead) {
            if (Time.time > nextAttack) { //select next attack and update cooldown time
                SelectAttack();
                nextCooldown = Time.time + lastAttack.getCastDuration();
                nextAttack = nextCooldown + attackCooldown;
                agent.SetDestination(transform.position);
                destination = transform.position;
                animator.SetBool("IsMoving", true);
            } else if (Time.time > nextCooldown && Vector3.Magnitude(transform.position - destination) < 1f) { //if in cooldown, move 
                destination = getDestination();
                agent.SetDestination(destination);
                animator.SetBool("IsMoving", true);
            } 
            // periodically uses special attacks when in phase2
            if (phase2 && Time.time > nextSpecialAttack) {
                nextSpecialAttack = Time.time + specialCooldown;
                specialAttack.OnCast();
            }
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
            while (attackChance > 0) attackChance -= attackChances[++attackChoice];

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

    //called when the boss dies
    public void Die() {
        isDead = true;
        agent.SetDestination(transform.position);
        GetComponent<Collider>().enabled = false;
        agent.enabled = false;
    }

    //returns if the boss is dead or not
    public bool IsDead() {
        return isDead;
    }

    //turns the boss to its second phase
    public void setPhase2() {
        attackCooldown = phase2AttackCooldown;
        phase2 = true;
        lastAttack.Stop();
        for(int i = 0; i < attacks.Length; i++) {
            attacks[i].Phase2();
        }
        nextAttack = Time.time + 2f;
        nextCooldown = Time.time + 2f;
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsCast", false);
        animator.SetBool("IsDead", false);
        animator.SetBool("IsSpray", false);
        animator.SetBool("IsPulse", false);
        animator.SetBool("Phase2", true);
        nextSpecialAttack = Time.time + specialCooldown / 2;
    }
}
