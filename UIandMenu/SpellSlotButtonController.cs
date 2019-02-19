using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotButtonController : MonoBehaviour
{
    /* Controls the effects of spell slot buttons in the crafting menu */

    [SerializeField] private SpellCraftMenu menu; //reference to the spell crafting menu
    [SerializeField] private int spellSlot; //which spell slot this button is for
    [SerializeField] private Image[] spellSlotImages; //used to change the sprites
    [SerializeField] private Image thisImage; //the image for this slot
    [SerializeField] private Sprite activeSprite; //used when this spell slot is being used
    [SerializeField] private Sprite inactiveSprite; //used when this spell slot is not being used

    //loads the spell from this spell slot
    public void PressButton() {
        for(int i = 0; i < spellSlotImages.Length; i++) {
            spellSlotImages[i].sprite = inactiveSprite;
        }
        thisImage.sprite = activeSprite;
        menu.LoadSpell(spellSlot);
    }
}
