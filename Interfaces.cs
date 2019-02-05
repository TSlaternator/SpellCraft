using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public interface IEnemySpellController{

	/* Interface for all enemy spell controllers */

	//controls the enemies shooting attack
	void Shoot();

	//controls enfeebling effect of spells
	void Enfeeble();

}

public interface IRoomTypeController {

    /* Interface to control room interactions */

    //spawns the room (with it's floor / wall types)
    void SpawnRoom(float xCentre, float zCentre, int width, int height);

    //what to do when the player enters the room
    void OnPlayerEnter();

    //what to do when the player exits the room
    void OnPlayerExit();

    //what to do when the player 'completes' the room
    void OnRoomComplete();

    //Returns the walls used in the room
    GameObject[] getWalls();

    //Returns the floor used in the room
    Tile[] getTiles();
}

