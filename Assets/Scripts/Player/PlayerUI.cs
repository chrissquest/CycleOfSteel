using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    // Container referencing all UI elements
    // Menus
    public GameObject uiScripts;
    [NonSerialized] 
    public PauseMenu pauseMenu;
    public MapMenu mapMenu;
    public UpgradeMenu upgradeMenu;
    public GameObject deathMenu;

    // UI
    public Slider healthBar;
    public Slider heatBar;
    public TextMeshProUGUI dataCellsText;
    public GameObject flashScreenUI;
    public GameObject bossBar;

    public GameObject OpenIndicatorUp;
    public GameObject OpenIndicatorDown;
    public GameObject OpenIndicatorLeft;
    public GameObject OpenIndicatorRight;

    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;
    private Queue<string> dialogueQueue;
    public bool isShowingDialogue;
    // Could have a variable like showingMenu, and then pause menu and stuff can check
    // if there is a showing menu first, if there is it will do something, implicitly the esc key will back out of the menu being show
    // Then if we have for example the death menu open, pause menu could not open, or map menu wont open with the pause menu open
    // And if we have an options submenu open... it gets more complicated, not sure how to tackle that.

    // I don't know if this has issues with load order, i guess we'll see
    // For example if i ask for the mapmenu from a singleton in the start of another class?
    // Works for generator from testing
    private void Awake()
    {
        pauseMenu = uiScripts.GetComponent<PauseMenu>();
        mapMenu = uiScripts.GetComponent<MapMenu>();
        upgradeMenu = uiScripts.GetComponent<UpgradeMenu>();

        dialogueQueue = new Queue<string>();
    }

    public void ShowIndicator(Direction d)
    {
        if(d == Direction.UP)
            OpenIndicatorUp.SetActive(true);
        else if (d == Direction.DOWN)
            OpenIndicatorDown.SetActive(true);
        else if (d == Direction.LEFT)
            OpenIndicatorLeft.SetActive(true);
        else if (d == Direction.RIGHT)
            OpenIndicatorRight.SetActive(true);

        Invoke("HideIndicators", 0.5f);
    }

    private void HideIndicators()
    {
        OpenIndicatorUp.SetActive(false);
        OpenIndicatorDown.SetActive(false);
        OpenIndicatorLeft.SetActive(false);
        OpenIndicatorRight.SetActive(false);
    }

    public void QueueDialogue(string[] dialogue)
    {
        // Show first dialogue
        dialogueUI.SetActive(true);
        isShowingDialogue = true;
        foreach (string s in dialogue)
            dialogueQueue.Enqueue(s);
        dialogueText.text = dialogueQueue.Dequeue();
        //pauseMenu.Pause();
    }

    // From spacebar or click
    public void ProgressDialogue()
    {
        if(dialogueQueue.Count > 0)
        {
            // Show next dialogue in queue
            dialogueText.text = dialogueQueue.Dequeue();
        }
        else
        {
            // If no more remaining just close it
            dialogueUI.SetActive(false);
            isShowingDialogue = false;
            //pauseMenu.Resume();
        }
    }

}
