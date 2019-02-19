using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INVENTORY_TESTER : MonoBehaviour
{

    [SerializeField] private Item[] itemPool; //possible items that can be added
    [SerializeField] private int itemsCount; //how many items to add
    [SerializeField] private PlayerInventoryController inventory; //the players inventory

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            for (int i = 0; i < itemsCount; i++) {
                inventory.PickupItem(itemPool[Random.Range(0, itemPool.Length)]);
            }
        }
    }
}
