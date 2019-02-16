using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolTipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	/* Controls the tooltip of each rune in the spell crafting menu */

	[SerializeField] private Text textBox; //the tooltip text box
	[SerializeField] private string toolTip; //the tooltip of this rune

	//Changes the current tooltip to this runes tooltip when its being moused over
	public void OnPointerEnter(PointerEventData eventData){
		textBox.text = toolTip;
	}

	//Resets the tooltip when this rune is no longer moused over
	public void OnPointerExit(PointerEventData eventData){
		textBox.text = "";
	}
}
