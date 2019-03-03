using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorSprite; //the cursor object used in game
    private GameController gameController; //game controller script
    private GameObject UI; //UI canvas for the shrine
    private PauseMenu pauseMenu; //the pauseMenu script
    private Shrine details; //details of this shrine
    private ShrineUIController UIController; //controls the UI
    private bool playerInProximity; //whether the player is close enough to interact with the shrine
    private bool activeShrine = true; //whether the shrine is active or not

    //initialises the variables
    void Start(){
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        pauseMenu = gameController.getPauseMenu();
        UI = gameController.getShrineUI();
        UIController = UI.GetComponent<ShrineUIController>();
        InitialiseShrine();
    }

    // Checks for player interaction
    void Update(){
        if (activeShrine && playerInProximity && Input.GetKeyDown(KeyCode.Space) && !pauseMenu.IsPaused()) {
            OpenUI();
        }
    }

    //detects whether the player is close enough to interact with the shrine
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = true;
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = false;
    }

    //opens the shrine UI
    private void OpenUI() {
        pauseMenu.setPaused(true);
        UpdateUI();
        UI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    //Updates the UI for the shrine
    private void UpdateUI() {
        UIController.UpdateUI(details, this);
    }

    //gets a type for the shrine
    private void InitialiseShrine() {
        details = gameController.getShrineType();
    }

    //called when a sacrifice is made, the shrine becomes inactive
    public void UseShrine() {
        activeShrine = false;
    }
}

//holds details of shrines
[System.Serializable]
public struct Shrine {
    public string name; //name of the shrine (shown in the UI)
    public string description; //description of the shrine (shown in the UI)
    public ShrineChoice[] choices; //holds the choices this shrine could grant the player
    public int activeChoice; //which choice the shrine will give
}

//holds details of which choice the shrine will give the player
[System.Serializable]
public struct ShrineChoice {
    public string description; //description of the choice (shown in the UI)
    public string sacrifice; //what the player must sacrifice to the shrine
    public Sprite sacrificeSprite; //shown in the UI
    public float sacrificeAmount; //how much the player must sacrifice
    public string reward; //what the player will get in return
    public float rewardAmount; //how much the player will get
}
