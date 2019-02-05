using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour {

    [SerializeField] private Tilemap ground; //the tilemap to control
    [SerializeField] private Tile[] coridoorTiles; //tiles used for coridoor floors
    [SerializeField] private Tile resetTile; //alpha tile used to reset tilemap when neccessary

    //draws the floor tiles for a coridoor, selecting randomly from the coridoorTiles array
    public void DrawCoridoor(float xCentre, float zCentre, int width, int height){
        for (int x = (int)(xCentre - width / 2f); x < (int)(xCentre + width / 2f); x++){
            for (int z = (int)(zCentre - height / 2f); z < (int)(zCentre + height / 2f); z++){
                ground.SetTile(new Vector3Int(x, z, 0), coridoorTiles[RandomTile(coridoorTiles)]);
            }
        }
    }

    //draws the floor tiles for a room, selecting randomly from the roomTiles arrays based on 'type'
    public void DrawRoom(float xCentre, float zCentre, int width, int height, Tile[] tiles) {
        for (int x = (int)(xCentre - width / 2f); x < (int)(xCentre + width / 2f); x++) {
            for (int z = (int)(zCentre - height / 2f); z < (int)(zCentre + height / 2f); z++) {
                ground.SetTile(new Vector3Int(x, z, 0), tiles[RandomTile(tiles)]);
            }
        }
    }

    //generates a random number up to the length of the tileset
    private int RandomTile(Tile[] tileset){
        return Random.Range(0, tileset.Length);
    }

    //resets all tiles in the tilemap
    public void ResetTiles(int width, int height){
        Debug.Log("Resetting Tiles");
        for (int x = 0; x < width; x++){
            for (int z = 0; z < height; z++){
                ground.SetTile(new Vector3Int(x, z, 0), null);
            }
        }
    }
}
