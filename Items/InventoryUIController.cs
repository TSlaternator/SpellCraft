using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private Transform inventorySlots; //holds all inventory slots
    [SerializeField] private PlayerInventoryController inventory; //controls player inventory functions
    [SerializeField] private GameObject inventoryUI; //UI of the inventory
    [SerializeField] private GameObject HUDUI; //UI of the HUD
    [SerializeField] private PauseMenu pauseMenu; //pauseMenu script
    [SerializeField] private Texture2D cursorSprite; //sprite of the cursor 
    private ItemSlotController[] slots; //list of the slot controller scripts
    private bool isOpen = false; //whether the inventory is open or not

    //initialising 
    void Start() {
        slots = inventorySlots.GetComponentsInChildren<ItemSlotController>();
    }

    //waits for the inventory key
    void Update() {
        if (Input.GetButtonDown("KeyboardI") && !pauseMenu.IsPaused()) {
            if (!isOpen) OpenInventory();
        }
    }

    //Updates the UI to show current items
    public void UpdateUI(List<Item> items) {
        for(int i = 0; i < slots.Length; i++) {
            if (i < items.Count) slots[i].AddItem(items[i]);
            else slots[i].EmptySlot();
        }
    }

    //opens the inventory UI
    public void OpenInventory() {
        pauseMenu.setPaused(true);
        isOpen = true;
        inventoryUI.SetActive(true);
        HUDUI.SetActive(false);
        Time.timeScale = 0f;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    //closes the inventory UI
    public void CloseInventory() {
        pauseMenu.setPaused(false);
        isOpen = false;
        inventoryUI.SetActive(false);
        HUDUI.SetActive(true);
        Time.timeScale = 1f;
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }
}
