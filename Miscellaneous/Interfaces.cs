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

    //Returns the chance of breakable objects (barrels, crates) spawning
    float getBreakablesFrequency();

    //Returns the breakable objects for this room type
    GameObject[] getBreakables();

    //Returns the chances of each breakable object spawning
    float[] getBreakablesChances();

    //Returns the floor type of the minimap version of the room
    GameObject getMinimapFloor();
}

public interface IEffectRune {

    /* Interface to control Spell Effect Runes */

    //applies the spell effect to the supplied game object
    void ApplyEffect(GameObject enemy, SpellEffectController controller);

    //Increases the potency of the effect
    void IncreasePotency();
}

public interface IScrollController {

    /* Interface to control scrolls */

    //what to do when the scroll is used
    void OnCast();
}

public interface IEnemyMoveController {

    /* Controls enemy movement when in range of the player */

    //gets a destination for the enemy
    Vector3 getDestination(Vector3 currentPosition, Vector3 playerPosition, float range);
}

public interface IEnemyAvoidanceController {

    /* Controls enemy avoidance when hit by a projectile */

    //gives the enemy a reference to the room its in
    void SetRoom(RoomController room);

    //what to do when the enemy is hit
    void OnHit();

    //returns the chance to use avoidance
    float AvoidanceChance();

    //returns true if currently avoiding
    bool IsAvoiding();
}

