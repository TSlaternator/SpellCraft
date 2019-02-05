using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MobRoomController : MonoBehaviour, IRoomTypeController{

    public LevelGenerator generator;

    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {

    }

    public void OnPlayerEnter() {
        Debug.Log("Player Entering Mob Room");
    }

    public void OnPlayerExit() {
        Debug.Log("Player Exiting Mob Room");
    }

    public void OnRoomComplete() {

    }

    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.mobRoom.walls;
    }

    public Tile[] getTiles() {
        return generator.mobRoom.floorTiles;
    }
}
