using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

public class DebugRoom : Interactable
{
    // Teleports player into gun debug room lol
    public override void OnInteract(Player player)
    {
        player.transform.position = player.transform.position + new Vector3(7, 0, 0);

        Player.getPlayer.DialogueEvent(PlayerEvent.Secret, new[] { "How did you know...", "Well, have at it" });
    }

}
