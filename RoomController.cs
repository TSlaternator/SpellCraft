using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject floorPlane; //not visible in game, used to spawn a navmesh
    private bool[] doors; //which directions have doors (0 = N, 1 = E,  2 = S, 3 = W)
    private int[] doorCentres; //position of the door on direction corresponding to doors[]
    private float xCentre, zCentre; //centre point of the room
    private int width, height; //dimensions of the room
    private int size; //size of the room
    private int roomType = -1; //type of room (used for visual look)
    private float threat, loot, essence; //variables used to determine room type
    private TileMapController tileGenerator; //allows spawning of floor tiles
    private IRoomTypeController roomTypeController; //allows different room types to have different designs
    private BoxCollider roomBounds; //allows detection of the player entering or leaving the room

    //spawns the physical room
    public void SpawnRoom() {

        //spawning invisible plane, used to create a navmesh to help control enemy AI
        GameObject floor = Instantiate(floorPlane, new Vector3(xCentre - 0.5f, -0.01f, zCentre - 0.5f), Quaternion.identity, gameObject.transform);
        floor.transform.localScale = new Vector3((float)width / 10f, 0f, (float)height / 10f);

        //set collider bounds
        roomBounds = gameObject.GetComponent<BoxCollider>();
        roomBounds.size = new Vector3(width + 2, 2f, height + 2);

        //sets up the roomTypeController
        switch (roomType) {
            case 0: roomTypeController = gameObject.AddComponent<SpawnRoomController>();
                Debug.Log("Spawn Room");
                GameObject.Find("Player").transform.position = new Vector3(xCentre, transform.position.y, zCentre);
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
        tileGenerator.DrawFog(xCentre, zCentre, width, height);
        roomTypeController.SpawnRoom(xCentre, zCentre, width, height);
        SpawnTileDecorations();
    }

    //allows the room to do something when the player enters
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") roomTypeController.OnPlayerEnter();
    }

    //allows the room to do something when the player exits
    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") roomTypeController.OnPlayerExit();
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
    public bool setDoor(int index, int position) {
        bool overwrite = false;
        if (doors[index]) overwrite = true;
        else {
            doors[index] = true;
            doorCentres[index] = position;
        }
        return overwrite;
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
            SpawnDecorations((int)(xCentre - 0.5f - (width / 2f)), (int)(zCentre + (height / 2f) - 1), width);
        } else {
            if (doorCentres[0] - (xCentre - width / 2) > 1) {
                firstCorner = xCentre - width / 2;
                wallSize = doorCentres[0] - 1 - firstCorner;
                spawnPos = new Vector3(firstCorner + wallSize / 2f - 0.5f, 1, zCentre + height / 2);
                thisWall = Instantiate(walls[0], spawnPos, Quaternion.identity, gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(wallSize, 1);
                thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(wallSize, 1);
                SpawnDecorations((int)(firstCorner - 0.5f), (int)(zCentre + (height / 2f) - 1), (int)wallSize);
            }
            if ((xCentre + width / 2) - doorCentres[0] > 1) {
                secondCorner = xCentre + width / 2;
                wallSize = secondCorner - doorCentres[0] - 1;
                spawnPos = new Vector3(secondCorner - wallSize / 2 - 0.5f, 1, zCentre + height / 2);
                thisWall = Instantiate(walls[0], spawnPos, Quaternion.identity, gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(wallSize, 1);
                thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(wallSize, 1);
                SpawnDecorations((int)(secondCorner - 0.5f - wallSize), (int)(zCentre + (height / 2f) - 1), (int)wallSize);
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
        Instantiate(walls[3], spawnPos, Quaternion.identity, gameObject.transform);
        spawnPos = new Vector3(xCentre - 1f - width / 2, 1, zCentre + height / 2);
        Instantiate(walls[3], spawnPos, Quaternion.identity * Quaternion.Euler(0f, 270f, 0f), gameObject.transform);
        spawnPos = new Vector3(xCentre - 1f - width / 2, 1, zCentre - 1f - height / 2);
        Instantiate(walls[3], spawnPos, Quaternion.identity * Quaternion.Euler(0f, 180f, 0f), gameObject.transform);
        spawnPos = new Vector3(xCentre + width / 2, 1, zCentre - 1f - height / 2);
        Instantiate(walls[3], spawnPos, Quaternion.identity * Quaternion.Euler(0f, 90f, 0f), gameObject.transform);
    }

    //Spawns decorations on the walls
    private void SpawnDecorations(int xLeft, int zPos, int width) {
        for (int i = 1; i <= width; i += 2) {
            if (Random.Range(0f, 1f) < roomTypeController.getWallDecorationFrequency()) {
                float decorationToSpawn = Random.Range(0f, 1f);
                float[] chances = roomTypeController.getWallDecorationChances();
                GameObject[] decorations = roomTypeController.getWallDecorations();
                for (int j = 0; j < decorations.Length; j++) {
                    if (decorationToSpawn <= chances[j]) {
                        Instantiate(decorations[j], new Vector3(xLeft + i, 1f, zPos), Quaternion.identity, transform);
                        j = decorations.Length;
                    }
                }
            }
        }
    }

    //Spawns decorated tiles on the floor
    private void SpawnTileDecorations() {
        float decorationChance = Random.Range(0f, 1f);
        int decorationChoice;
        if (decorationChance < roomTypeController.getCarpetChance()) {
            decorationChoice = Random.Range(0, roomTypeController.getCarpetCount());
            tileGenerator.DrawCarpet(xCentre, zCentre, width, height, roomTypeController.getCarpetTiles(decorationChoice));
        } else if (decorationChance < roomTypeController.getBorderChance()) {
            decorationChoice = Random.Range(0, roomTypeController.getBorderCount());
            tileGenerator.DrawBorder(xCentre, zCentre, width, height, roomTypeController.getBorderTiles(decorationChoice));
        }
    }
}
