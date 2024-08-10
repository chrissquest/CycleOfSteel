using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

public class DebugRoomExit : Interactable
{
    // Teleports player into gun debug room lol
    public override void OnInteract(Player player)
    {
        player.transform.position = player.transform.position + new Vector3(0, -2.5f, 0);
        Player.getPlayer.DialogueEvent(PlayerEvent.Secret2, new[] { "Farewell, you seem well equipped now." });
    }

}
