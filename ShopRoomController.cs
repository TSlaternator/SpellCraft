using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShopRoomController : MonoBehaviour, IRoomTypeController {

    public LevelGenerator generator;

    public void SpawnRoom(float xCentre, float zCentre, int width, int height) {
        Instantiate(generator.shopRoom.shop, new Vector3(xCentre - (width / 2) + 1.5f, 0f, zCentre + (height / 2) - 3f), Quaternion.identity, transform);
    }

    public void OnPlayerEnter() {
        Debug.Log("Player Entering Shop Room");
    }

    public void OnPlayerExit() {
        Debug.Log("Player Exiting Shop Room");
    }

    public void OnRoomComplete() {

    }

    public GameObject[] getWalls() {
        generator = GameObject.Find("LevelManager").GetComponent<LevelGenerator>();
        return generator.shopRoom.walls;
    }

    public Tile[] getTiles() {
        return generator.shopRoom.floorTiles;
    }
}
