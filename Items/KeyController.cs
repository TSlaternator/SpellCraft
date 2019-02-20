using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    //checks when the player enters the keys pickup radius
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.isTrigger) {
            other.gameObject.GetComponent<PlayerInventoryController>().AddKey();
            Destroy(gameObject);
        }
    }
}
