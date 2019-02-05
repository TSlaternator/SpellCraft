using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGroup{

    //defines a group of rooms in the dungeon

	private List<Room> rooms; //list of all rooms in the group
	private int roomCount; //count of all rooms in the group
	private float[] centre; //centre point of the group (derived from centre of all rooms)

    //constructor
	public RoomGroup(){
		rooms = new List<Room> ();
		roomCount = 0;
		centre = new float[2];
	}

    //returns the rooms in this group
	public List<Room> GetRooms(){
		return rooms;
	}

    //returns a specific room in this group
	public Room GetRoom(int index){
		return rooms [index];
	}

    //adds a room to the group
	public void AddRoom(Room room){
		rooms.Add (room);
		roomCount++;
        CalculateCentre();
	}

    //removes a room from the group
	public void RemoveRoom(Room room){
		rooms.Remove (room);
		roomCount--;
        CalculateCentre();
	}

    //adds a list of rooms to the group
	public void AddRooms(List<Room> roomsToAdd){
		for (int i = 0; i < roomsToAdd.Count; i++) {
			rooms.Add (roomsToAdd [i]);
			roomCount++;
		}
        CalculateCentre();
	}

    //calculates the centre of the roomGroup
	private void CalculateCentre(){
		float xPos = 0;
		float zPos = 0;
		for (int i = 0; i < roomCount; i++) {
			xPos += rooms [i].getXCentre();
			zPos += rooms [i].getZCentre();
		}
		centre[0] = xPos / roomCount;
		centre[1] = zPos / roomCount;
	}

    //returns the number of rooms in this group
    public int getCount(){
        return roomCount;
    }

    //returns the centre point of the roomGroup
    public float[] getCentre(){
        return centre;
    }
}
