using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScribingAltarController : MonoBehaviour
{
    private bool playerInProximity = false; //whether the player is in proximity or not
    private PauseMenu pauseMenu; //the pausemenu script
    private GameController gameController; //the gameController script

    //used for initialising
    private void Start() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        pauseMenu = gameController.getPauseMenu();
    }

    // Checks for the player trying to interact
    void Update(){
        if (playerInProximity && Input.GetKeyDown(KeyCode.Space) && !pauseMenu.IsPaused()) {
            pauseMenu.SpellMenu();
        }
    }

    //detects whether the player is close enough to interact with the altar
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = true;
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = false;
    }
}
