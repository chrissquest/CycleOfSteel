using UnityEngine;
using TMPro;
using Coordinate;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public enum PlayerEvent // Made so i can reference events by a name instead of pure numbers
{
    Intro,
    Secret,
    Secret2,
    GunPickup,
    Desk,
    ShootButton,
    PreBoss
}
public class PlayerData : MonoBehaviour
{
    // This file is way to big, but it takes long to organize it so it's not really worth to do right now
    // Could make a container script, "Player" Which holds references to PlayerInput, PlayerData, PlayerStats, etc...
    // Just not a priority so I won't mess with it lol, welcome to the longggg script
    // ctrl+f "Section" for sections

    public GameObject generatorRef;

    public Coord roomCoord;
    private Coord lastRoomCoord; // Used to know when player enters a new room
    public float maxHealth;
    public float health;
    public float moveSpeed;
    public float critChance;
    public float damagedInvulnerableTime;
    private float lastDamageTime;
    public int dataCells;
    public bool isDead;
    public float difficulty;

    // Move out of here (UI? or PlayerEffects class that im planning on making for chilling and burning)
    public Material defaultMaterial;
    public Material flashMaterial;

    // Move to PlayerSounds
    public AudioSource playerHurt;
    public AudioSource playerHeal;
    public AudioSource playerPickup;

    // Shooting visuals, could be in different class like PlayerVisuals
    public Sprite playerSprite;
    public Sprite playerSpriteNoHand;
    public GameObject leftHandSprite;
    public GameObject rightHandSprite;

    // Dialogue events completed
    public bool[] eventsCompleted;

    public HubRespawn hubRespawn;

    // Reference to all other player component classes, for organization's sake
    public Player player;

    private void Start()
    {
        player = transform.GetComponent<Player>();

        // Load data from file
        LoadData();
    }



    private void Update()
    {
        // Update room Coordinate
        roomCoord.x = (int)Math.Floor(transform.position.x / RoomInfo.width);
        roomCoord.y = (int)Math.Floor(transform.position.y / RoomInfo.height);

        if(lastRoomCoord != roomCoord )
        {
            // We have entered a new room
            GameObject currentRoom = getCurrentRoom();
            if (currentRoom != null)
            {
                // Tell the room we entered
                RoomScript currentRoomScript = currentRoom.GetComponent<RoomScript>();
                currentRoomScript.PlayerEnteredRoom(this);
                // Update the map when player enters a room
                player.playerUI.mapMenu.PlayerEnteredRoom(currentRoomScript.roomCoord);
                // If it's a new room, teleport the player in the direction we entered... mmm math
                if (currentRoomScript.NewRoom())
                {
                    Coord diff = roomCoord - lastRoomCoord;
                    transform.position += new Vector3(diff.x, diff.y, 0) * 0.75f;
                }
            }

            // Reset variables
            lastRoomCoord = roomCoord;
        }
    }

    // Section save data
    public void SaveData(bool ifSaveInventory)
    {
        PlayerSaveData data = new PlayerSaveData();
        data.maxHealth = maxHealth;
        data.health = health;
        data.moveSpeed = moveSpeed;
        data.critChance = critChance;
        data.dataCells = dataCells;
        data.slotSelected = player.playerInventory.slotSelected;
        data.difficulty = difficulty;

        if (ifSaveInventory)
        {
            GameObject[] inv = player.playerInventory.inventory;
            for (int i = 0; i < inv.Length; i++)
            {
                if (inv[i] != null)
                {
                    data.inventoryIDs[i] = inv[i].GetComponent<Gun>().ID;
                }
            }
        } else
        {
            PlayerSaveData dataOld = new PlayerSaveData().Load();
            if(dataOld != null)
                data.inventoryIDs = dataOld.inventoryIDs;
            else
            {
                data.inventoryIDs[0] = 0;
                data.inventoryIDs[1] = -1;
                data.inventoryIDs[2] = -1;
                data.inventoryIDs[3] = -1;
            }
        }


        data.upgradeMenuLevels[0] = player.playerUI.upgradeMenu.critLevel;
        data.upgradeMenuLevels[1] = player.playerUI.upgradeMenu.hpLevel;
        data.upgradeMenuLevels[2] = player.playerUI.upgradeMenu.moveLevel;

        data.eventsCompleted = eventsCompleted;

        data.Save();
    }


    public void LoadData()
    {
        PlayerSaveData data = new PlayerSaveData().Load();
        GameObject[] gameItems = player.playerInventory.idSystem.gameItems;
        // If there was a save found
        if (data != null)
        {
            maxHealth = data.maxHealth;
            player.playerUI.healthBar.maxValue = maxHealth;
            SetHealth(data.health);
            moveSpeed = data.moveSpeed;
            critChance = data.critChance;
            setDataCells(data.dataCells);
            player.playerInventory.setSlot(data.slotSelected);
            // Ahh god I would redo saves a lot if time permitted but that would definitely cause bugs at first
            // In the future, have save versions so if a future format is incompatible, i could just force a new save right away
            // then, make every list of items store it's own length so every addition wouldnt break everything
            // and make it identify if there was no data(like below) for every piece of data, and set to defaults if it's an unexpected value.
            if (data.difficulty != 0)
                difficulty = data.difficulty;
            else difficulty = 1f;


            GameObject[] inv = player.playerInventory.inventory;
            // Initialize inventory from save data
            for (int i = 0; i < data.inventoryIDs.Length; i++)
            {
                if (inv[i] != null)
                {
                    Destroy(inv[i]);
                    inv[i] = null;
                }
                if (data.inventoryIDs[i] != -1 && gameItems[data.inventoryIDs[i]] != null)
                    player.playerInventory.InstantiateToSlot(gameItems[data.inventoryIDs[i]], i);
            }


            player.playerInventory.refreshDisplayItem();

            player.playerUI.upgradeMenu.critLevel = data.upgradeMenuLevels[0];
            player.playerUI.upgradeMenu.hpLevel = data.upgradeMenuLevels[1];
            player.playerUI.upgradeMenu.moveLevel = data.upgradeMenuLevels[2];

            eventsCompleted = data.eventsCompleted;
        }
        else
        {
            // There was no save data, start with default values
            player.playerUI.healthBar.maxValue = maxHealth;
            FullHealPlayer();
            moveSpeed = 10f;
            critChance = 0f;
            player.playerInventory.slotSelected = 0;
            difficulty = 1f;

            // Initialize inventory
            // Starter pistol
            player.playerInventory.InstantiateToSlot(gameItems[0], 0);

            // Keep up to date with number of events
            eventsCompleted = new bool[Enum.GetNames(typeof(PlayerEvent)).Length];

            player.playerInventory.refreshDisplayItem();
        }
    }

    //Section actions, accessors to let the player know to do something

    public void Flash(float duration)
    {
        GetComponent<SpriteRenderer>().material = flashMaterial;
        player.playerUI.flashScreenUI.SetActive(true);
        Invoke("endFlash", duration);
    }

    private void endFlash()
    {
        GetComponent<SpriteRenderer>().material = defaultMaterial;
        player.playerUI.flashScreenUI.SetActive(false);
    }

    public void DamagePlayer(float dmg, Vector2 knockback)
    {
        if (Time.time - lastDamageTime > damagedInvulnerableTime)
        {
            // Damage rigidBody and update UI
            health -= dmg;
            player.playerUI.healthBar.value = health;
            playerHurt.Play();
            // Maybe display damage against player somehow? purple damage number or something?

            GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);

            // Flash white
            Flash(0.25f);

            if (health <= 0)
            {
                Die();
            }

            lastDamageTime = Time.time;
        }
    }

    public void HealPlayer(float hp)
    {
        // Heal rigidBody and update UI
        health += hp;
        playerHeal.Play();

        if (health > maxHealth)
            health = maxHealth;

        player.playerUI.healthBar.value = health;
    }

    // This is used when upgrading the players health
    public void AddMaxHealth(float hp)
    {
        maxHealth += hp;
        player.playerUI.healthBar.maxValue = maxHealth;
        FullHealPlayer();
    }

    // this is internally used to heal and stuff
    public void SetHealth(float hp)
    {
        health = hp;

        if (health > maxHealth)
            health = maxHealth;

        player.playerUI.healthBar.value = health;

        if (health <= 0)
        {
            Die();
        }
    }

    public void FullHealPlayer()
    {
        health = maxHealth;
        player.playerUI.healthBar.value = health;
    }

    public void Die()
    {
        isDead = true;
        // Player has died
        player.playerUI.pauseMenu.Pause();
        player.playerUI.deathMenu.SetActive(true); // Should probably standardize how menus work but got no time for that 
        player.playerUI.deathMenu.GetComponent<DeathMenu>().Died();

        StartCoroutine(RespawnRoutine());
    }

    public void Respawn()
    {
        if (isDead)
        {
            isDead = false;

            player.playerUI.deathMenu.SetActive(false);
            player.playerUI.bossBar.SetActive(false);
            player.playerUI.pauseMenu.Resume();
            // Save any data cells you got
            FullHealPlayer();
            SaveData(false);
            LoadData();
            //SceneManager.LoadScene("Hub");
            getGenerator().Init();
            player.TeleportHub();
            hubRespawn.StartAnim();
        }
    }
    public IEnumerator RespawnRoutine()
    {
        yield return new WaitForSecondsRealtime(5);
        Respawn();
    }

    public void AddBossBar(Killable k)
    {
        player.playerUI.bossBar.SetActive(true);
        player.playerUI.bossBar.GetComponent<BossBar>().SetKillable(k);
    }

    public void RemoveBossBar()
    {
        player.playerUI.bossBar.SetActive(false);
    }

    // Section stats

    public void incrementDataCells(int count)
    {
        dataCells += count;
        playerPickup.Play();
        player.playerUI.dataCellsText.text = dataCells.ToString();
    }

    public void setDataCells(int count)
    {
        dataCells = count;
        player.playerUI.dataCellsText.text = dataCells.ToString();
    }

    public void addMoveSpeed(float ms)
    {
        moveSpeed += ms;
    }

    private float normalMoveSpeed;
    public void stunMovement(float seconds)
    {
        // Save how much movespeed the player had normally, set it to 0 during the stun duration
        normalMoveSpeed = moveSpeed;
        moveSpeed = 0;
        Invoke("revertMovement", seconds);
    }

    public void revertMovement()
    {
        moveSpeed = normalMoveSpeed;
    }

    public void addCritChance(float crit)
    {
        critChance += crit;
    }

    public GameObject getCurrentRoom()
    {
        if (generatorRef != null) return generatorRef.GetComponent<DungeonGenerator>().getRoom(roomCoord);
        else return null;
    }

    public DungeonGenerator getGenerator()
    {
        if (generatorRef != null)
        {
            return generatorRef.GetComponent<DungeonGenerator>();
        }
        else return null;
    }

}
