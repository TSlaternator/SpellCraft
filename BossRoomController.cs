using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossRoomController : MonoBehaviour, IRoomTypeController {

    public LevelGenerator generator;

    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {
        //do nothing
    }

    public void OnPlayerEnter() {
        Debug.Log("Player Entering Boss Room");
    }

    public void OnPlayerExit() {
        Debug.Log("Player Exiting Boss Room");
    }

    public void OnRoomComplete() {

    }

    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.bossRoom.walls;
    }

    public Tile[] getTiles() {
        return generator.bossRoom.floorTiles;
    }
}
