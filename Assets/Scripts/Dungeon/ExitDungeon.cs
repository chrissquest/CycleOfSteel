using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class ExitDungeon : Interactable
{

    public override void OnInteract(Player player)
    {
         // Save inventory to file
         player.playerData.FullHealPlayer();
         player.playerData.SaveData(true);

        // TODO Fade away would be cool
        //SceneManager.LoadScene("Hub");
        // TODO remake how enterance looks,
        // perhaps have you teleport to the doorway so it's like you re-entered
        Player.getPlayer.TeleportHub();
        Player.getPlayer.playerData.getGenerator().LeftDungeon();
    }

}
