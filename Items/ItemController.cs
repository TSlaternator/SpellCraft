using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private SpriteRenderer sprite;
    private bool playerInProximity = false;

    // checks for player proximity and pickup button
    void Update() {
        if (playerInProximity && Input.GetKey(KeyCode.Space)) {
            PlayerInventoryController inventory = GameObject.Find("Player").GetComponent<PlayerInventoryController>();
            item.setInventoryController(inventory);
            inventory.PickupItem(item);
            Destroy(gameObject);
        }
    }

    //checks when the player enters the objects pickup radius
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") playerInProximity = true;
    }

    //checks when the player leaves the objects pickup radius
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") playerInProximity = false;
    }

    //sets up the sprite and item of the controller
    public void Initialise(Item newItem) {
        item = newItem;
        sprite.sprite = item.getSprite();
    }
}
