using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShrineUIController : MonoBehaviour
{
    [SerializeField] private PlayerStatController playerStats; //holds the players stats
    [SerializeField] private PauseMenu pauseMenu; //holds the pause menu script
    [SerializeField] private Text title; //holds the title of the shrine
    [SerializeField] private Text description; //holds the description of the shrine
    [SerializeField] private Text offer; //holds the offer of the shrine
    [SerializeField] private Text sacrificeOption; //holds what the player must sacrifice
    [SerializeField] private Image sacrificeIcon; //holds what the player will receive
    [SerializeField] private Shrine shrine; //current shrine being interacted with
    private ShrineChoice currentChoice; //current choice being interacted with
    private ShrineController shrineController; //the shrineController of the currrent shrine 

    //updates the UI with the details provided
    public void UpdateUI(Shrine shrine, ShrineController controller) {
        this.shrine = shrine;
        shrineController = controller;
        currentChoice = shrine.choices[shrine.activeChoice];
        title.text = shrine.name;
        description.text = shrine.description;
        offer.text = currentChoice.description;
        sacrificeOption.text = "Sacrifice " + currentChoice.sacrificeAmount;
        sacrificeIcon.sprite = currentChoice.sacrificeSprite;
    }

    //what happens when the player makes a sacrifice
    public void Sacrifice() {
        if (playerStats.getStat(currentChoice.sacrifice) >= currentChoice.sacrificeAmount) {
            playerStats.BuffPlayer(currentChoice.sacrifice, currentChoice.sacrificeAmount * -1);
            playerStats.BuffPlayer(currentChoice.reward, currentChoice.rewardAmount);
            shrineController.UseShrine();
            LeaveShrine();
        } else {
            offer.text = "You don't have enough " + currentChoice.sacrifice + ", come back later...";
        }
    }

    //what happens when the player leaves the shrine
    public void LeaveShrine() {
        pauseMenu.Resume();
    }
}
