using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabController : MonoBehaviour {

	/* Controls the tab buttons on the crafting menu */

	[SerializeField] private GameObject thisTab; //the current tab
	[SerializeField] private GameObject nextTab; //the tab this button switches to

	//switches the tabs, turning this tab off, and the next tab on
	public void SwitchTabs(){
		thisTab.SetActive (false);
		nextTab.SetActive (true);
	}
}
