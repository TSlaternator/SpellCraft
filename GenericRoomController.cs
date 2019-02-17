using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenericRoomController : MonoBehaviour, IRoomTypeController {

    private Transform enemies; //list of enemies
    private LevelGenerator generator; //the level generator script
    private TileMapController tileController; //the tile map controller script
    private RoomController roomController; //room controller script of this room
    private bool explored = false; //will turn true once the room has been entered
    private bool complete = false; //when true, player will be rewarded for completing the room
    private float xCentre, zCentre; //center of the room
    private int width, height; //dimensions of the room
    private float carpetChance = 0.3f; //chances of spawning a carpet
    private float borderChance = 0.8f; //chances of spawning a border
    private float pillarChance = 0.4f; //chances of spawning a pillar at applicable points
    private float obstructionChance = 0.8f; //chances of spawning an obstruction at applicable points

    //checks to see if the room is complete
    void Update() {
        if (explored && !complete) {
            if (enemies.childCount == 0) OnRoomComplete();
        }
    }

    //called when the room is first spawned
    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {
        this.xCentre = xCentre;
        this.zCentre = zCentre;
        this.width = width;
        this.height = height;
    }

    //controls what happens when the player enters the room
    public void OnPlayerEnter() {
        //if being entered for the first time
        if (!explored) {
            explored = true;
            tileController = GameObject.Find("LevelManager").GetComponent<TileMapController>();
            tileController.RemoveFog(xCentre, zCentre, width, height);
            roomController = gameObject.GetComponent<RoomController>();
            roomController.SpawnMobs(generator.mobRoom.mobs, generator.mobRoom.mobThreatValues, 0);
            enemies = GameObject.Find("EnemiesList").transform;
            roomController.LockDoors();
            GetComponent<RoomController>().AddToMiniMap();
        }
    }

    //controls what happens when the player exits the room
    public void OnPlayerExit() {
        Debug.Log("Player Exiting Generic Room");
    }

    //controls what happens when the room is 'completed' (all enemies/boss killed)
    public void OnRoomComplete() {
        Debug.Log("Room Complete!");
        complete = true;
        roomController.UnlockDoors();
        roomController.DropRewards();
    }

    //gets the wall components for this room type
    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.genericRoom.walls;
    }

    //gets the floor tile for this room type
    public Tile[] getTiles() {
        return generator.genericRoom.floorTiles;
    }

    //Returns the chance of wall decorations spawning
    public float getWallDecorationFrequency() {
        return generator.genericRoom.wallDecorationFrequency;
    }

    //Returns the wall decorations for this room
    public GameObject[] getWallDecorations() {
        return generator.genericRoom.wallDecorations;
    }

    //Returns the chances of each wall decoration spawning
    public float[] getWallDecorationChances() {
        return generator.genericRoom.wallDecorationChances;
    }

    //returns the chances of spawning a carpet
    public float getCarpetChance() {
        return carpetChance;
    }

    //gets the number of possible carpets for the room
    public int getCarpetCount() {
        return generator.genericRoom.carpets.Length;
    }

    //gets the tiles for a specific carpet
    public Tile[] getCarpetTiles(int carpetID) {
        return generator.genericRoom.carpets[carpetID].tiles;
    }

    //returns the chances of spawning a border
    public float getBorderChance() {
        return borderChance;
    }

    //gets the number of possible borders for the room
    public int getBorderCount() {
        return generator.genericRoom.borders.Length;
    }

    //gets the tile for a specific border
    public Tile[] getBorderTiles(int borderID) {
        return generator.genericRoom.borders[borderID].tiles;
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
        return generator.genericRoom.obstructionChances;
    }

    //gets all obstructions associated with the room
    public GameObject[] getObstructions() {
        return generator.genericRoom.obstructions;
    }

    //Returns the chance of breakable objects (barrels, crates) spawning
    public float getBreakablesFrequency() {
        return generator.genericRoom.breakableFrequency;
    }

    //Returns the breakable objects for this room type
    public GameObject[] getBreakables() {
        return generator.genericRoom.breakables;
    }

    //Returns the chances of each breakable object spawning
    public float[] getBreakablesChances() {
        return generator.genericRoom.breakablesChances;
    }

    //Returns the minimap floor object for the room
    public GameObject getMinimapFloor() {
        return generator.genericRoom.minimapFloor;
    }
}
