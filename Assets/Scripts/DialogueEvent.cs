using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

public class DialogueEvent : MonoBehaviour
{
    public PlayerEvent playerEvent;
    public string[] dialogue;
    public bool destroyOnEventTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.getPlayer.DialogueEvent(playerEvent, dialogue);
        }
    }

    private void Start()
    {
        // Has to be after player loads data... really not sure how to guarantee this
        // Only happens for the exit button tho
        Invoke("Check", 0.04f);
    }

    private void Check()
    {
        if (destroyOnEventTriggered && Player.getPlayer.playerData.eventsCompleted[(int)playerEvent])
            Destroy(gameObject);
    }

}
