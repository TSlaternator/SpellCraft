using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLevel{

    //defines a subLevel within the dungeon (created by BSP)

	private SubLevel child1, child2; //the children of this SubLevel
    private SubLevel pair; //this SubLevels organic pair
    private Room room; //the room inside this SubLevel
    private float xCentre, zCentre; //this SubLevel's centre coordinates
    private int height, width; //this SubLevel's dimensions
    private float roomXCentre, roomZCentre; //this SubLevel's room's centre coordinates
    private int roomHeight, roomWidth; //this SubLevel's room's dimensions
    private bool splitHorizontal; //whether to split the SubLevel horizontally (or vertically if false)
    private int splitPoint; //the point the SubLevel is split at
	private int minRoomSize; //minimum size of a room
    private int depth; //the depth of this SubLevel in the BSP

    //constructor for the SubLevel
	public SubLevel(float xPos, float zPos, int w, int h, int minRoomSize, bool isLeft, int depth){
		xCentre = xPos;
		zCentre = zPos;
		width = w;
		height = h;
		this.minRoomSize = minRoomSize;
		this.depth = depth;
	}

    //returns true if the subLevel is a leaf node (no children)
	public bool IsLeaf(){
		return child1 == null && child2 == null;
	}

    //returns true if the SubLevel is too large and should be split further
	public bool IsLarge(int maxDimensions){
		return height > maxDimensions || width > maxDimensions;
	}

    //splits the SubLevel into two smaller SubLevels
	public bool SplitLevel(int minLevelSize, int maxLevelSize){
		if (!IsLeaf ()) {
			return false;
		}

		if ((float)width / (float)height >= 1.2f) splitHorizontal = false;
		else if ((float)height / (float)width >= 1.2f) splitHorizontal = true;
		else splitHorizontal = Random.Range (0.0f, 1.0f) > 0.5f;

		if ((Mathf.Max (height, width) / 3)< minLevelSize) {
			return false;
		}

		if (splitHorizontal) {
			splitPoint = Random.Range (minLevelSize, height - minLevelSize);

			child1 = new SubLevel (xCentre, zCentre - height / 2f + splitPoint / 2f, width, splitPoint, minRoomSize, true, depth + 1);
			child2 = new SubLevel (xCentre, zCentre + splitPoint / 2f, width, height - splitPoint, minRoomSize, false, depth + 1);
			child1.pair = child2;
			child2.pair = child1;
		} else {
			splitPoint = Random.Range (minLevelSize, width - minLevelSize);

            child1 = new SubLevel (xCentre - width / 2f + splitPoint / 2f, zCentre, splitPoint, height, minRoomSize, true, depth + 1);
			child2 = new SubLevel (xCentre + splitPoint / 2f, zCentre, width - splitPoint, height, minRoomSize, false, depth + 1);
			child1.pair = child2;
			child2.pair = child1;
		}

		return true;
	}

    //Recursive Method to Create Rooms in all the leaf SubLevels
	public void CreateRoom(){
		if (child1 != null) {
			child1.CreateRoom ();
		}
		if (child2 != null) {
			child2.CreateRoom ();
		}
		if (IsLeaf ()) {
			roomWidth = (int)Random.Range (minRoomSize, width - 4);
			roomHeight = (int)Random.Range (minRoomSize, height - 4);
            if (roomWidth % 2 != 0) roomWidth -= 1;
            if (roomHeight % 2 != 0) roomHeight -= 1;
			room = new Room ((int)xCentre, (int)zCentre, roomWidth, roomHeight, depth);
			if (pair != null && pair.room != null) {
				room.setPair (pair.room);
				pair.room.setPair (room);
			} 
		}
	}

    //Recursive Method to find a SubLevels organic pair
	public void FindPairs(){
		if (child1 != null) {
			child1.FindPairs ();
		}
		if (child2 != null) {
			child2.FindPairs ();
		}
		if (IsLeaf()){
			if (room.getPair() == null) {
				room.setPair (FindPair ());
				room.getPair().setPair (room);
			}
		}
	}

    //Method to find a pair for SubLevels with no organic pair
	public Room FindPair(){
		List<Room> rooms = new List<Room> ();
		FindRooms (pair, rooms);
		Room pairRoom = rooms [0];
		float dist = GetDistance (rooms [0]);
		float temp;
		for (int i = 1; i < rooms.Count; i++) {
			temp = GetDistance (rooms [i]);
			if (temp < dist) {
				dist = temp;
				pairRoom = rooms [i];
			}
		}

		return pairRoom;
	}

    //Method to create a list of all rooms created
	public void FindRooms(SubLevel level, List<Room> roomsList){
		if (level.IsLeaf())
			roomsList.Add(level.room);
		else {
			FindRooms (level.child1, roomsList);
			FindRooms (level.child2, roomsList);
		}
	}

    //gets the euclidean distance between this SubLevels room and another room
	public float GetDistance(Room otherRoom){
		return Mathf.Sqrt (Mathf.Pow((otherRoom.getXCentre() - room.getXCentre()), 2) + Mathf.Pow((otherRoom.getZCentre() - room.getZCentre()), 2));
	}

    //returns the SubLevel's children
    public SubLevel getChild(int childID){
        if (childID == 1) return child1;
        else if (childID == 2) return child2;
        else return null;
    }

    //returns this SubLevel's room
    public Room getRoom(){
        return room;
    }

    //returns this SubLevel's depth
    public int getDepth(){
        return depth;
    }

    //returns this SubLevel's room's width
    public int getRoomWidth(){
        return roomWidth;
    }

    //returns this SubLevel's room's height
    public int getRoomHeight(){
        return roomHeight;
    }

    //returns this SubLevel's xCentre
    public float getXCentre(){
        return xCentre;
    }

    //returns this SubLevel's zCentre
    public float getZCentre(){
        return zCentre;
    }
}
