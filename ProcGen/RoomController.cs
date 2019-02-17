using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject floorPlane; //not visible in game, used to spawn a navmesh
    [SerializeField] private GameObject manaDrop; //mana pickup that can drop when the room is completed
    [SerializeField] private GameObject goldDrop; //gold pickup that can drop when the room is completed
    [SerializeField] private GameObject healthDrop; //health pickup that can drop when the room is completed
    private Transform enemiesList; //all enemies are spawned under this
    private bool[] doors; //which directions have doors (0 = N, 1 = E,  2 = S, 3 = W)
    private List<DoorController> doorControllers = new List<DoorController>(); //list of door controller scripts attached to the room
    private int[] doorCentres; //position of the door on direction corresponding to doors[]
    private float xCentre, zCentre; //centre point of the room
    private int width, height; //dimensions of the room
    private int size; //size of the room
    private int roomType = -1; //type of room (used for visual look)
    private float threat, loot, essence; //variables used to determine room type
    private TileMapController tileGenerator; //allows spawning of floor tiles
    private IRoomTypeController roomTypeController; //allows different room types to have different designs
    private BoxCollider roomBounds; //allows detection of the player entering or leaving the room
    private coordinates[] nodes; //nodes that can spawn an obstacle (table, pillar etc)
    private List<GameObject> minimapComponents = new List<GameObject>(); //minimap components of the room

    //spawns the physical room
    public void SpawnRoom() {

        //spawning invisible plane, used to create a navmesh to help control enemy AI
        GameObject floor = Instantiate(floorPlane, new Vector3(xCentre - 0.5f, -0.01f, zCentre - 0.5f), Quaternion.identity, gameObject.transform);
        floor.transform.localScale = new Vector3((float)width / 10f, 0f, (float)height / 10f);

        //set collider bounds
        roomBounds = gameObject.GetComponent<BoxCollider>();
        roomBounds.size = new Vector3(width - 1f, 2f, height - 1f);

        //sets up the roomTypeController
        switch (roomType) {
            case 0: roomTypeController = gameObject.AddComponent<SpawnRoomController>();
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
        tileGenerator.DrawFog(xCentre, zCentre, width, height); //Darkening rooms until they're explored
        roomTypeController.SpawnRoom(xCentre, zCentre, width, height);
        SpawnTileDecorations();

        //setting up the rooms minimap components
        GameObject MMFloor = Instantiate(roomTypeController.getMinimapFloor(), new Vector3(xCentre, 99.5f, zCentre ), Quaternion.identity, gameObject.transform);
        MMFloor.transform.localScale = new Vector3(MMFloor.transform.localScale.x * width, MMFloor.transform.localScale.y, MMFloor.transform.localScale.z * height);
        minimapComponents.Add(MMFloor);
        MMFloor.SetActive(false);

        //Generating obstacles (pillars, tables etc)
        CreateNodes();
        SpawnObstacles();
    }

    //allows the room to do something when the player enters
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.isTrigger) roomTypeController.OnPlayerEnter();
    }

    //allows the room to do something when the player exits
    void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.isTrigger) roomTypeController.OnPlayerExit();
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
    //also spawns breakable objects (barrels, crates etc) about the walls
    public void SpawnWalls(GameObject[] walls) {
        Vector3 spawnPos;
        GameObject thisWall;
        GameObject MMWall;
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
            SpawnBreakables((int)(xCentre - (width / 2)), (int)(zCentre + (height / 2) - 1), width - 2, true);
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
                SpawnBreakables((int)(firstCorner), (int)(zCentre + (height / 2) - 1), (int)wallSize - 1, true);
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
                SpawnBreakables((int)(secondCorner - wallSize - 1), (int)(zCentre + (height / 2) - 1), (int)wallSize - 1, true);
            }
        }
        //east wall
        if (!doors[1]) {
            spawnPos = new Vector3(xCentre - 1f - width / 2, 1, zCentre - 0.5f);
            thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * height, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(height, 1);
            SpawnBreakables(xCentre - (width / 2), zCentre - (height / 2) + 1, height - 3, false);
        } else {
            if (doorCentres[1] - (zCentre - height / 2) > 1) {
                firstCorner = zCentre - height / 2;
                wallSize = doorCentres[1] - 1 - firstCorner;
                spawnPos = new Vector3(xCentre - 1f - width / 2, 1, firstCorner - 0.5f + wallSize / 2f);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
                SpawnBreakables(xCentre - (width / 2), firstCorner + 1, (int)wallSize - 1, false);
            }
            if ((zCentre + height / 2) - doorCentres[1] > 1) {
                secondCorner = zCentre + height / 2;
                wallSize = secondCorner - doorCentres[1] - 1;
                spawnPos = new Vector3(xCentre - 1f - width / 2, 1, secondCorner - 0.5f - wallSize / 2);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
                SpawnBreakables(xCentre - (width / 2), secondCorner - wallSize, (int)wallSize - 2, false);
            }
        }
        //west wall
        if (!doors[3]) {
            spawnPos = new Vector3(xCentre + width / 2, 1, zCentre - 0.5f);
            thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * height, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(height, 1);
            SpawnBreakables((int)(xCentre + (width / 2) - 1), (int)(zCentre - (height / 2) + 1), height - 3, false);
        } else {
            if (doorCentres[3] - (zCentre - height / 2) > 1) {
                firstCorner = zCentre - height / 2;
                wallSize = doorCentres[3] - 1 - firstCorner;
                spawnPos = new Vector3(xCentre + width / 2, 1, firstCorner - 0.5f + wallSize / 2f);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
                SpawnBreakables((int)(xCentre + (width / 2) - 1), (int)(firstCorner + 1), (int)wallSize - 1, false);
            }
            if ((zCentre + height / 2) - doorCentres[3] > 1) {
                secondCorner = zCentre + height / 2;
                wallSize = secondCorner - doorCentres[3] - 1;
                spawnPos = new Vector3(xCentre + width / 2, 1, secondCorner - 0.5f - wallSize / 2);
                thisWall = Instantiate(walls[1], spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
                thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * wallSize, thisWall.transform.localScale.y, thisWall.transform.localScale.z);
                thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(wallSize, 1);
                SpawnBreakables((int)(xCentre + (width / 2) - 1), (int)(secondCorner - wallSize), (int)wallSize - 2, false);
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
            SpawnBreakables((int)(xCentre - (width / 2)), (int)(zCentre - (height / 2) + 1), width - 2, true);
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
                SpawnBreakables((int)(firstCorner), (int)(zCentre - (height / 2) + 1), (int)wallSize - 1, true);
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
                SpawnBreakables((int)(secondCorner - wallSize - 1), (int)(zCentre - (height / 2) + 1), (int)wallSize - 1, true);
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

    //Spawns decorations on the wall, varient used for the 3x1 walls generated by the SpawnObstacles method
    private void SpawnDecorations(float xPos, float zPos) {
        if (Random.Range(0f, 1f) * 0.8f < roomTypeController.getWallDecorationFrequency()) {
            float decorationToSpawn = Random.Range(0f, 1f);
            float[] chances = roomTypeController.getWallDecorationChances();
            GameObject[] decorations = roomTypeController.getWallDecorations();
            for (int i = 0; i < decorations.Length; i++) {
                if (decorationToSpawn <= chances[i]) {
                    Instantiate(decorations[i], new Vector3(xPos, 0.6f, zPos - 0.01f), Quaternion.identity, transform);
                    i = decorations.Length;
                }
            }
        }
    }

    //Spawns breakable objects (barrels, crates, pots) around the edges of the room
    private void SpawnBreakables(float xLeft, float zBottom, int length, bool direction) {
        for (int i = 1; i <= length; i ++) {
            if (Random.Range(0f, 1f) < roomTypeController.getBreakablesFrequency()) {
                float breakableToSpawn = Random.Range(0f, 1f);
                float[] chances = roomTypeController.getBreakablesChances();
                GameObject[] breakables = roomTypeController.getBreakables();
                for (int j = 0; j < breakables.Length; j++) {
                    if (breakableToSpawn <= chances[j]) {
                        if (direction) Instantiate(breakables[j], new Vector3(xLeft + i, 0f, zBottom), Quaternion.identity, transform); //horizontal
                        else Instantiate(breakables[j], new Vector3(xLeft, 0f, zBottom + i), Quaternion.identity, transform); //vertical
                        j = breakables.Length;
                    }
                }
            }
        } 
    }

    //Spawns decorated tiles on the floor
    private void SpawnTileDecorations() {
        float decorationChance = Random.Range(0f, 1f);
        int decorationChoice;
        if (decorationChance <= roomTypeController.getCarpetChance()) {
            decorationChoice = Random.Range(0, roomTypeController.getCarpetCount());
            tileGenerator.DrawCarpet(xCentre, zCentre, width, height, roomTypeController.getCarpetTiles(decorationChoice));
        } else if (decorationChance < roomTypeController.getBorderChance()) {
            decorationChoice = Random.Range(0, roomTypeController.getBorderCount());
            tileGenerator.DrawBorder(xCentre, zCentre, width, height, roomTypeController.getBorderTiles(decorationChoice));
        }
    }

    //Creates the grid of nodes for obstacles
    private void CreateNodes() {
        //setting up variables
        int nodeSize = 6;
        int rows = (height - 2) / nodeSize;
        int columns = (width - 2) / nodeSize;
        nodes = new coordinates[rows * columns];
        int xMultiplier, zMultiplier;
        bool evenX = (columns % 2 == 0) ? true : false;
        bool evenZ = (rows % 2 == 0) ? true : false;

        //generating node coordinates
        for (int  i = 0; i < columns; i++) {
            //getting xMultiplier
            if (evenX && i >= columns / 2) xMultiplier = i - (columns / 2) + 1;
            else xMultiplier = i - (columns / 2);
            for (int j = 0; j < rows; j++) {
                //getting zMultiplier
                if (evenZ && j >= rows / 2) zMultiplier = j - (rows / 2) + 1;
                else zMultiplier = j - (rows / 2);

                //setting up coordinates
                if (evenX) { 
                    if (i >= columns / 2) nodes[i * rows + j].x = xCentre + xMultiplier * nodeSize - 0.5f - (nodeSize / 2f);
                    else nodes[i * rows + j].x = xCentre + xMultiplier * nodeSize - 0.5f + (nodeSize / 2f);
                } else nodes[i * rows + j].x = xCentre + xMultiplier * nodeSize - 0.5f;

                if (evenZ) {
                    if (j >= rows / 2) nodes[i * rows + j].z = zCentre + zMultiplier * nodeSize - 0.5f - (nodeSize / 2f);
                    else nodes[i * rows + j].z = zCentre + zMultiplier * nodeSize - 0.5f + (nodeSize / 2f);
                } else nodes[i * rows + j].z = zCentre + zMultiplier * nodeSize - 0.5f;
            }
        }
    }

    //Procedurally spawns obstacles at the node positions
    private void SpawnObstacles() {
        for (int i = 0; i < nodes.Length; i++) {
            if (nodes[i].x != xCentre - 0.5f || nodes[i].z != zCentre - 0.5f) {
                float obstacleChance = Random.Range(0f, 1f);
                if (obstacleChance < roomTypeController.getPillarChance()) {
                    SpawnPillar(nodes[i].x, nodes[i].z, roomTypeController.getWalls());
                    SpawnDecorations(nodes[i].x, nodes[i].z - 1f);
                } else if (obstacleChance < roomTypeController.getObstructionChance()) {
                    float obstructionToSpawn = Random.Range(0f, 1f);
                    float[] chances = roomTypeController.getObstructionChances();
                    GameObject[] obstructions = roomTypeController.getObstructions();
                    for (int j = 0; j < obstructions.Length; j++) {
                        if (obstructionToSpawn <= chances[j]) {
                            Instantiate(obstructions[j], new Vector3(nodes[i].x, 0f, nodes[i].z + 0.5f), Quaternion.identity, transform);
                            j = obstructions.Length;
                        }
                    }
                }
            }
        }
    }

    //spawns a pillar about the specified position, with the room types walls
    private void SpawnPillar(float xPos, float zPos, GameObject[] walls) {
        GameObject wall;
        wall = Instantiate(walls[0], new Vector3(xPos, 1, zPos - 0.5f), Quaternion.identity, transform); //bottom section
        wall.tag = "Pillar";
        wall = Instantiate(walls[2], new Vector3(xPos, 1, zPos + 0.5f), Quaternion.identity, transform); //top section
        wall.tag = "Pillar";
        wall = Instantiate(walls[4], new Vector3(xPos - 1, 1, zPos - 0.5f), Quaternion.identity, transform); //bottom left section
        wall.tag = "Pillar";
        wall = Instantiate(walls[4], new Vector3(xPos - 1, 1, zPos + 0.5f), Quaternion.identity * Quaternion.Euler(0, 90f, 0), transform); //top left section
        wall.tag = "Pillar";
        wall = Instantiate(walls[4], new Vector3(xPos + 1, 1, zPos + 0.5f), Quaternion.identity * Quaternion.Euler(0, 180f, 0), transform); //top right section
        wall.tag = "Pillar";
        wall = Instantiate(walls[4], new Vector3(xPos + 1, 1, zPos - 0.5f), Quaternion.identity * Quaternion.Euler(0, 270f, 0), transform); //bottom right section
        wall.tag = "Pillar";
        //Spawning breakables around the pillar (Currently decided not to use this, may change in future)
        // SpawnBreakables(xPos - 2, zPos - 1.5f, 3, true);
        // SpawnBreakables(xPos - 2, zPos - 1.5f, 2, false);
        // SpawnBreakables(xPos + 2, zPos - 1.5f, 2, false);
    }

    //Spawns mobs in the room based on its threat value, and available mobs
    public void SpawnMobs(GameObject[] mobs, int[] mobThreatLevels, int roomThreatBonus) {
        enemiesList = GameObject.Find("EnemiesList").transform;
        float threatLevel = threat * 100;
        threatLevel += roomThreatBonus;
        int minThreatValue = getMin(mobThreatLevels);
        while (threatLevel >= minThreatValue) {
            int mobToSpawn = Random.Range(0, mobThreatLevels.Length);
            if (threatLevel >= mobThreatLevels[mobToSpawn]) {
                SpawnMob(mobs[mobToSpawn]);
                threatLevel -= mobThreatLevels[mobToSpawn];
            }
        } 
    }

    //Spawns the given mob somewhere random (but pathable) within the room
    public void SpawnMob(GameObject mob) {
        bool spawned = false; //will repeat until true!
        float xPos, zPos; //positions to spawn at
        Vector3 spawnPoint; //point to spawn the mob at
        NavMeshHit hit; //navmesh hit data (used for debugging)
        while (!spawned) {
            xPos = Random.Range(xCentre - width / 2, xCentre + width / 2);
            zPos = Random.Range(zCentre - height / 2, zCentre + height / 2);
            spawnPoint = new Vector3 (xPos, 0f, zPos);
            if (NavMesh.SamplePosition(spawnPoint, out hit, 0.1f, NavMesh.AllAreas)) {
                Instantiate(mob, spawnPoint, Quaternion.identity, enemiesList);
                spawned = true;
            } 
        }
    }

    //Gets the lowest value in the array
    public int getMin(int[] values) {
        int answer = 100;
        for (int i = 0; i < values.Length; i++) {
            if (values[i] < answer) answer = values[i];
        }
        return answer;
    }

    //drops rewards to players upon them clearing the room
    public void DropRewards() {
        if (threat >= 0.25f) {
            int goldToDrop = (int)(loot * 10);
            int manaToDrop = (int)(essence * 10);
            int healthToDrop = (int)(threat * 4);

            SpawnPickups(goldDrop, goldToDrop);
            SpawnPickups(manaDrop, manaToDrop);
            SpawnPickups(healthDrop, healthToDrop);
        }
    }

    //spawns pickups upon room completion
    public void SpawnPickups(GameObject pickup, int amount) {
        for(int i = 0; i < amount; i++) {
            Instantiate(pickup, transform.position + GenerateOffset(), Quaternion.identity);
        }
    }

    //generates a random offset to spawn pickups at
    private Vector3 GenerateOffset() {
        float offsetX = Random.Range(-0.5f, 0.5f);
        float offsetZ = Random.Range(-0.5f, 0.5f);
        return new Vector3(offsetX, 0f, offsetZ);
    }

    //Adds a door to the rooms list of doors
    public void AddDoor(DoorController door) {
        doorControllers.Add(door);
    }

    //Locks the doors when player enter a room with enemies
    public void LockDoors() {
        for(int i = 0; i < doorControllers.Count; i++) {
            doorControllers[i].LockDoor();
        }
    }

    //Unlocks the doors when a player clears a room with enemies
    public void UnlockDoors() {
        for (int i = 0; i < doorControllers.Count; i++) {
            doorControllers[i].UnlockDoor();
        }
    }

    //Adds a minimap component that will show up once the room is explored
    public void AddMinimapComponent(GameObject component) {
        minimapComponents.Add(component);
        component.SetActive(false);
    }

    //highlights the room on the minimap
    public void AddToMiniMap() {
        for (int i = 0; i < minimapComponents.Count; i++) {
            minimapComponents[i].SetActive(true);
        }
    }
}

[System.Serializable]
public struct coordinates {
    public float x; //x position of the coordinate
    public float z; //z position of the coordinate
}
