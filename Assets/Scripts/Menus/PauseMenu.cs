using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    // If we are currently paused
    public static bool isPaused;

    private Player player;

    public AudioMixer mixer;
    private float premutedVolGun;
    private float premutedVolPlayer;

    // Would go into options sub menu, but i dont care enough to make that
    public GameObject touchControls;
    public bool isTouchEnabled;

    void Start()
    {
        player = Player.getPlayer;
        pauseMenu.SetActive(false);
        isTouchEnabled = false;
    }

    public void PauseShowMenu()
    {
        pauseMenu.SetActive(true);
        GetComponent<MapMenu>().HideMenu();
        Pause();
    }

    public void ResumeShowMenu()
    {
        if (!player.playerData.isDead)
        {
            // Uhh ideally we would not have to do this, making other menus go away on resume (implied that the game is paused when said menu is open)
            // perhaps there would be some toggle variable like bool inMenu that stops pausing from happening
            pauseMenu.SetActive(false);
            GetComponent<UpgradeMenu>().HideMenu();
            GetComponent<MapMenu>().HideMenu();
            Resume();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        Camera.main.GetComponent<CameraController>().stopShake();
        // For looping gun sounds, not sure how I would pause the sounds of the laser gun but not the laser itself)
        // might wanna add everything cause footsteps are also an issue
        mixer.GetFloat("gunVolume", out premutedVolGun);
        mixer.SetFloat("gunVolume", -80f);
        mixer.GetFloat("playerVolume", out premutedVolPlayer);
        mixer.SetFloat("playerVolume", -80f);

        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        mixer.SetFloat("gunVolume", premutedVolGun);
        mixer.SetFloat("playerVolume", premutedVolPlayer);

        // Let the PlayerInput script know we have resumed game (prevents some bugs)
        player.playerInput.OnResumeGame();
    }

    public void Menu()
    {
        // Ideally I can save everything like the dungeon and enemy states, but this could prob be a "back to hub" button instead and just save data?
        player.playerData.SaveData(false);
        Resume();
        SceneManager.LoadScene("StartMenu");
    }

    public void Quit()
    {
        //player.playerData.SaveData(false);
        Application.Quit();
    }

    public void OnEscape()
    {
        if (isPaused)
            ResumeShowMenu();
        else
            PauseShowMenu();
    }

    public void TouchControls()
    {
        if(!isTouchEnabled)
        {
            isTouchEnabled = true;
            touchControls.SetActive(true);
        }
        else
        {
            isTouchEnabled = false;
            touchControls.SetActive(false);
        }
    }

}
