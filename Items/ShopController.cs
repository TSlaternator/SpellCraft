using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorSprite; //the cursor object used in game
    private bool isVisited = false; //whether the chest is open or not
    private List<Item> items; //list of items the chest is holding
    private ShopSlotController[] slots; //holds the shopSlotController items
    [SerializeField] private lootPool commonStock; //holds all common loot drops
    [SerializeField] private lootPool uncommonStock; //holds all uncommon loot drops
    [SerializeField] private lootPool rareStock; //holds all rare loot drops
    [SerializeField] private lootPool epicStock; //holds all epic loot drops
    private int loot; //loot value of the chest (used to generate components)
    private bool playerInProximity; //whether the player is in proximity or not
    private GameObject UI; // the UI for the shop
    private GameController gameController; //holds details of the chest UI
    private PauseMenu pauseMenu; //the pausemenu script
    private Text playerMoney; //displays the players money
    private PlayerInventoryController inventory; //reference to the players inventory

    void Start() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        slots = gameController.getShopSlots();
        UI = gameController.getShopUI();
        pauseMenu = gameController.getPauseMenu();
        playerMoney = gameController.getPlayerMoneyText();
        inventory = gameController.getPlayerInventory();
    }

    //checks to see if the player is trying to open the chest
    private void Update() {
        if (playerInProximity && Input.GetKeyDown(KeyCode.Space) && !pauseMenu.IsPaused()) {
            if (!isVisited) {
                Initialise();
                isVisited = true;
                OpenUI();
            } else if (isVisited) OpenUI();
        }
    }

    //sets this as the active chest for the UI controllers
    private void setActiveShop() {
        for (int i = 0; i < slots.Length; i++) slots[i].setShop(this);
    }

    //called the first time a chest is opened
    private void Initialise() {
        items = new List<Item>();
        setActiveShop();
        GenerateLoot();
    }

    //detects whether the player is close enough to interact with the chest
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = true;
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.isTrigger) playerInProximity = false;
    }

    //generates loot of the chest
    private void GenerateLoot() {
        int stock = Random.Range((int)(slots.Length / 2), slots.Length);
        for(int i = 0; i < stock; i++) {
            int stockChoice;
            float poolChoice = Random.Range(0f, 1f);
            if (poolChoice <= commonStock.frequency) {
                stockChoice = Random.Range(0, commonStock.itemPool.Length);
                AddItem(commonStock.itemPool[stockChoice]);
            } else if (poolChoice <= uncommonStock.frequency) {
                stockChoice = Random.Range(0, uncommonStock.itemPool.Length);
                AddItem(uncommonStock.itemPool[stockChoice]);
            } else if (poolChoice <= rareStock.frequency) {
                stockChoice = Random.Range(0, rareStock.itemPool.Length);
                AddItem(rareStock.itemPool[stockChoice]);
            } else if (poolChoice <= epicStock.frequency) {
                stockChoice = Random.Range(0, epicStock.itemPool.Length);
                AddItem(epicStock.itemPool[stockChoice]);
            }
        }
        UpdateUI();
    }

    //opens the shop UI
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
        playerMoney.text = "You have: " + inventory.getGold() + "g";
        for (int i = 0; i < slots.Length; i++) {
            if (i < items.Count) slots[i].AddItem(items[i]);
            else slots[i].EmptySlot();
        }
    }
}
