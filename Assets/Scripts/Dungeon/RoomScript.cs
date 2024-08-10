using TMPro;
using UnityEngine;
using Coordinate;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum RoomType
{
    SPAWN,
    RANDOM,
    EXIT,
    BOSS
}

public enum Direction
{
    NONE,
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class RoomScript : MonoBehaviour
{
    public Tilemap doorPlacementTileMap;
    public Tile doorHorzTop;
    public Tile doorHorzBot;
    public Tile doorVert;

    // Information about where walls/ doors are
    // For if you duplicate this room and customize it
    public bool doorTop;
    public bool doorBottom;
    public bool doorLeft;
    public bool doorRight;

    public bool doorsInverseGenerated;

    // All walls sealed by default
    // Possible to manually close a door, but I don't think there's a big reason to have this public now
    // true means there is a wall there
    protected bool top;
    protected bool bottom;
    protected bool left;
    protected bool right;

    public TextMeshProUGUI label;

    public RoomType roomType;

    [SerializeField]
    public Coord roomCoord;

    public int enemyCount;
    public List<Killable> enemies = new List<Killable>();

    public int distanceFromSpawn = 0;

    // All bools are false if generated, this is for making a room manually
    void Start()
    {

        if(!doorsInverseGenerated)
        {
            top = doorTop;
            bottom = doorBottom;
            left = doorLeft;
            right = doorRight;
        }
        else
        {
            top = !top;
            bottom = !bottom;
            left = !left;
            right = !right;
        }
    }

    // Ran in the generator when making a new room
    public void Init()
    {
        top = true;
        bottom = true;
        left = true;
        right = true;

        // Spawn in enemies
        //GetComponentInParent<EnemySpawner>().SpawnEnemies(this);

    }

    public void copyTo(ref RoomScript room)
    {
        room.top = top;
        room.bottom = bottom;
        room.left = left;
        room.right = right;
    }

    // Open door in cardinal direction
    public void CardinalOpen(Coord c)
    {
        if (c.x == -1) left = false;
        if (c.x == 1) right = false;
        if (c.y == -1) bottom = false;
        if (c.y == 1) top = false;
    }

    // Open door in opposite of cardinal direction
    public void CardinalOpenInverse(Coord c)
    {
        if (c.x == 1) left = false;
        if (c.x == -1) right = false;
        if (c.y == 1) bottom = false;
        if (c.y == -1) top = false;
    }

    public bool getTop() { return top; }
    public bool getBottom() { return bottom; }
    public bool getLeft() { return left; }
    public bool getRight() { return right; }

    public void AddEnemyCount(Killable k) { 
        enemyCount++;
        enemies.Add(k);
    }
    public void SubEnemyCount(Killable k) { 
        enemyCount--;
        enemies.Remove(k);
        if (enemyCount <= 0)
        {
            UnsealRoom();
            Player.getPlayer.playerData.getGenerator().ClearedRoom();
        }
    }

    // Called when player enters this room
    public void PlayerEnteredRoom(PlayerData player)
    {
        if (enemyCount > 0)
        {
            // Close the room doors till all enemies are killed
            SealRoom();
            // Let all enemies know a player entered (For bosses)
            foreach (Killable k in enemies)
                k.PlayerEnteredRoom(player);
        }
    }

    public bool NewRoom()
    {
        return enemyCount > 0;
    }

    public virtual void SealRoom()
    {
        if (top)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(15, 17, 0), doorHorzTop);
            doorPlacementTileMap.SetTile(new Vector3Int(16, 17, 0), doorHorzTop);
            doorPlacementTileMap.SetTile(new Vector3Int(15, 16, 0), doorHorzBot);
            doorPlacementTileMap.SetTile(new Vector3Int(16, 16, 0), doorHorzBot);
        }

        if (bottom)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(15, 0, 0), doorHorzTop);
            doorPlacementTileMap.SetTile(new Vector3Int(16, 0, 0), doorHorzTop);
        }

        if (left)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(0, 8, 0), doorVert);
            doorPlacementTileMap.SetTile(new Vector3Int(0, 9, 0), doorVert);
            doorPlacementTileMap.SetTile(new Vector3Int(0, 10, 0), doorVert);
        }

        if (right)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(31, 8, 0), doorVert);
            doorPlacementTileMap.SetTile(new Vector3Int(31, 9, 0), doorVert);
            doorPlacementTileMap.SetTile(new Vector3Int(31, 10, 0), doorVert);
        }
    }

    public void UnsealRoom()
    {
        if (top)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(15, 17, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(16, 17, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(15, 16, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(16, 16, 0), null);
            Player.getPlayer.playerUI.ShowIndicator(Direction.UP);
        }

        if (bottom)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(15, 0, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(16, 0, 0), null);
            Player.getPlayer.playerUI.ShowIndicator(Direction.DOWN);
        }

        if (left)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(0, 8, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(0, 9, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(0, 10, 0), null);
            Player.getPlayer.playerUI.ShowIndicator(Direction.LEFT);
        }

        if (right)
        {
            doorPlacementTileMap.SetTile(new Vector3Int(31, 8, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(31, 9, 0), null);
            doorPlacementTileMap.SetTile(new Vector3Int(31, 10, 0), null);
            Player.getPlayer.playerUI.ShowIndicator(Direction.RIGHT);
        }
    }

    // This i handled with extending this class bc i didnt wanna mess with generation, If I ever make this type of generation again it would be much cleaner...
    // This can be for Spawn and exit rooms to place walls instead of having the hallways lead to nowhere
    public void PlaceWalls()
    {
        // Maybe make a setTileRect helper function to place a section of walls to block off the hall
        // Or make 4 tilemaps and enable/disable the right ones
    }

    public Vector2 GetPositionCenter()
    {
        return new Vector2(roomCoord.x * RoomInfo.width + RoomInfo.halfwidth, roomCoord.y * RoomInfo.height + RoomInfo.halfheight);
    }

}
