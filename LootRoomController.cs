﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LootRoomController : MonoBehaviour, IRoomTypeController {

    public LevelGenerator generator;

    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {
        Instantiate(generator.lootRoom.chest, new Vector3(xCentre - 0.5f, 0f, zCentre - 0.5f), Quaternion.identity, transform);
    }

    public void OnPlayerEnter() {
        Debug.Log("Player Entering Loot Room");
    }

    public void OnPlayerExit() {
        Debug.Log("player Exiting Loot Room");
    }

    public void OnRoomComplete() {

    }

    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.lootRoom.walls;
    }

    public Tile[] getTiles() {
        return generator.lootRoom.floorTiles;
    }
}
