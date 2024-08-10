using Coordinate;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    public GameObject mapMenu;
    public PlayerData playerData;

    // Reference to full map from generator
    private GameObject[,] fullMap;
    // Image based map we create as we explore
    private GameObject[,] mapImages;
    private GameObject mapHighlightImage;
    public GameObject mapUnknown;
    public Sprite mapHighlight;
    public Sprite mapSpawn;
    public Sprite mapExit;
    public Sprite mapExplored;
    public Sprite mapBoss;

    private bool isShowing;


    // When keyboard input is pressed
    public void OnMap()
    {
        // If the menu is already showing and key is pressed, hide menu
        if (isShowing)
        {
            GetComponent<PauseMenu>().ResumeShowMenu();
            isShowing = false;
        }
        else
        {
            //GetComponent<PauseMenu>().Pause();
            isShowing = true;
            mapMenu.SetActive(true);
            // Get map data from player and display it
            //updateMap();
        }
    }

    public void HideMenu()
    {
        mapMenu.SetActive(false);
    }

    public void DungeonFinishedGeneration() {

        if(mapImages != null)
        {
            // Clear images (For if you regenerate a dungeon)
            foreach (GameObject image in mapImages)
                if (image != null) Destroy(image);
            Destroy(mapHighlightImage);
        } else
        {
            mapImages = new GameObject[DungeonGenerator.dungeonSize, DungeonGenerator.dungeonSize];
        }

        fullMap = playerData.getGenerator().roomGrid;


        // Initialize image map to spawn room and connected rooms
        Coord spawnCoord = playerData.getGenerator().spawnCoord;
        // Spawn room image
        mapImages[spawnCoord.x, spawnCoord.y] = spawnImage(spawnCoord);
        mapImages[spawnCoord.x, spawnCoord.y].GetComponent<SpriteRenderer>().sprite = mapSpawn;
        mapHighlightImage = spawnImage(spawnCoord);
        mapHighlightImage.GetComponent<SpriteRenderer>().sprite = mapHighlight;
        mapHighlightImage.GetComponent<SpriteRenderer>().sortingOrder = 1;
        // Check connected rooms and mark as unknown
        checkConnectedForUnknown(spawnCoord);
    }

    private void checkConnectedForUnknown(Coord roomCheck)
    {
        foreach (Coord cardinal in Coord.cardinals)
        {
            Coord dir = roomCheck + cardinal;
            // If the room exists, and has not already been marked
            if (fullMap[dir.x, dir.y] != null && mapImages[dir.x, dir.y] == null)
            {
                mapImages[dir.x, dir.y] = spawnImage(dir);
            }
        }
    }

    private GameObject spawnImage(Coord cord)
    {
        GameObject go = Instantiate(mapUnknown, mapMenu.transform.position, mapMenu.transform.rotation, mapMenu.transform);
        go.transform.localScale = new Vector3(270, 270, 0);
        // Magic numbers... :'(
        go.transform.localPosition = new Vector3(cord.x * 135 - (135 * 5), cord.y * 93 - (93 * 5), 0);
        return go;
    }

    public void PlayerEnteredRoom(Coord room)
    {
        if (fullMap[room.x, room.y] != null)
        {
            // Display proper image for this room
            if (fullMap[room.x, room.y].GetComponent<RoomScript>().roomType == RoomType.RANDOM)
                mapImages[room.x, room.y].GetComponent<SpriteRenderer>().sprite = mapExplored;
            else if (fullMap[room.x, room.y].GetComponent<RoomScript>().roomType == RoomType.EXIT)
                mapImages[room.x, room.y].GetComponent<SpriteRenderer>().sprite = mapExit;
            else if (fullMap[room.x, room.y].GetComponent<RoomScript>().roomType == RoomType.BOSS)
                mapImages[room.x, room.y].GetComponent<SpriteRenderer>().sprite = mapBoss;
            // Dont need to mark spawn since there's only one 

            // Mark surrounding rooms
            checkConnectedForUnknown(room);

            // Highlight which room the player is in
            mapHighlightImage.transform.position = mapImages[room.x, room.y].transform.position; //mapMenu.transform.position + new Vector3(room.x * 0.375f - 2, room.y * 0.25f - 1, 0);
        }
    }


    /*
    private void displayFullMap(GameObject[,] map)
    {
        // Clear images
        foreach (GameObject image in mapImages)
            if (image != null) Destroy(image);

        int index = 0;
        foreach (GameObject room in map)
        {
            if (room != null)
            {
                RoomScript roomScript = room.GetComponent<RoomScript>();

                mapImages[index] = Instantiate(mapUnknown, mapMenu.transform.position + new Vector3(roomScript.roomCoord.x * 0.375f - 2, roomScript.roomCoord.y * 0.25f - 1, 0), mapMenu.transform.rotation, mapMenu.transform);

                // For different icons eventually
                if (roomScript.roomType == RoomType.SPAWN)
                {
                    mapImages[index].GetComponent<Image>().sprite = mapSpawn;
                }
                else if (roomScript.roomType == RoomType.EXIT)
                {
                    mapImages[index].GetComponent<Image>().sprite = mapExit;
                }
                else if (roomScript.roomType == RoomType.RANDOM)
                {
                    //mapImages[index].GetComponent<Image>().sprite = mapSpawn;
                }
            }

            index++;
        }
    }
    */

}
