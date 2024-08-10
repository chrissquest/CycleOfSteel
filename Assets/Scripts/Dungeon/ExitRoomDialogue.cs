using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitRoomDialogue : MonoBehaviour
{
    private bool dialogueShown = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!dialogueShown)
        {
            Player.getPlayer.playerUI.QueueDialogue(new[] { "The Core AI is close...", "[Go down the stairs to exit or continue forth for a challege.]" });
            dialogueShown = true;
        }
    }
}
