using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{

    [SerializeField] private Texture2D cursorSprite; //the cursor object used in game
    private bool isOpen = false; //whether the chest is open or not
    private List<Item> items; //list of items the chest is holding
    private ChestSlotController[] slots; //holds the chestSlotController items
    [SerializeField] private lootPool commonLoot; //holds all common loot drops
    [SerializeField] private lootPool uncommonLoot; //holds all uncommon loot drops
    [SerializeField] private lootPool rareLoot; //holds all rare loot drops
    [SerializeField] private lootPool epicLoot; //holds all epic loot drops
    private int loot; //loot value of the chest (used to generate components)
    private bool playerInProximity; //whether the player is in proximity or not
    private GameObject UI; // the UI for the chest
    private GameController gameController; //holds details of the chest UI
    private PlayerInventoryController inventory; //controls the players inventory
    private PauseMenu pauseMenu; //the pausemenu script
    [SerializeField] private Animator animator; //the animator attached to the object
    [SerializeField] private AnimationClip openAnimation; //used to get the time taken to open the chest

    void Start() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        slots = gameController.getChestSlots();
        UI = gameController.getChestUI();
        pauseMenu = gameController.getPauseMenu();
        inventory = gameController.getPlayerInventory();
    }

    //checks to see if the player is trying to open the chest
    private void Update() {
        if (playerInProximity && Input.GetKeyDown(KeyCode.Space) && !pauseMenu.IsPaused()) {
            if (!isOpen && inventory.UseKey()) {
                Initialise();
                isOpen = true;
                StartCoroutine(OpenAnimation());
            } else if (isOpen) OpenUI();
        }
    }

    private IEnumerator OpenAnimation() {
        animator.SetBool("IsOpen", true);
        yield return new WaitForSeconds(openAnimation.length * 1.5f);
        OpenUI();
    }

    //sets this as the active chest for the UI controllers
    private void setActiveChest() {
        for (int i = 0; i < slots.Length; i++) slots[i].setChest(this);
    }

    //called the first time a chest is opened
    private void Initialise() {
        items = new List<Item>();
        setActiveChest();
        GenerateLoot();
    }

    //detects whether the player is close enough to interact with the chest
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = true;
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = false;
    }

    //sets the loot value of the chest
    public void setLoot(int lootValue) {
        loot = lootValue;
    }

    //generates loot of the chest
    private void GenerateLoot() {
        while(loot >= commonLoot.value) {
            float poolChoice = Random.Range(0f, 1f);
            int lootChoice;
            if (poolChoice <= commonLoot.frequency) {
                lootChoice = Random.Range(0, commonLoot.itemPool.Length);
                Item itemChoice = commonLoot.itemPool[lootChoice];
                itemChoice.setInventoryController(inventory);
                AddItem(itemChoice);
                loot -= commonLoot.value;
            } else if (poolChoice <= uncommonLoot.frequency && loot >= uncommonLoot.value) {
                lootChoice = Random.Range(0, uncommonLoot.itemPool.Length);
                Item itemChoice = uncommonLoot.itemPool[lootChoice];
                itemChoice.setInventoryController(inventory);
                AddItem(itemChoice);
                loot -= uncommonLoot.value;
            } else if (poolChoice <= rareLoot.frequency && loot >= rareLoot.value) {
                lootChoice = Random.Range(0, rareLoot.itemPool.Length);
                Item itemChoice = rareLoot.itemPool[lootChoice];
                itemChoice.setInventoryController(inventory);
                AddItem(itemChoice);
                loot -= rareLoot.value;
            } else if (poolChoice <= epicLoot.frequency && loot >= epicLoot.value) {
                lootChoice = Random.Range(0, epicLoot.itemPool.Length);
                Item itemChoice = epicLoot.itemPool[lootChoice];
                itemChoice.setInventoryController(inventory);
                AddItem(itemChoice);
                loot -= epicLoot.value;
            }
            items[items.Count - 1].setInventoryController(inventory);
        }
        UpdateUI();
    }

    //opens the chest UI
    private void OpenUI() {
        pauseMenu.setPaused(true);
        UpdateUI();
        UI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    //closes the UI
    private void CloseUI() {
        pauseMenu.setPaused(false);
        Time.timeScale = 1f;
        UI.SetActive(false);
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }

    //adds an item to the chest
    private void AddItem(Item item) {
        items.Add(item);
    }
     
    //removes an item from the chest
    public void RemoveItem(Item item) {
        items.Remove(item);
        UpdateUI();
    }

    //Updates the UI to show current items
    public void UpdateUI() {
        for (int i = 0; i < slots.Length; i++) {
            if (i < items.Count) slots[i].AddItem(items[i]);
            else slots[i].EmptySlot();
        }
    }
}
