using System.Collections.Generic;
using UnityEngine;
using Coordinate;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject openRoom;
    public GameObject spawnRoom;
    public GameObject exitRoom;
    public GameObject bossRoom;

    public GameObject[] roomDoorsNSEW;
    public GameObject[] roomDoorsNSE;
    public GameObject[] roomDoorsNEW;
    public GameObject[] roomDoorsNSW;
    public GameObject[] roomDoorsSEW;
    public GameObject[] roomDoorsNE;
    public GameObject[] roomDoorsNS;
    public GameObject[] roomDoorsNW;
    public GameObject[] roomDoorsSE;
    public GameObject[] roomDoorsEW;
    public GameObject[] roomDoorsSW;
    public GameObject[] roomDoorsN;
    public GameObject[] roomDoorsE;
    public GameObject[] roomDoorsS;
    public GameObject[] roomDoorsW;
    private GameObject[] roomDoorsNONE;

    // Grid of gameobjects, the actual dungeon
    public GameObject[,] roomGrid;

    // Temporary queues for generation of dungeon
    private Queue<Coord> endRooms;
    private Queue<Coord> nextEndRooms;


    // Dungeon initialization variables
    // Max grid size dungeon generates in, make sure it's large enough to generate rooms wanted
    public const int dungeonSize = 10;
    // Target amount of rooms to generate, keeps trying till it hits it or gives up
    public const int dungeonRooms = 8;
    // Spawn room in middle coordinate
    public Coord spawnCoord = new Coord(dungeonSize / 2, dungeonSize / 2);
    public Coord exitCoord;
    // Count of rooms made
    int roomsGenerated = 0;
    int layerGenerated = 0;

    // Reference to map to let know when done generating
    public MapMenu mapMenu;

    private int roomsCleared;

    public DungeonGenerator()
    {
        roomGrid = new GameObject[dungeonSize, dungeonSize];
    }

    // Ran once on start of scene, generates the dungeon
    void Start()
    {
        Init();
    }

    public void Init()
    {
        GenerateDungeon(dungeonSize, dungeonRooms, spawnCoord, ref roomsGenerated);
    }

    void GenerateDungeon(int dungeonSize, int dungeonRooms, Coord spawnCoord, ref int roomsGenerated)
    {
        layerGenerated = 0;
        // Coordinates of the end rooms 
        // Two lists needed because we loop over one and generate the next one while inside the loop
        endRooms = new Queue<Coord>();
        nextEndRooms = new Queue<Coord>();

        // Regenerate the whole dungeon
        for (int x = 0; x < dungeonSize; x++)
            for (int y = 0; y < dungeonSize; y++)
                if (roomGrid[x, y] != null)
                {
                    Destroy(roomGrid[x, y]);
                    roomGrid[x, y] = null;
                }

        // Starter room to generate off
        roomGrid[spawnCoord.x, spawnCoord.y] = Instantiate(spawnRoom, new Vector3(RoomInfo.width * spawnCoord.x, RoomInfo.height * spawnCoord.y, 0), Quaternion.identity, transform);
        roomsGenerated = 1;
        //  Initalize spawn room
        roomGrid[spawnCoord.x, spawnCoord.y].GetComponent<RoomScript>().Init();
        roomGrid[spawnCoord.x, spawnCoord.y].GetComponent<RoomScript>().roomType = RoomType.SPAWN;
        roomGrid[spawnCoord.x, spawnCoord.y].GetComponent<RoomScript>().roomCoord = spawnCoord;
        //roomGrid[spawnCoord.x, spawnCoord.y].GetComponent<RoomScript>().label.text = roomsGenerated.ToString();

        // Add to endRooms queue to generate more rooms off of spawn room
        endRooms.Enqueue(spawnCoord);


        // Stop infinite looping if more rooms cannot be spawned (usually runs into a loop)
        // About 7 tries is enough to make sure there is a < 1% chance that more rooms could not have been generated (with a 50% spawn chance if conditions are met (.5^7 = 0.00781) )
        int tries = 0;
        int maxTries = 7;

        // Keep trying to generate rooms till we get the amount dungeonRooms
        while (roomsGenerated < dungeonRooms && tries < maxTries)
        {
            GenerateRooms(dungeonSize, dungeonRooms, ref roomsGenerated, ref tries);
        }

        // Mark exit room
        // Replaces the last room with an exit room
        while(endRooms.Count > 0)
            exitCoord = endRooms.Dequeue();
        RoomScript exitRoomScript = roomGrid[exitCoord.x, exitCoord.y].GetComponent<RoomScript>();
        exitRoomScript.roomType = RoomType.EXIT;
        exitRoomScript.roomCoord = exitCoord;

        // Spawn boss room
        // Always connected to exit room so it's the furthest possible, but still connects to this room so there is a little break when you enter the preboss room
        // Can always spawn since there's no configuration where there's no space near the exit room (a room will not spawn if there another room already in a cardinal)
        // May touch other rooms but doesn't matter since this is the last room
        bool bossCreated = false;
        foreach (Coord dir in Coord.cardinals)
        {
            if(!bossCreated)
            {
                Coord bossCoord = exitCoord + dir;
                // If there's an empty space
                if (IsInGrid(bossCoord, dungeonSize) && roomGrid[bossCoord.x, bossCoord.y] == null)
                {
                    roomGrid[bossCoord.x, bossCoord.y] = Instantiate(bossRoom, new Vector3(RoomInfo.width * bossCoord.x, RoomInfo.height * bossCoord.y, 0), Quaternion.identity, transform);
                    RoomScript bossRoomScript = roomGrid[bossCoord.x, bossCoord.y].GetComponent<RoomScript>();
                    bossRoomScript.Init();
                    bossRoomScript.roomType = RoomType.BOSS;
                    bossRoomScript.roomCoord = bossCoord;
                    exitRoomScript.CardinalOpen(dir);
                    bossRoomScript.CardinalOpenInverse(dir);
                    bossRoomScript.SealRoom();
                    bossCreated = true;
                    roomsGenerated++;
                }
            }
        }



        RoomScript roomScript;
        // Seal up the rooms (Spawn in the walls with the information rooms already know)
        for (int x = 0; x < dungeonSize; x++)
            for (int y = 0; y < dungeonSize; y++)
                // If there is a room there
                if (roomGrid[x, y] != null) {
                    // If it's set to be randomized
                    RoomType roomType = roomGrid[x, y].GetComponent<RoomScript>().roomType;
                    if (roomType == RoomType.RANDOM) {
                        roomScript = roomGrid[x, y].GetComponent<RoomScript>();
                        spawnRandomRoom(roomScript, x, y);
                    }
                    else if (roomType == RoomType.SPAWN)
                    {
                        roomGrid[x, y].GetComponent<RoomScript>().SealRoom();
                    }
                    else if (roomType == RoomType.EXIT)
                    {
                        // Copy door layout to exit room
                        GameObject exitRoomObject = Instantiate(exitRoom, new Vector3(RoomInfo.width * x, RoomInfo.height * y, 0), Quaternion.identity, transform);
                        RoomScript exitScript = exitRoomObject.GetComponent<RoomScript>();
                        roomGrid[x, y].GetComponent<RoomScript>().copyTo(ref exitScript);
                        // Destroy layout room
                        Destroy(roomGrid[x, y]);
                        // Replace it with exit room now with info
                        roomGrid[x, y] = exitRoomObject;
                        roomGrid[x, y].GetComponent<RoomScript>().SealRoom();

                        // Gonna be honest, the above looks cursed but just gonna slide this in here so exit room coords are marked
                        roomGrid[x, y].GetComponent<RoomScript>().roomCoord = exitCoord;
                    }
                }

        // Let map menu know
        Player.getPlayer.playerUI.mapMenu.DungeonFinishedGeneration();
    }

    public void spawnRandomRoom(RoomScript roomScript, int x, int y)
    {
        // Depending on the room generated, slot in a proper room

        // Very large if chain for every possible combination of room types, not sure if there's a way around this, it's not inefficient, just ugly
        if (!roomScript.getTop()) // Top open
            if (!roomScript.getRight()) // Right open
                if (!roomScript.getBottom()) // Bottom open
                    if (!roomScript.getLeft()) // Left open // Completely open room
                        spawnRoomFromArray(x, y, roomDoorsNSEW);
                    else // Left closed
                        spawnRoomFromArray(x, y, roomDoorsNSE);
                else // Bottom closed
                    if (!roomScript.getLeft()) // Left open
                    spawnRoomFromArray(x, y, roomDoorsNEW);
                else // Left closed
                    spawnRoomFromArray(x, y, roomDoorsNE);
            else // Right closed
                if (!roomScript.getBottom()) // Bottom open
                if (!roomScript.getLeft()) // Left open
                    spawnRoomFromArray(x, y, roomDoorsNSW);
                else // Left closed
                    spawnRoomFromArray(x, y, roomDoorsNS);
            else // Bottom closed
                    if (!roomScript.getLeft()) // Left Open
                spawnRoomFromArray(x, y, roomDoorsNW);
            else // Left Closed
                spawnRoomFromArray(x, y, roomDoorsN);
        else // Top closed
            if (!roomScript.getRight()) // Right open
            if (!roomScript.getBottom()) // Bottom open
                if (!roomScript.getLeft()) // Left open
                    spawnRoomFromArray(x, y, roomDoorsSEW);
                else // Left closed
                    spawnRoomFromArray(x, y, roomDoorsSE);
            else // Bottom closed
                    if (!roomScript.getLeft()) // Left open
                spawnRoomFromArray(x, y, roomDoorsEW);
            else // Left closed
                spawnRoomFromArray(x, y, roomDoorsE);
        else // Right closed
                    if (!roomScript.getBottom()) // Bottom open
            if (!roomScript.getLeft()) // Left open
                spawnRoomFromArray(x, y, roomDoorsSW);
            else // Left closed
                spawnRoomFromArray(x, y, roomDoorsS);
        else // Bottom closed
                        if (!roomScript.getLeft()) // Left Open
            spawnRoomFromArray(x, y, roomDoorsW);
        else // Left Closed
            spawnRoomFromArray(x, y, roomDoorsNONE); // This should never be generated,
                                                     // if it does that means we have a 1 room dungeon and it would just seal a default room
    }

    public void spawnRoomFromArray(int x, int y, GameObject[] roomArr)
    {
        // Dont replace spawn room w random room
        if(x == spawnCoord.x && y == spawnCoord.y)
        {
            roomGrid[x, y].GetComponent<RoomScript>().PlaceWalls();
        }
        else if(roomArr != null && roomArr.Length > 0)
        {
            // Transfer data from Open Room to newly placed prefab room
            GameObject oldRoom = roomGrid[x, y];

            bool spawnedRoom = false;
            // ASSUMED ROOMS ARE IN ORDER FROM MOST DIFFICULT TO LEAST DIFFICULT, always have a difficulty 1 room available at least
            foreach (GameObject room in roomArr)
            {
                if (!spawnedRoom)
                {
                    // If a high difficulty room can spawn here, do it
                    // isnt necissarily high difficulty, can control that easy rooms spawn too
                    // basically at x distance, a room of difficulty x or lower can spawn
                    // so in a zone 1 away, 1 difficulty room will spawn
                    // in zone 2, 2 will spawn first, then 1 if there is no 2
                    //         Room to spawn                                      Zone distance
                    if (room.GetComponent<RoomScript>().distanceFromSpawn <= roomGrid[x, y].GetComponent<RoomScript>().distanceFromSpawn)
                    {
                        roomGrid[x, y] = Instantiate(room, new Vector3(RoomInfo.width * x, RoomInfo.height * y, 0), Quaternion.identity, transform);

                        spawnedRoom = true;
                    }
                }
            }
            // Spawn random room
            //roomGrid[x, y] = Instantiate(roomArr[Random.Range(0, roomArr.Length)], new Vector3(RoomInfo.width * x, RoomInfo.height * y, 0), Quaternion.identity, transform);
            roomGrid[x, y].GetComponent<RoomScript>().roomCoord = oldRoom.GetComponent<RoomScript>().roomCoord;
            roomGrid[x, y].GetComponent<RoomScript>().distanceFromSpawn = oldRoom.GetComponent<RoomScript>().distanceFromSpawn;
            // This should be set in the prefab
            //roomGrid[x, y].GetComponent<RoomScript>().roomType = oldRoom.GetComponent<RoomScript>().roomType;
            Destroy(oldRoom);
        }
        else roomGrid[x, y].GetComponent<RoomScript>().PlaceWalls();
    }

    void GenerateRooms(int dungeonSize, int dungeonRooms, ref int roomsGenerated, ref int tries)
    {
        // Go through each end room
        foreach (Coord endRoom in endRooms)
            // Check four cardinal directions from end room
            foreach (Coord cardinal in Coord.cardinals)
            {
                Coord possibleRoom = endRoom + cardinal;

                if (IsValidRoomLocation(possibleRoom, dungeonSize, dungeonRooms, roomsGenerated))
                {
                    // Random 50% chance
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        CreateRoom(possibleRoom, ref roomsGenerated, ref tries, cardinal, endRoom);
                    }
                }
            }

        // If there were rooms generated, those are now the furthest rooms, so only use those for generation
        // Could tweak this to higher value to be more lenient where we can gen from?
        if (nextEndRooms.Count >= 2)
        {
            endRooms.Clear();
            endRooms = new Queue<Coord>(nextEndRooms);
            layerGenerated++;
            foreach (Coord endRoom in endRooms)
                getRoom(endRoom).GetComponent<RoomScript>().distanceFromSpawn = layerGenerated;
            nextEndRooms.Clear();
        }
        else
        {
            foreach (Coord ends in nextEndRooms)
                endRooms.Enqueue(ends);
        }
        tries++;
    }

    private void CreateRoom(Coord room, ref int roomsGenerated, ref int tries, Coord cardinal, Coord roomFrom)
    {

        roomGrid[room.x, room.y] = Instantiate(openRoom, new Vector3(RoomInfo.width * room.x, RoomInfo.height * room.y, 0), Quaternion.identity, transform);
        roomsGenerated++;

        RoomScript roomGenScript = roomGrid[room.x, room.y].GetComponent<RoomScript>();
        roomGenScript.Init();
        roomGenScript.roomType = RoomType.RANDOM;
        roomGenScript.roomCoord = new Coord(room.x, room.y);

        //roomGrid[room.x, room.y].GetComponent<RoomScript>().label.text = roomsGenerated.ToString();

        // Inform the room which way it was generated from, opening it's door that way
        roomGenScript.CardinalOpenInverse(cardinal);
        // Inform the room we generated from which way this new room is
        roomGrid[roomFrom.x, roomFrom.y].GetComponent<RoomScript>().CardinalOpen(cardinal);


        // If we generated a room, put it in the nextEndRooms queue
        nextEndRooms.Enqueue(room);
        // Succesfully spawned a room, reset tries (for making sure 50% chance goes through)
        tries = 0;
    }

    private bool IsValidRoomLocation(Coord room, int dungeonSize, int dungeonRooms, int roomsGenerated)
    {
        // If there is no room already there, and we have not reached max rooms
        if (IsInGrid(room, dungeonSize) && roomGrid[room.x, room.y] == null && roomsGenerated < dungeonRooms)
        {
            // Check this room's neighbors, if there are less than 2, we can spawn it (prevents looping rooms)
            int neighborCount = 0;

            foreach (Coord cardinal in Coord.cardinals)
            {
                Coord neighbor = room + cardinal;
                if (IsInGrid(neighbor, dungeonSize) && roomGrid[neighbor.x, neighbor.y] != null) neighborCount++;
            }

            if (neighborCount <= 1)
                return true;
        }
        
        return false;
    }

    private bool IsInGrid(Coord c, int dungeonSize)
    {
        if (c.x >= 0 && c.x < dungeonSize && c.y >= 0 && c.y < dungeonSize) return true;
        else return false;
    }

    public GameObject getRoom(Coord room)
    {
        if(IsInGrid(room, dungeonSize) && roomGrid[room.x, room.y] != null)
            return roomGrid[room.x, room.y];
        else return null;
    }

    public void OnDebug()
    {
        GenerateDungeon(dungeonSize, dungeonRooms, spawnCoord, ref roomsGenerated);
    }

    public void ClearedRoom()
    {
        roomsCleared++;
    }

    public void LeftDungeon()
    {
        if (roomsCleared >= roomsGenerated - 2)
        {
            // Dungeon has been cleared
            // Generate a new one
            // Increase difficulty here
            Player.getPlayer.playerData.difficulty = 2f;

            Init();
        }
    }

}
