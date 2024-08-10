using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitHub : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.getPlayer.playerData.SaveData(true);
            EnterDungeon();
        }
    }

    void EnterDungeon()
    {
        Player.getPlayer.TeleportDungeon();
    }

}
