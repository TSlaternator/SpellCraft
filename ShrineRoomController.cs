using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShrineRoomController : MonoBehaviour, IRoomTypeController {

    public LevelGenerator generator;

    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {
        Instantiate(generator.shrineRoom.shrine, new Vector3(xCentre, 0f, zCentre), Quaternion.identity, transform);
    }

    public void OnPlayerEnter() {

    }

    public void OnPlayerExit() {

    }

    public void OnRoomComplete() {

    }

    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.shrineRoom.walls;
    }

    public Tile[] getTiles() {
        return generator.shrineRoom.floorTiles;
    }
}
