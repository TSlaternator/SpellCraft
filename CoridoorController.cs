using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoridoorController : MonoBehaviour {

    private float xCentre, zCentre; //centre of the coridoor
    private int width, height; //dimensions of the coridoor
    private bool isHorizontal; //if the coridoor is horizontal or vertical
    private TileMapController tileGenerator; //allows spawning of ground tiles
    [SerializeField] private GameObject northWall; //walls along the north side of the coridoors
    [SerializeField] private GameObject sideWalls; //walls along the sides of the coridoors
    [SerializeField] private GameObject southWall; //walls along the south of the coridoors
    [SerializeField] private GameObject door; //door object
    [SerializeField] private GameObject sideDoor; //side door object

    //spawns the coridoors tiles
    public void SpawnCoridoor() {
        tileGenerator.DrawCoridoor(xCentre, zCentre, width, height);
        SpawnWalls();
    }

    //sets the dimensions of the coridoor
    public void setVariables(int width, int height, bool isHorizontal) {
        this.width = width;
        this.height = height;
        this.isHorizontal = isHorizontal;
        xCentre = gameObject.transform.position.x;
        zCentre = gameObject.transform.position.z;
        tileGenerator = GameObject.Find("LevelManager").GetComponent<TileMapController>();
    }

    //lets the attached rooms know where the door is (so they know to leave room for it!)
    public bool setRooms(Room room1, Room room2) {
        bool success = true;
        if (isHorizontal) {
            if (room1.getXCentre() > room2.getXCentre()) {
                if (room1.getController().setDoor(1, (int)zCentre)) success = false;
                if (room2.getController().setDoor(3, (int)zCentre)) success = false;
            } else {
                if (room1.getController().setDoor(3, (int)zCentre)) success = false;
                if (room2.getController().setDoor(1, (int)zCentre)) success = false;
            }
        } else {
            if (room1.getZCentre() > room2.getZCentre()) {
                if (room1.getController().setDoor(2, (int)xCentre)) success = false;
                if (room2.getController().setDoor(0, (int)xCentre)) success = false;
            } else {
                if (room1.getController().setDoor(0, (int)xCentre)) success = false;
                if (room2.getController().setDoor(2, (int)xCentre)) success = false;
            }
        }
        return success;
    }

    public void SpawnWalls() {
        Vector3 spawnPos;
        GameObject thisWall;
        GameObject thisDoor;
        Transform wallTexture;
        bool evenLength;
        if (isHorizontal) {
            evenLength = (width % 2 == 0) ? true : false;
            //north wall
            spawnPos = new Vector3(xCentre - 0.5f, 1, zCentre + 1f);
            thisWall = Instantiate(northWall, spawnPos, Quaternion.identity, gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * (width - 2), thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(width, 1);
            thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(width - 2, 1);
            wallTexture = thisWall.GetComponentsInChildren<Transform>()[1];
            wallTexture.localScale = new Vector3((thisWall.transform.localScale.x * width) / (thisWall.transform.localScale.x * (width - 2)) / 10, 1f, 0.1f);
            //south wall
            spawnPos = new Vector3(xCentre - 0.5f, 1, zCentre - 2f);
            thisWall = Instantiate(southWall, spawnPos, Quaternion.identity, gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * (width - 2), thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentsInChildren<Renderer>()[0].material.mainTextureScale = new Vector2(width, 1);
            thisWall.GetComponentsInChildren<Renderer>()[1].material.mainTextureScale = new Vector2(width - 2, 1);
            wallTexture = thisWall.GetComponentsInChildren<Transform>()[1];
            wallTexture.localScale = new Vector3((thisWall.transform.localScale.x * width) / (thisWall.transform.localScale.x * (width - 2)) / 10, 1f, 0.1f);
            //doors
            if (evenLength) spawnPos = new Vector3(xCentre - 0.6f + width / 2, 0, zCentre - 0.5f);
            else spawnPos = new Vector3(xCentre - 0.1f + width / 2, 0, zCentre - 0.5f);
            thisDoor = Instantiate(sideDoor, spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
            thisDoor.GetComponent<DoorController>().setFacing(1);
            if (evenLength) spawnPos = new Vector3(xCentre - 0.4f - width / 2, 0, zCentre - 0.5f);
            else spawnPos = new Vector3(xCentre - 0.9f - width / 2, 0, zCentre - 0.5f);
            thisDoor = Instantiate(sideDoor, spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
            thisDoor.GetComponent<DoorController>().setFacing(3);
        } else {
            evenLength = (height % 2 == 0) ? true : false;
            //side walls
            spawnPos = new Vector3(xCentre - 2f, 1, zCentre - 0.5f);
            thisWall = Instantiate(sideWalls, spawnPos, Quaternion.identity * Quaternion.Euler(0, -90, 0), gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * (height - 2), thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(height - 2, 1);
            spawnPos = new Vector3(xCentre + 1f, 1, zCentre - 0.5f);
            thisWall = Instantiate(sideWalls, spawnPos, Quaternion.identity * Quaternion.Euler(0, 90, 0), gameObject.transform);
            thisWall.transform.localScale = new Vector3(thisWall.transform.localScale.x * (height - 2), thisWall.transform.localScale.y, thisWall.transform.localScale.z);
            thisWall.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(height - 2, 1);
            //doors
            if (evenLength) spawnPos = new Vector3(xCentre - 0.5f, 0, zCentre - 0.6f + height / 2);
            else spawnPos = new Vector3(xCentre - 0.5f, 0, zCentre - 0.1f + height / 2);
            thisDoor = Instantiate(door, spawnPos, Quaternion.identity, gameObject.transform);
            thisDoor.GetComponent<DoorController>().setFacing(0);
            if (evenLength) spawnPos = new Vector3(xCentre - 0.5f, 0, zCentre - 0.4f - height / 2);
            else spawnPos = new Vector3(xCentre - 0.5f, 0, zCentre - 0.9f - height / 2);
            thisDoor = Instantiate(door, spawnPos, Quaternion.identity * Quaternion.Euler(0, 180, 0), gameObject.transform);
            thisDoor.GetComponent<DoorController>().setFacing(2);
        }
    }
}
