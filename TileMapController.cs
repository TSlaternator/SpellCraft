using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour {

    [SerializeField] private Tilemap ground; //the tilemap to control
    [SerializeField] private Tilemap fog; //tilemap used to darken rooms until they are entered for the first time
    [SerializeField] private Tile[] coridoorTiles; //tiles used for coridoor floors
    [SerializeField] private Tile resetTile; //alpha tile used to reset tilemap when neccessary
    [SerializeField] private Tile[] fogTiles; //Varying degrees of transparency, with the least being 90% opaque

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

    //fills the rooms tilemap with the maximum fog tile 
    public void DrawFog(float xCentre, float zCentre, int width, int height) {
        for (int x = (int)(xCentre - width / 2f); x < (int)(xCentre + width / 2f); x++) {
            for (int z = (int)(zCentre - height / 2f); z < (int)(zCentre + height / 2f); z++) {
                fog.SetTile(new Vector3Int(x, z, 0), fogTiles[0]);
            }
        }
    }

    //Coroutine to slowly fade the fog out
    public IEnumerator FadeFog(float xCenter, float zCenter, int width, int height, int fogLevel) {
        while (fogLevel <= fogTiles.Length) {
            RemoveFog(xCenter, zCenter, width, height, fogLevel++);
            yield return new WaitForSeconds(0.03f);
        }
    }

    //starts the coroutine to gradually remove a rooms fog
    public void RemoveFog(float xCentre, float zCentre, int width, int height) {
        StartCoroutine(FadeFog(xCentre, zCentre, width, height, 0));
    }

    //sets fog to the level supplied
    public void RemoveFog(float xCenter, float zCenter, int width, int height, int fogLevel) {
        for (int x = (int)(xCenter - width / 2f); x < (int)(xCenter + width / 2f); x++) {
            for (int z = (int)(zCenter - height / 2f); z < (int)(zCenter + height / 2f); z++) {
                if (fogLevel < fogTiles.Length) fog.SetTile(new Vector3Int(x, z, 0), fogTiles[fogLevel]);
                else fog.SetTile(new Vector3Int(x, z, 0), null);
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
                fog.SetTile(new Vector3Int(x, z, 0), null);
            }
        }
    }
}
