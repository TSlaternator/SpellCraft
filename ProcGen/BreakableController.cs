using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableController : MonoBehaviour
{
    [SerializeField] private Animator animator; //animator of the breakable
    [SerializeField] private Collider col; //collider of the breakable

    //Controls what happens when something collides with the breakable
    //In that even, plays the break animation, turns the collider off, 
    //and destroys the object after 5 seconds
    private void OnCollisionEnter(Collision collision) {
        col.enabled = false;
        animator.SetBool("Breaking", true);
        Destroy(gameObject, 5f);
    }
}
