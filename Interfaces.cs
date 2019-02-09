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

    //Returns the chance of wall decorations spawning
    float getWallDecorationFrequency();

    //Returns the wall decorations for this room
    GameObject[] getWallDecorations();

    //Returns the chances of each wall decoration spawning
    float[] getWallDecorationChances();

    //returns the chances of spawning a carpet
    float getCarpetChance();

    //gets the number of possible carpets for the room
    int getCarpetCount();

    //gets the tiles for a specific carpet
    Tile[] getCarpetTiles(int carpetID);

    //returns the chances of spawning a border
    float getBorderChance();

    //gets the number of possible borders for the room
    int getBorderCount();

    //gets the tile for a specific border
    Tile[] getBorderTiles(int borderID);

    //gets the chance to spawn a pillar at applicable points
    float getPillarChance();

    //gets the chance to spawn an obstruction at applicable points
    float getObstructionChance();

    //gets the chances of the rooms obstructions spawning
    float[] getObstructionChances();

    //gets all obstructions associated with the room
    GameObject[] getObstructions();
}

