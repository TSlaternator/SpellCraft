using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour {

    [SerializeField] private Tilemap ground; //the tilemap to control
    [SerializeField] private Tilemap decoration; //the tilemap used for carpets / borders
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

    //Draws a decorative border on the rooms floor tiles
    public void DrawBorder(float xCentre, float zCentre, int width, int height, Tile[] tiles) {
        //getting the corner tile sprites
        Tile[] corners = new Tile[4];
        corners[0] = tiles[1];
        corners[1] = tiles[2];
        corners[2] = tiles[5];
        corners[3] = tiles[4];

        DrawHorizontal(xCentre, zCentre + (height / 2 - 2) , width - 4, tiles[3]);
        DrawHorizontal(xCentre, zCentre - (height / 2 - 3), width - 4, tiles[3]);
        DrawVertical(xCentre + (width / 2 - 2), zCentre, height - 5, tiles[0]);
        DrawVertical(xCentre - (width / 2 - 1), zCentre, height - 5, tiles[0]);
        DrawCorners(xCentre, zCentre, width - 4, height - 4, corners);
    }

    //Draws a decorative carpet on the rooms floor tiles
    public void DrawCarpet(float xCentre, float zCentre, int width, int height, Tile[] tiles) {
        //getting the corner tile sprites
        Tile[] corners = new Tile[4];
        corners[0] = tiles[0];
        corners[1] = tiles[1];
        corners[2] = tiles[3];
        corners[3] = tiles[2];

        DrawHorizontal(xCentre, zCentre + (height / 2 - 3), width - 6, tiles[4]);
        DrawHorizontal(xCentre, zCentre - (height / 2 - 4), width - 6, tiles[5]);
        DrawVertical(xCentre + (width / 2 - 3), zCentre, height - 7, tiles[7]);
        DrawVertical(xCentre - (width / 2 - 2), zCentre, height - 7, tiles[6]);
        DrawCorners(xCentre, zCentre, width - 6, height - 6, corners);
        DrawInside(xCentre, zCentre + 1, width - 6, height - 8, tiles[8]);
    }

    //helper method to draw corners for the carpet / border
    private void DrawCorners(float xCentre, float zCentre, int width, int height, Tile[] tiles) {
        decoration.SetTile(new Vector3Int((int)(xCentre - (width / 2) - 1), (int)(zCentre + (height / 2)), 0), tiles[0]);
        decoration.SetTile(new Vector3Int((int)(xCentre + (width / 2)), (int)(zCentre + (height / 2)), 0), tiles[1]);
        decoration.SetTile(new Vector3Int((int)(xCentre + (width / 2)), (int)(zCentre - (height / 2) + 1), 0), tiles[2]);
        decoration.SetTile(new Vector3Int((int)(xCentre - (width / 2) - 1), (int)(zCentre - (height / 2) + 1), 0), tiles[3]);
    }

    //helper method to draw inside of the carpet
    private void DrawInside(float xCentre, float zCentre, int width, int height, Tile tile) {
        for (int x = (int)(xCentre - width / 2f); x < (int)(xCentre + width / 2f); x++) {
            for (int z = (int)(zCentre - height / 2f); z < (int)(zCentre + height / 2f); z++) {
                decoration.SetTile(new Vector3Int(x, z, 0), tile);
            }
        }
    }

    //helper method to draw horizontal edges of the carpet / border
    private void DrawHorizontal(float xCentre, float zPoint, int width, Tile tile) {
        int leftSide = (int)(xCentre - (width / 2));
        for (int i = 0; i < width; i++) {
            decoration.SetTile(new Vector3Int(leftSide + i, (int)zPoint, 0), tile);
        }
    }

    //helper method to draw vertical edges of the carpet / border
    private void DrawVertical(float xPoint, float zCentre, int height, Tile tile) {
        int bottomSide = (int)(zCentre - (height / 2));
        for (int i = 0; i < height; i++) {
            decoration.SetTile(new Vector3Int((int)xPoint, bottomSide + i, 0), tile);
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
