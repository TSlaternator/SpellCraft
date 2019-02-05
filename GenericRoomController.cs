using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenericRoomController : MonoBehaviour, IRoomTypeController {

    public LevelGenerator generator;

    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {

    }

    public void OnPlayerEnter() {

    }

    public void OnPlayerExit() {

    }

    public void OnRoomComplete() {

    }

    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.genericRoom.walls;
    }

    public Tile[] getTiles() {
        return generator.genericRoom.floorTiles;
    }
}
