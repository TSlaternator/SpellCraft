using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject northWall; //wall at the top of the room
    [SerializeField] private GameObject sideWalls; //walls on the left and right
    [SerializeField] private GameObject southWall; //wall at the bottom of the room
    [SerializeField] private GameObject cornerWall; //walls at the corners of the room
    private bool[] doors; //which directions have doors (0 = N, 1 = E,  2 = S, 3 = W)
    private int[] doorCentres; //position of the door on direction corresponding to doors[]
    private float xCentre, zCentre; //centre point of the room
    private int width, height; //dimensions of the room
    private int size; //size of the room
    private int roomType = -1; //type of room (used for visual look)
    private float threat, loot, essence; //variables used to determine room type
    private TileMapController tileGenerator; //allows spawning of floor tiles
    private IRoomTypeController roomTypeController; //allows different room types to have different designs

    //spawns the physical room
    public void SpawnRoom() {

        //GameObject.Find("Player").GetComponent<PlayerController>().setStartPosition(new Vector3(xCentre, 0f, zCentre));

        //sets up the roomTypeController
        switch (roomType) {
            case 0: roomTypeController = gameObject.AddComponent<SpawnRoomController>();
                    GameObject.Find("Player").transform.position = new Vector3(xCentre, 0.5f, zCentre);
                    GameObject.Find("CameraPosition").transform.position = new Vector3(xCentre, 10f, zCentre - 10f);
                    break;
            case 1: roomTypeController = gameObject.AddComponent<BossRoomController>(); break;
            case 2: roomTypeController = gameObject.AddComponent<ShopRoomController>(); break;
            case 3: roomTypeController = gameObject.AddComponent<LibraryRoomController>(); break;
            case 4: roomTypeController = gameObject.AddComponent<ShrineRoomController>(); break;
            case 5: roomTypeController = gameObject.AddComponent<MobRoomController>(); break;
            case 6: roomTypeController = gameObject.AddComponent<LootRoomController>(); break;
            case 7: roomTypeController = gameObject.AddComponent<GenericRoomController>(); break;
        }
        //Generating floors and walls
        SpawnWalls(roomTypeController.getWalls());
        tileGenerator.DrawRoom(xCentre, zCentre, width, height, roomTypeController.getTiles());
        roomTypeController.SpawnRoom(xCentre, zCentre, width, height);
    }

    //sets up the door variables
    public void SetDoors() {
        doors = new bool[] { false, false, false, false };
        doorCentres = new int[] { 0, 0, 0, 0 };
    }

    //sets the type of the room
    public void setRoomType(int type) {
        roomType = type;
    }

    //randomly generates a type for the room
    public void setRoomType() {
        //generating variables
        threat = Random.Range(0f, 1f);
        loot = Random.Range(0f, 1f);
        essence = Random.Range(0f, 1f);

        //if essence >= 0.9 room becomes a shrine room
        if (essence >= 0.9f) roomType = 4;
        //if threat > loot * 1.5 room becomes a mob room
        else if (threat > loot * 1.5f && threat >= 0.5f) roomType = 5;
        //if loot > threat * 1.5 room becomes a chest room
        else if (loot > threat * 1.5f && loot >= 0.5f) roomType = 6;
        //else, room is generic
        else roomType = 7;
    }

    //gets the type of the room
    public int getRoomType() {
        return roomType;
    }

    //gets the size of the room
    public int getSize() {
        return size;
    }

    //sets the dimensions of the coridoor
    public void setVariables(int width, int height) {
        this.width = width;
        this.height = height;
        xCentre = gameObject.transform.position.x;
        zCentre = gameObject.transform.position.z;
        size = width * height;
        tileGenerator = GameObject.Find("LevelManager").GetComponent<TileMapController>();
    }

    //lets the room know where a door will be
    public void setDoor(int index, int position) {
        doors[index] = true;
        doorCentres[index] = position;
    }

    //spawns the walls of the room, leaving gaps for any doors
    public void SpawnWalls(GameObject[] walls) {
        Vector3 spawnPos;
        GameObject thisWall;
        Transform wallTexture;
        float firstCorner, secondCorner, wallSize;
        //north wall
        if (!doors[0]) {
            spawnPos = new Vector3(xCentre - 0.5f, 1, zCentre + height / 2);
            thisWall = Instantiate(walls[0], spawnPos, Quaternion.identity, gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * width, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(width, 1);
            thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(width, 1);
        } else {
            if (doorCentres[0] - (xCentre - width / 2) > 1) {
                firstCorner = xCentre - width / 2;
                wallSize = doorCentres[0] - 1 - firstCorner;
                spawnPos = new Vector3(firstCorner + wallSize / 2f - 0.5f, 1, zCentre + height / 2);
                thisWall = Instantiate(walls[0], spawnPos, Quaternion.identity, gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(wallSize, 1);
                thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(wallSize, 1);
            }
            if ((xCentre + width / 2) - doorCentres[0] > 1) {
                secondCorner = xCentre + width / 2;
                wallSize = secondCorner - doorCentres[0] - 1;
                spawnPos = new Vector3(secondCorner - wallSize / 2 - 0.5f, 1, zCentre + height / 2);
                thisWall = Instantiate(walls[0], spawnPos, Quaternion.identity, gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(wallSize, 1);
                thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(wallSize, 1);
            }
        }
        //east wall
        if (!doors[1]) {
            spawnPos = new Vector3(xCentre - 1f - width / 2, 1, zCentre - 0.5f);
            thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * height, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(height, 1);
        } else {
            if (doorCentres[1] - (zCentre - height / 2) > 1) {
                firstCorner = zCentre - height / 2;
                wallSize = doorCentres[1] - 1 - firstCorner;
                spawnPos = new Vector3(xCentre - 1f - width / 2, 1, firstCorner - 0.5f + wallSize / 2f);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
            }
            if ((zCentre + height / 2) - doorCentres[1] > 1) {
                secondCorner = zCentre + height / 2;
                wallSize = secondCorner - doorCentres[1] - 1;
                spawnPos = new Vector3(xCentre - 1f - width / 2, 1, secondCorner - 0.5f - wallSize / 2);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
            }
        }
        //west wall
        if (!doors[3]) {
            spawnPos = new Vector3(xCentre + width / 2, 1, zCentre - 0.5f);
            thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * height, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(height, 1);
        } else {
            if (doorCentres[3] - (zCentre - height / 2) > 1) {
                firstCorner = zCentre - height / 2;
                wallSize = doorCentres[3] - 1 - firstCorner;
                spawnPos = new Vector3(xCentre + width / 2, 1, firstCorner - 0.5f + wallSize / 2f);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
            }
            if ((zCentre + height / 2) - doorCentres[3] > 1) {
                secondCorner = zCentre + height / 2;
                wallSize = secondCorner - doorCentres[3] - 1;
                spawnPos = new Vector3(xCentre + width / 2, 1, secondCorner - 0.5f - wallSize / 2);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
            }
        }
        //south wall
        if (!doors[2]) {
            spawnPos = new Vector3(xCentre - 0.5f, 1, zCentre - 1f - height / 2);
            thisWall = Instantiate(walls[2], spawnPos, Quaternion.identity, gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * width, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(width + 2, 1);
            thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(width, 1);
            wallTexture = thisWall.GetComponentsInChildren<Transform>()[1];
            wallTexture.localScale = new Vector3((thisWall.transform.localScale.x * (width + 2)) / (thisWall.transform.localScale.x * (width)) / 10, 1f, 0.1f);
        } else {
            if (doorCentres[2] - (xCentre - width / 2) > 1) {
                firstCorner = xCentre - width / 2;
                wallSize = doorCentres[2] - 1 - firstCorner;
                spawnPos = new Vector3(firstCorner + wallSize / 2f - 0.5f, 1, zCentre - 1f - height / 2);
                thisWall = Instantiate(walls[2], spawnPos, Quaternion.identity, gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(wallSize + 1, 1);
                thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(wallSize, 1);
                wallTexture = thisWall.GetComponentsInChildren<Transform>()[1];
                wallTexture.localScale = new Vector3((thisWall.transform.localScale.x * (wallSize + 1)) / (thisWall.transform.localScale.x * (wallSize)) / 10, 1f, 0.1f);
                wallTexture.position = new Vector3(wallTexture.position.x - 0.5f, wallTexture.position.y, wallTexture.position.z);
            }
            if ((xCentre + width / 2) - doorCentres[2] > 1) {
                secondCorner = xCentre + width / 2;
                wallSize = secondCorner - doorCentres[2] - 1;
                spawnPos = new Vector3(secondCorner - wallSize / 2 - 0.5f, 1, zCentre - 1f - height / 2);
                thisWall = Instantiate(walls[2], spawnPos, Quaternion.identity, gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(wallSize + 1, 1);
                thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(wallSize, 1);
                wallTexture = thisWall.GetComponentsInChildren<Transform>()[1];
                wallTexture.localScale = new Vector3((thisWall.transform.localScale.x * (wallSize + 1)) / (thisWall.transform.localScale.x * (wallSize)) / 10, 1f, 0.1f);
                wallTexture.position = new Vector3(wallTexture.position.x + 0.5f, wallTexture.position.y, wallTexture.position.z);
            }
        }
        //corners
        spawnPos = new Vector3(xCentre + width / 2, 1, zCentre + height / 2);
        Instantiate(cornerWall, spawnPos, Quaternion.identity, gameObject.transform);
        spawnPos = new Vector3(xCentre - 1f - width / 2, 1, zCentre + height / 2);
        Instantiate(cornerWall, spawnPos, Quaternion.identity, gameObject.transform);
        spawnPos = new Vector3(xCentre - 1f - width / 2, 1, zCentre - 1f - height / 2);
        Instantiate(cornerWall, spawnPos, Quaternion.identity, gameObject.transform);
        spawnPos = new Vector3(xCentre + width / 2, 1, zCentre - 1f - height / 2);
        Instantiate(cornerWall, spawnPos, Quaternion.identity, gameObject.transform);
    }
}
