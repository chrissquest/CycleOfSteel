using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static PlayerData;

public class HubRespawn : MonoBehaviour
{
    public RuntimeAnimatorController anim;
    public GameObject playerView;

    void Start()
    {
        StartAnim();
    }

    public void StartAnim()
    {
        GetComponent<Animator>().runtimeAnimatorController = null;
        GetComponent<Animator>().runtimeAnimatorController = anim;
        Player.getPlayer.gameObject.transform.SetParent(transform);
        Invoke("FinishAnim", 1.5f);
    }

    void FinishAnim()
    {
        Player.getPlayer.gameObject.transform.SetParent(playerView.transform);
        // One time dialogue event
        Player.getPlayer.DialogueEvent(PlayerEvent.Intro, new[] { "The tyranny of the knights must end...", "The Core AI… their god must be destroyed. I must escape this factory using any means necessary." });
        // Maybe show new dialogues when player has died? could be fun
    }

}
