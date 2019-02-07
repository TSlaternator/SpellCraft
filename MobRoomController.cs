﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MobRoomController : MonoBehaviour, IRoomTypeController{

    private LevelGenerator generator; //the level generator script
    private TileMapController tileController; //the tile map controller script
    private bool explored = false; //will turn true once the room has been entered
    private float xCentre, zCentre; //center of the room
    private int width, height; //dimensions of the room

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
        }
    }

    //controls what happens when the player exits the room
    public void OnPlayerExit() {
        Debug.Log("Player Exiting Mob Room");
    }

    //controls what happens when the room is 'completed' (all enemies/boss killed)
    public void OnRoomComplete() {

    }

    //gets the wall components for this room type
    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.mobRoom.walls;
    }

    //gets the floor tile for this room type
    public Tile[] getTiles() {
        return generator.mobRoom.floorTiles;
    }

    //Returns the chance of wall decorations spawning
    public float getWallDecorationFrequency() {
        return generator.mobRoom.wallDecorationFrequency;
    }

    //Returns the wall decorations for this room
    public GameObject[] getWallDecorations() {
        return generator.mobRoom.wallDecorations;
    }

    //Returns the chances of each wall decoration spawning
    public float[] getWallDecorationChances() {
        return generator.mobRoom.wallDecorationChances;
    }
}
