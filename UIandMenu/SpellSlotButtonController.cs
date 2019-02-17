using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlotButtonController : MonoBehaviour
{
    /* Controls the effects of spell slot buttons in the crafting menu */

    [SerializeField] private SpellCraftMenu menu; //reference to the spell crafting menu
    [SerializeField] private int spellSlot; //which spell slot this button is for

    //loads the spell from this spell slot
    public void PressButton() {
        menu.LoadSpell(spellSlot);
    }
}
