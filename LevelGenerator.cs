using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour {

	[SerializeField] private int levelWidth, levelHeight; //overall dimensions of the level
	[SerializeField] private int minRoomSize, maxRoomSize; //max and min size a room can be
	[SerializeField] private List<Room> rooms; //list of rooms in the level
	[SerializeField] private List<RoomGroup> groups; //list of groups of rooms (used when joining up the level)
	[SerializeField] private Transform roomList; //rooms are spawned under this transform to reduce clutter in the GUI
	[SerializeField] private Transform coridoorList; //coridoors are spawned under this transform to reduce clutter in the GUI
    [SerializeField] private GameObject coridoor; //empty gameObject with script to control coridoor creation
    [SerializeField] private GameObject room; //empty gameObject with script to control room creation
    [SerializeField] private TileMapController tileController; //controls the tileMap, used to refresh all tiles on a failed generation
    [SerializeField] private NavMeshSurface[] navmeshes; //nav mesh surfaces, should be baked after the rooms have spawned
    public GenericRoom genericRoom; //struct to hold generic room properties
    public BossRoom bossRoom; //struct to hold boss room properties
    public LibraryRoom libraryRoom; //struct to hold library room properties
    public ShopRoom shopRoom; //struct to hold shoproom properties
    public ShrineRoom shrineRoom; //struct to hold shrine room properties
    public LootRoom lootRoom; //struct to hold loot room properties
    public MobRoom mobRoom; //struct to hold mob room properties
	private bool spawnFailed = true; //if true when generation is 'finished', the level will be generated again

    //main method to generate the map
	void Start () {
		while (spawnFailed) {
			try{
				spawnFailed = false;
				SubLevel rootLevel = new SubLevel (levelWidth / 2f, levelHeight / 2f, levelWidth, levelHeight, minRoomSize, true, 0);
                SplitLevel(rootLevel);
				rootLevel.CreateRoom ();
				rootLevel.FindPairs ();
				rooms = new List<Room> ();
				groups = new List<RoomGroup> ();
				GetRooms (rootLevel);
				SpawnRooms ();
                GroupRooms ();
				SpawnCoridoorsWithinGroups();
				SpawnCoridoorsBetweenGroups();
                if (!spawnFailed) {
                    BuildRooms();
                    BuildNavMesh();
                }
            } catch {
				spawnFailed = true;
			}
			if (spawnFailed) {
				for (int i = 0; i < roomList.childCount; i++) {
					Destroy (roomList.GetChild (i).gameObject);
				}
				for (int i = 0; i < coridoorList.childCount; i++) {
					Destroy (coridoorList.GetChild (i).gameObject);
				}
                while (roomList.childCount > 0) roomList.GetChild(0).parent = null;
                tileController.ResetTiles(levelWidth, levelHeight);
                for (int i = 0; i < navmeshes.Length; i++) {
                    navmeshes[i].RemoveData();
                }
			}
		}
	}

    //builds the navmesh after generating the level
    private void BuildNavMesh() {
        for(int i = 0; i < navmeshes.Length; i++) {
            navmeshes[i].BuildNavMesh();
        }
    }

    //splits the SubLevel into smaller SubLevels until they're the correct size
	private void SplitLevel(SubLevel level){
		if (level.IsLeaf () && level.IsLarge(maxRoomSize)) {
			if (level.SplitLevel (minRoomSize, maxRoomSize)) {
                SplitLevel(level.getChild(1));
                SplitLevel(level.getChild(2));
			}
		}
	}

    //loops through each room, creating its physical walls and floor
    private void BuildRooms() {
        for (int i = 0; i < roomList.childCount; i++) {
            rooms[i].getController().SpawnRoom();
        }
    }

    //groups together rooms based on their connections 
	private void GroupRooms(){
		groups.Add (new RoomGroup ());
		groups [0].AddRoom (rooms [0]);
		for (int i = 1; i < rooms.Count; i++) {
			bool newGroup = true;
			for (int j = 0; j < groups.Count; j++) {
				if (groups [j].GetRooms ().Contains (rooms[i].getPair())) {
					groups [j].AddRoom (rooms [i]);
					newGroup = false;
				}
			} 
			if (newGroup) {
				groups.Add (new RoomGroup ());
				groups [groups.Count - 1].AddRoom (rooms [i]);
			}
		} 
		for (int i = groups.Count - 1; i >= 0; i--) {
			for (int j = 0; j < groups[i].getCount(); j++){
				Room roomToGroup = groups [i].GetRoom (j);
				for (int k = 0; k < groups.Count; k++) {
					if (k != i) {
						if (groups [k].GetRooms ().Contains (roomToGroup.getPair())) {
							groups [k].AddRoom (roomToGroup);
							groups.RemoveAt (i);
						}
					}
				} 
			}
		}
	}

    //Creates a list of all rooms in the level
	private void GetRooms(SubLevel level){
		if (level.getChild(1) != null) {
			GetRooms (level.getChild(1));
		}
		if (level.getChild(2) != null) {
			GetRooms (level.getChild(2));
		}
		if (level.IsLeaf ()) {
			rooms.Add (level.getRoom());
		}
	}

    //Spawns in the rooms
	private void SpawnRooms(){
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].getPair() != rooms [i]) {
                SpawnRoom(rooms[i]);
			} else {
				rooms.Remove (rooms[i--]);
			}
		}

        int smallestSize = maxRoomSize * maxRoomSize;
        int biggestSize = 0;
        RoomController smallestRoom = rooms[0].getController();
        RoomController biggestRoom = smallestRoom;

        //finding biggest and smallest rooms
        for (int i = 0; i < rooms.Count; i++) {
            if(rooms[i].getController().getSize() < smallestSize) {
                smallestRoom = rooms[i].getController();
                smallestSize = smallestRoom.getSize();
            }
            else if (rooms[i].getController().getSize() > biggestSize) {
                biggestRoom = rooms[i].getController();
                biggestSize = biggestRoom.getSize();
            }
        }

        //setting a spawn, boss, library and shop room
        smallestRoom.setRoomType(0);
        biggestRoom.setRoomType(1);
        SpawnSpecialRoom(2);
        SpawnSpecialRoom(3);

        //setting the rest of the rooms
        for (int i = 0; i < rooms.Count; i++) {
            RoomController currentRoom = rooms[i].getController();
            if (currentRoom.getRoomType() == -1) {
                currentRoom.setRoomType();
            }
        } 
	}

    //spawns a special room (such as the library, or shop) that appear once per floor
    private void SpawnSpecialRoom(int roomType) {
        int roomID = Random.Range(0, rooms.Count);
        RoomController room = rooms[roomID].getController();
        if (room.getRoomType() == -1) room.setRoomType(roomType);
        else SpawnSpecialRoom(roomType);
    }

    //Spawns in a room object
    private void SpawnRoom(Room newRoom) {
        Vector3 spawnPoint = new Vector3(newRoom.getXCentre(), 0, newRoom.getZCentre());
        GameObject roomObject = Instantiate(room, spawnPoint, Quaternion.identity, roomList);
        RoomController controller = roomObject.GetComponent<RoomController>();
        controller.setVariables(newRoom.getWidth(), newRoom.getHeight());
        controller.SetDoors();
        newRoom.setController(controller);
    }

    //spawns coridoors between rooms in the same group
	private void SpawnCoridoorsWithinGroups(){
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i].getPair().getConnectedTo() != rooms[i] && rooms [i].getPair() != null) {
				SpawnCoridoor (rooms [i], rooms [i].getPair(), true, false);
				rooms [i].setConnectedTo(rooms[i].getPair());
			}
		}
	}

    //spawns coridoors between unconnected groups
	private void SpawnCoridoorsBetweenGroups(){
		while (groups.Count > 1) {
			RoomGroup[] tempGroups = findClosestGroup (groups);
			JoinGroups (tempGroups [0], tempGroups [1]);
			tempGroups [0].AddRooms (tempGroups [1].GetRooms());
			groups.Remove (tempGroups [1]);
		}
	}

    //spawns coridoors for every situation
	private bool SpawnCoridoor(Room roomA, Room roomB, bool repeat, bool green){
		Vector3 roomAEdge, roomBEdge;
        float width = 0;
        float height = 0;
		bool success = true;
		if (roomA.getXCentre() == roomB.getXCentre()) {
			if (roomA.getZCentre() > roomB.getZCentre()) {
				roomAEdge = new Vector3 (roomA.getXCentre(), 0f, roomA.getZCentre() - (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 (roomB.getXCentre(), 0f, roomB.getZCentre() + (roomB.getHeight() / 2f));
				height = (roomA.getZCentre() - (roomA.getHeight() / 2f)) - (roomB.getZCentre() + (roomB.getHeight() / 2f));
			} else {
				roomAEdge = new Vector3 (roomA.getXCentre(), 0f, roomA.getZCentre() + (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 (roomB.getXCentre(), 0f, roomB.getZCentre() - (roomB.getHeight() / 2f));
				height = (roomB.getZCentre() - (roomB.getHeight() / 2f)) - (roomA.getZCentre() + (roomA.getHeight() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, 2, (int)height, roomA, roomB, false);

		} else if (roomA.getZCentre() == roomB.getZCentre()) {
			if (roomA.getXCentre() > roomB.getXCentre()) {
				roomAEdge = new Vector3 (roomA.getXCentre() - (roomA.getWidth() / 2f), 0f, roomA.getZCentre());
				roomBEdge = new Vector3 (roomB.getXCentre() + (roomB.getWidth() / 2f), 0f, roomB.getZCentre());
				width = (roomA.getXCentre() - (roomA.getWidth() / 2f)) - (roomB.getXCentre() + (roomB.getWidth() / 2f));
			} else {
				roomAEdge = new Vector3 (roomA.getXCentre() + (roomA.getWidth() / 2f), 0f, roomA.getZCentre());
				roomBEdge = new Vector3 (roomB.getXCentre() - (roomB.getWidth() / 2f), 0f, roomB.getZCentre());
				width = (roomB.getXCentre() - (roomB.getWidth() / 2f)) - (roomA.getXCentre() + (roomA.getWidth() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, (int)width, 2, roomA, roomB, true);

        } else if (IsInside (roomA.getXCentre(), false, roomB)) {
			if (roomA.getZCentre() > roomB.getZCentre()) {
				roomAEdge = new Vector3 (roomA.getXCentre(), 0f, roomA.getZCentre() - (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 (roomA.getXCentre(), 0f, roomB.getZCentre() + (roomB.getHeight() / 2f));
				height = (roomA.getZCentre() - (roomA.getHeight() / 2f)) - (roomB.getZCentre() + (roomB.getHeight() / 2f));
			} else {
				roomAEdge = new Vector3 (roomA.getXCentre(), 0f, roomA.getZCentre() + (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 (roomA.getXCentre(), 0f, roomB.getZCentre() - (roomB.getHeight() / 2f));
				height = (roomB.getZCentre() - (roomB.getHeight() / 2f)) - (roomA.getZCentre() + (roomA.getHeight() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, 2, (int)height, roomA, roomB, false);

        } else if (IsInside (roomA.getZCentre(), true, roomB)) {
			if (roomA.getXCentre() > roomB.getXCentre()) {
				roomAEdge = new Vector3 (roomA.getXCentre() - (roomA.getWidth() / 2f), 0f, roomA.getZCentre());
				roomBEdge = new Vector3 (roomB.getXCentre() + (roomB.getWidth() / 2f), 0f, roomA.getZCentre());
				width = (roomA.getXCentre() - (roomA.getWidth() / 2f)) - (roomB.getXCentre() + (roomB.getWidth() / 2f));
			} else {
				roomAEdge = new Vector3 (roomA.getXCentre() + (roomA.getWidth() / 2f), 0f, roomA.getZCentre());
				roomBEdge = new Vector3 (roomB.getXCentre() - (roomB.getWidth() / 2f), 0f, roomA.getZCentre());
				width = (roomB.getXCentre() - (roomB.getWidth() / 2f)) - (roomA.getXCentre() + (roomA.getWidth() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, (int)width, 2, roomA, roomB, true);

        } else if (IsInside ((roomA.getXCentre() - (roomA.getWidth() / 2f) + 1), false, roomB)) {
			if (roomA.getZCentre() > roomB.getZCentre()) {
				roomAEdge = new Vector3 ((roomA.getXCentre() - (roomA.getWidth() / 2f) + 1), 0f, roomA.getZCentre() - (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 ((roomA.getXCentre() - (roomA.getWidth() / 2f) + 1), 0f, roomB.getZCentre() + (roomB.getHeight() / 2f));
				height = (roomA.getZCentre() - (roomA.getHeight() / 2f)) - (roomB.getZCentre() + (roomB.getHeight() / 2f));
			} else {
				roomAEdge = new Vector3 ((roomA.getXCentre() - (roomA.getWidth() / 2f) + 1), 0f, roomA.getZCentre() + (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 ((roomA.getXCentre() - (roomA.getWidth() / 2f) + 1), 0f, roomB.getZCentre() - (roomB.getHeight() / 2f));
				height = (roomB.getZCentre() - (roomB.getHeight() / 2f)) - (roomA.getZCentre() + (roomA.getHeight() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, 2, (int)height, roomA, roomB, false);

        } else if (IsInside ((roomA.getXCentre() + (roomA.getWidth() / 2f) - 1), false, roomB)) {
			if (roomA.getZCentre() > roomB.getZCentre()) {
				roomAEdge = new Vector3 ((roomA.getXCentre() + (roomA.getWidth() / 2f) - 1), 0f, roomA.getZCentre() - (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 ((roomA.getXCentre() + (roomA.getWidth() / 2f) - 1), 0f, roomB.getZCentre() + (roomB.getHeight() / 2f));
				height = (roomA.getZCentre() - (roomA.getHeight() / 2f)) - (roomB.getZCentre() + (roomB.getHeight() / 2f));
			} else {
				roomAEdge = new Vector3 ((roomA.getXCentre() + (roomA.getWidth() / 2f) - 1), 0f, roomA.getZCentre() + (roomA.getHeight() / 2f));
				roomBEdge = new Vector3 ((roomA.getXCentre() + (roomA.getWidth() / 2f) - 1), 0f, roomB.getZCentre() - (roomB.getHeight() / 2f));
				height = (roomB.getZCentre() - (roomB.getHeight() / 2f)) - (roomA.getZCentre() + (roomA.getHeight() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, 2, (int)height, roomA, roomB, false);

        } else if (IsInside ((roomA.getZCentre() - (roomA.getHeight() / 2f) + 1), true, roomB)) {
			if (roomA.getXCentre() > roomB.getXCentre()) {
				roomAEdge = new Vector3 (roomA.getXCentre() - (roomA.getWidth() / 2f), 0f, (roomA.getZCentre() - (roomA.getHeight() / 2f) + 1));
				roomBEdge = new Vector3 (roomB.getXCentre() + (roomB.getWidth() / 2f), 0f, (roomA.getZCentre() - (roomA.getHeight() / 2f) + 1));
				width = (roomA.getXCentre() - (roomA.getWidth() / 2f)) - (roomB.getXCentre() + (roomB.getWidth() / 2f));
			} else {
				roomAEdge = new Vector3 (roomA.getXCentre() + (roomA.getWidth() / 2f), 0f, (roomA.getZCentre() - (roomA.getHeight() / 2f) + 1));
				roomBEdge = new Vector3 (roomB.getXCentre() - (roomB.getWidth() / 2f), 0f, (roomA.getZCentre() - (roomA.getHeight() / 2f) + 1));
				width = (roomB.getXCentre() - (roomB.getWidth() / 2f)) - (roomA.getXCentre() + (roomA.getWidth() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, (int)width, 2, roomA, roomB, true);
        }
        else if (IsInside ((roomA.getZCentre() + (roomA.getHeight() / 2f) - 1), true, roomB)) {
			if (roomA.getXCentre() > roomB.getXCentre()) {
				roomAEdge = new Vector3 (roomA.getXCentre() - (roomA.getWidth() / 2f), 0f, (roomA.getZCentre() + (roomA.getHeight() / 2f) - 1));
				roomBEdge = new Vector3 (roomB.getXCentre() + (roomB.getWidth() / 2f), 0f, (roomA.getZCentre() + (roomA.getHeight() / 2f) - 1));
				width = (roomA.getXCentre() - (roomA.getWidth() / 2f)) - (roomB.getXCentre() + (roomB.getWidth() / 2f));
			} else {
				roomAEdge = new Vector3 (roomA.getXCentre() + (roomA.getWidth() / 2f), 0f, (roomA.getZCentre() + (roomA.getHeight() / 2f) - 1));
				roomBEdge = new Vector3 (roomB.getXCentre() - (roomB.getWidth() / 2f), 0f, (roomA.getZCentre() + (roomA.getHeight() / 2f) - 1));
				width = (roomB.getXCentre() - (roomB.getWidth() / 2f)) - (roomA.getXCentre() + (roomA.getWidth() / 2f));
			}
            SpawnCoridoor((roomAEdge + roomBEdge) / 2f, (int)width, 2, roomA, roomB, true);
        }
        else {
			if (repeat)
				return (SpawnCoridoor (roomB, roomA, false, green));
			else if (!green) {
				for (int i = 0; i < groups.Count; i++) {
					if (groups [i].GetRooms ().Contains (roomB)) {
						groups [i].RemoveRoom (roomB);
					}
				}
				groups.Add (new RoomGroup ());
				groups [groups.Count - 1].AddRoom (roomB);
			} else
				success = false;
		} 
        //stops corridoors spawning through rooms
        if (width >= minRoomSize * 1.5 || height > minRoomSize * 1.5) {
            success = false;
        }
		return success;
	}

    //spawns a coridoor gameObject and sets up its variables
    private void SpawnCoridoor(Vector3 spawnPoint, int width, int height, Room roomA, Room roomB, bool isHorizontal){
        GameObject newCoridoor = Instantiate(coridoor, spawnPoint, Quaternion.identity, coridoorList);
        CoridoorController controller = newCoridoor.GetComponent<CoridoorController>();
        controller.setVariables(width, height, isHorizontal);
        controller.SpawnCoridoor();
        controller.setRooms(roomA, roomB);
    }

    //checks to see if a point from one room would be inside another room
	private bool IsInside(float point, bool direction, Room roomB){
		float roomStart, roomEnd;
		if (direction == true) {
			roomStart = roomB.getZCentre() - (roomB.getHeight() / 2f) + 1;
			roomEnd = roomB.getZCentre() + (roomB.getHeight() / 2f) - 1;
		} else {
			roomStart = roomB.getXCentre() - (roomB.getWidth() / 2f) + 1;
			roomEnd = roomB.getXCentre() + (roomB.getWidth() / 2f) - 1;
		}
		return (roomStart <= point && point <= roomEnd);
	}

    //finds the closest pair of groups (based on their centre point)
	private RoomGroup[] findClosestGroup(List<RoomGroup> roomGroups){
		RoomGroup[] returnGroup = new RoomGroup[2];
		returnGroup [0] = roomGroups [0];
		float closestDistance = float.MaxValue;
		for (int i = 1; i < roomGroups.Count; i++) {
			float euclideanDistance = EuclideanDist (returnGroup [0], roomGroups [i]);
			if (euclideanDistance < closestDistance) {
				closestDistance = euclideanDistance;
				returnGroup[1] = roomGroups[i];
			}
		}
		return returnGroup;
	}

    //calculates euclidean distance between two groups
	private float EuclideanDist(RoomGroup groupA, RoomGroup groupB){
		return Mathf.Sqrt (Mathf.Pow((groupB.getCentre()[0] - groupA.getCentre()[0]), 2) + Mathf.Pow((groupB.getCentre()[1] - groupA.getCentre()[1]), 2));
	}

    //finds the closest rooms from groupA and groupB
	private Room[] findClosestRoom(RoomGroup groupA, RoomGroup groupB){
		Room[] returnRoom = new Room[2];
		float closestDistance = float.MaxValue;
		for (int i = 1; i < groupA.getCount(); i++) {
			for (int j = 0; j < groupB.getCount(); j++) {
				float euclideanDistance = EuclideanDist (groupA.GetRoom(i), groupB.GetRoom(j));
				if (euclideanDistance < closestDistance) {
					returnRoom [0] = groupA.GetRoom(i);
					returnRoom [1] = groupB.GetRoom(j);
					closestDistance = euclideanDistance;
				}
			}
		}
		return returnRoom;
	}

    //joins two groups together with a coridoor
	private void JoinGroups(RoomGroup groupA, RoomGroup groupB){
		Room[] closestRooms = new Room[2];
		float closestDistance = float.MaxValue;
		for (int i = 1; i < groupA.getCount(); i++) {
			for (int j = 0; j < groupB.getCount(); j++) {
				float euclideanDistance = EuclideanDist (groupA.GetRoom(i), groupB.GetRoom(j));
				if (euclideanDistance < closestDistance) {
					closestRooms [0] = groupA.GetRoom(i);
					closestRooms [1] = groupB.GetRoom(j);
					closestDistance = euclideanDistance;
				}
			}
		}
		if (!SpawnCoridoor(closestRooms[0], closestRooms[1], true, true)) spawnFailed = true;
	}

    //calculates the euclidean distance between two rooms
	private float EuclideanDist(Room roomA, Room roomB){
		return Mathf.Sqrt (Mathf.Pow((roomB.getXCentre() - roomA.getXCentre()), 2) + Mathf.Pow((roomB.getZCentre() - roomA.getZCentre()), 2));
	}
}

[System.Serializable]
public struct GenericRoom {
    public GameObject[] walls; //walls used in the room
    public Tile[] floorTiles; //floor tiles of the room
}

[System.Serializable]
public struct BossRoom {
    public GameObject[] walls; //walls used in the room
    public Tile[] floorTiles; //floor tiles of the room
}

[System.Serializable]
public struct ShopRoom {
    public GameObject[] walls; //walls used in the room
    public Tile[] floorTiles; //floor tiles of the room
    public GameObject shop; //shop sprites
}

[System.Serializable]
public struct LibraryRoom {
    public GameObject[] walls; //walls used in the room
    public Tile[] floorTiles; //floor tiles of the room
    public GameObject scribingTable; //scribing table sprite
}

[System.Serializable]
public struct ShrineRoom {
    public GameObject[] walls; //walls used in the room
    public Tile[] floorTiles; //floor tiles of the room
    public GameObject shrine; //shrine game object
}
[System.Serializable]
public struct LootRoom {
    public GameObject[] walls; //walls used in the room
    public Tile[] floorTiles; //floor tiles of the room
    public GameObject chest; //chest to hold loot
}

[System.Serializable]
public struct MobRoom {
    public GameObject[] walls; //walls used in the room
    public Tile[] floorTiles; //floor tiles of the room
}
