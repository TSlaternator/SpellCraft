using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room{

    //Class to define a room in the dungeon

	private float xCentre, zCentre; //centre point of the room
    private int height, width; //dimensions of the room
    private int depth; //how deep the room is (how many times the level has been split to create it)
    private Room pair; //the pair of this room (used to create coridoors)
    private Room connectedTo; //extra room this room is connected to (other than pair)
    private RoomController controller; //controller attached to room game object

    //Constructor for the room
	public Room(float xCentre, float zCentre, int width, int height, int depth){
		this.xCentre = xCentre;
		this.zCentre = zCentre;
		this.width = width;
		this.height = height;
		this.depth = depth;
	}

    //sets the rooms pair
	public void setPair(Room pair){
		this.pair = pair;
	}

    //gets the rooms pair
    public Room getPair(){
        return pair;
    }

    //sets the rooms 'connectedTo' room
    public void setConnectedTo(Room connectedTo){
        this.connectedTo = connectedTo;
    }

    //returns the rooms 'connectedTo' room
    public Room getConnectedTo(){
        return connectedTo;
    }

    //returns the rooms xCentre
    public float getXCentre(){
        return xCentre;
    }

    //returns the rooms zCentre
    public float getZCentre(){
        return zCentre;
    }

    //returns the rooms width
    public int getWidth(){
        return width;
    }

    //returns the rooms height
    public int getHeight(){
        return height;
    }

    //returns the rooms depth
    public int getDepth(){
        return depth;
    }

    //sets the roomController variable
    public void setController(RoomController ctrl){
        controller = ctrl;
    }

    //returns the controller
    public RoomController getController() {
        return controller;
    }

}
