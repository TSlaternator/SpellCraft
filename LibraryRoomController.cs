using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LibraryRoomController : MonoBehaviour, IRoomTypeController {

    public LevelGenerator generator;

    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {
        Instantiate(generator.libraryRoom.scribingTable, new Vector3(xCentre, 0f, zCentre), Quaternion.identity, transform);
    }

    public void OnPlayerEnter() {

    }

    public void OnPlayerExit() {

    }

    public void OnRoomComplete() {

    }

    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.libraryRoom.walls;
    }

    public Tile[] getTiles() {
        return generator.libraryRoom.floorTiles;
    }
}
