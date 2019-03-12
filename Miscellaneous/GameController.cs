using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    /* Controls overarching game effects */

    [SerializeField] private Texture2D cursorSprite; //new sprite to use as the in game cursor
    [SerializeField] private GameObject chestUI; //UI for chests
    [SerializeField] private GameObject shopUI; //UI for the shop
    [SerializeField] private PauseMenu pauseMenu; //Pausemenu controller scripts
    [SerializeField] private ChestSlotController[] chestSlots; //chest slots on the UI
    [SerializeField] private ShopSlotController[] shopSlots; //shop slots in the UI
    [SerializeField] private PlayerInventoryController inventory; //controls the players inventory
    [SerializeField] private string[] shopKeeperPurchaseQuotes; //messages for when a successful purchase is made
    [SerializeField] private string[] shopKeeperInadequateFundsQuotes; //messages for when the player tries to buy something they can't afford
    [SerializeField] private string[] shopKeeperBagFullQuotes; //messages for when the player tries to buy something, with a full bag
    [SerializeField] private Text playerMoneyText; //displays the players money in the shop UI
    [SerializeField] private Shrine[] shrineTypes; //holds the types of shrine that can appear in the game
    [SerializeField] private GameObject shrineUI; //holds the UI for shrines
    [SerializeField] private GameObject bossHealthBar; //holds the UI for the bosses healthbar
    [SerializeField] private Image bossHealth; //used in the healthbar
    [SerializeField] private Image bossDamagedHealth; //used in the healthbar

    //sets the cursor to the custom one
    void Start() {
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }

    //gets the chest UI object
    public GameObject getChestUI() {
        return chestUI;
    }

    //gets the shop UI object
    public GameObject getShopUI() {
        return shopUI;
    }

    //gets the chest slots
    public ChestSlotController[] getChestSlots() {
        return chestSlots;
    }

    //gets the shop slots
    public ShopSlotController[] getShopSlots() {
        return shopSlots;
    }

    //gets the pause menu
    public PauseMenu getPauseMenu() {
        return pauseMenu;
    }

    //gets the players inventory controller
    public PlayerInventoryController getPlayerInventory() {
        return inventory;
    }

    //gets a quote from the shopkeeper
    public string getShopKeeperQuote(int type) {
        if (type == 0) {
            int quote = Random.Range(0, shopKeeperPurchaseQuotes.Length);
            return shopKeeperPurchaseQuotes[quote];
        } else if (type == 1) {
            int quote = Random.Range(0, shopKeeperBagFullQuotes.Length);
            return shopKeeperBagFullQuotes[quote];
        } else {
            int quote = Random.Range(0, shopKeeperInadequateFundsQuotes.Length);
            return shopKeeperInadequateFundsQuotes[quote];
        }
    }

    //gets the UI element to display player money in the shop
    public Text getPlayerMoneyText() {
        return playerMoneyText;
    }

    //gets a type for a shrine
    public Shrine getShrineType() {
        return (shrineTypes[Random.Range(0, shrineTypes.Length)]);
    }

    //gets the shrineUI
    public GameObject getShrineUI() {
        return shrineUI;
    }

    //gets the health bar UI object
    public GameObject getHealthBar() {
        return bossHealthBar;
    }

    //gets the health image of the health UI object
    public Image getHealthUI() {
        return bossHealth;
    }

    //gets the damaged health image of the health UI object
    public Image getDamagedHealthUI() {
        return bossDamagedHealth;
    }
}
