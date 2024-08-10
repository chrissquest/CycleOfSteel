using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

public class Player : MonoBehaviour
{
    // Container for all player classes
    [NonSerialized]
    public PlayerData playerData;
    [NonSerialized]
    public PlayerInput playerInput;
    [NonSerialized]
    public PlayerUI playerUI;
    [NonSerialized]
    public PlayerAnimations playerAnimations;
    [NonSerialized]
    public PlayerInventory playerInventory;

    // Static reference for enemies or anything that may need it without a clear link

    public static Player getPlayer { get; private set; }

    void Awake()
    {
        if (getPlayer == null)
        {
            getPlayer = this;
        }
        else if (getPlayer != this)
        {
            Destroy(this);
            getPlayer = this;
        }

        playerData = GetComponent<PlayerData>();
        playerInput = GetComponent<PlayerInput>();
        playerUI = GetComponent<PlayerUI>();
        playerAnimations = GetComponent<PlayerAnimations>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    public float DistanceToPlayer(GameObject g)
    {
        return (g.transform.position - transform.position).magnitude;
    }

    public void DialogueEvent(PlayerEvent pEvent, string[] dialogue)
    {
        if (!getPlayer.playerData.eventsCompleted[(int)pEvent])
        {
            getPlayer.playerUI.QueueDialogue(dialogue);
            getPlayer.playerData.eventsCompleted[(int)pEvent] = true;
        }
    }

    public void TeleportDungeon()
    {
        DungeonGenerator dung = getPlayer.playerData.generatorRef.GetComponent<DungeonGenerator>();
        Vector3 spawnPos = dung.getRoom(dung.spawnCoord).transform.position + new Vector3(RoomInfo.halfwidth - 3, RoomInfo.halfheight + 2, 0);
        getPlayer.transform.position = spawnPos;
        Camera.main.transform.position = spawnPos;
    }

    public void TeleportHub()
    {
        DungeonGenerator dung = getPlayer.playerData.generatorRef.GetComponent<DungeonGenerator>();
        Vector3 hubPos = new Vector3(-30.203f, 7.2489f, 0);
        getPlayer.transform.position = hubPos;
        Camera.main.transform.position = hubPos;
    }

}
