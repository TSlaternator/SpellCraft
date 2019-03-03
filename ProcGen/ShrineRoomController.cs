using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShrineRoomController : MonoBehaviour, IRoomTypeController {

    private LevelGenerator generator; //the level generator script
    private TileMapController tileController; //the tile map controller script
    private RoomController roomController; //room controller of the room
    private bool explored = false; //will turn true once the room has been entered
    private float xCentre, zCentre; //center of the room
    private int width, height; //dimensions of the room
    private float carpetChance = 0f; //chances of spawning a carpet
    private float borderChance = 0.7f; //chances of spawning a border
    private float pillarChance = 0f; //chances of spawning a pillar at applicable points
    private float obstructionChance = 0.3f; //chances of spawning an obstruction at applicable points

    //called when the room is first spawned
    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {
        this.xCentre = xCentre;
        this.zCentre = zCentre;
        this.width = width;
        this.height = height;
        Instantiate(generator.shrineRoom.interactable, new Vector3(xCentre - 0.5f, 0f, zCentre - 0.5f), Quaternion.identity, transform);
    }

    //controls what happens when the player enters the room
    public void OnPlayerEnter() {
        //if being entered for the first time
        if (!explored) {
            explored = true;
            tileController = GameObject.Find("LevelManager").GetComponent<TileMapController>();
            tileController.RemoveFog(xCentre, zCentre, width, height);
            roomController = gameObject.GetComponent<RoomController>();
            roomController.SpawnPassiveMobs(generator.shrineRoom.mobs[0]);
            GetComponent<RoomController>().AddToMiniMap();
        }
    }

    //controls what happens when the player exits the room
    public void OnPlayerExit() {
        Debug.Log("Player Exiting Shrine Room");
    }

    //controls what happens when the room is 'completed' (all enemies/boss killed)
    public void OnRoomComplete() {

    }

    //gets the wall components for this room type
    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.shrineRoom.walls;
    }

    //gets the floor tile for this room type
    public Tile[] getTiles() {
        return generator.shrineRoom.floorTiles;
    }

    //Returns the chance of wall decorations spawning
    public float getWallDecorationFrequency() {
        return generator.shrineRoom.wallDecorationFrequency;
    }

    //Returns the wall decorations for this room
    public GameObject[] getWallDecorations() {
        return generator.shrineRoom.wallDecorations;
    }

    //Returns the chances of each wall decoration spawning
    public float[] getWallDecorationChances() {
        return generator.shrineRoom.wallDecorationChances;
    }

    //returns the chances of spawning a carpet
    public float getCarpetChance() {
        return carpetChance;
    }

    //gets the number of possible carpets for the room
    public int getCarpetCount() {
        return -1;
    }

    //gets the tiles for a specific carpet
    public Tile[] getCarpetTiles(int carpetID) {
        return null;
    }

    //returns the chances of spawning a border
    public float getBorderChance() {
        return borderChance;
    }

    //gets the number of possible borders for the room
    public int getBorderCount() {
        return generator.shrineRoom.borders.Length;
    }

    //gets the tile for a specific border
    public Tile[] getBorderTiles(int borderID) {
        return generator.shrineRoom.borders[borderID].tiles;
    }

    //gets the chance to spawn a pillar at applicable points
    public float getPillarChance() {
        return pillarChance;
    }

    //gets the chance to spawn an obstruction at applicable points
    public float getObstructionChance() {
        return obstructionChance;
    }

    //gets the chances of the rooms obstructions spawning
    public float[] getObstructionChances() {
        return generator.shrineRoom.obstructionChances;
    }

    //gets all obstructions associated with the room
    public GameObject[] getObstructions() {
        return generator.shrineRoom.obstructions;
    }

    //Returns the chance of breakable objects (barrels, crates) spawning
    public float getBreakablesFrequency() {
        return generator.shrineRoom.breakableFrequency;
    }

    //Returns the breakable objects for this room type
    public GameObject[] getBreakables() {
        return generator.shrineRoom.breakables;
    }

    //Returns the chances of each breakable object spawning
    public float[] getBreakablesChances() {
        return generator.shrineRoom.breakablesChances;
    }

    //Returns the minimap floor object for the room
    public GameObject getMinimapFloor() {
        return generator.shrineRoom.minimapFloor;
    }
}
