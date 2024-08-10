using Coordinate;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{

    // Component references
    private Rigidbody2D rigidBody;
    private PlayerData playerData;
    // Input logic
    private Vector2 moveVect;
    public Vector2 lookVect;
    [NonSerialized]
    public bool isLooking;

    // Need to do player pickup logic not from item because input only calls to one copy of object with PlayerInput
    //private GameObject pickupableItem;
    private List<GameObject> interactables;

    // Visualize gun rotation
    public Texture2D cursorTexture;

    // Reference to all other player component classes
    [NonSerialized]
    public Player player;

    void Start()
    {
        interactables = new List<GameObject>();

        player = GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();

        Cursor.SetCursor(cursorTexture, new Vector2(16f, 16f), CursorMode.Auto);
    }

    void Update()
    {
        // Input still works when paused, could cause bugs (firing bullets while paused)
        if (!PauseMenu.isPaused && !player.playerUI.isShowingDialogue)
        {
            // Movement
            rigidBody.AddForce(moveVect * playerData.moveSpeed * Time.deltaTime * 100f);

            // Actively shooting
            if(isLooking)
            {
                if(player.playerInventory.getCurrentGun() != null)
                {
                    // shootPos is at the pivot, and a little bit forward towards direction of shot
                    Vector3 shootPos = player.playerAnimations.pivot.transform.position + (new Vector3(lookVect.x, lookVect.y, 0f) * 0.35f);
                    // Inform the gun that it should be shot
                    // This is called repeatedly because we are in Update, See @ToggleFiring for Starting and Stopping calls
                    player.playerInventory.getCurrentGun().updateFire(transform, lookVect, shootPos, rigidBody.velocity, playerData.critChance);

                    player.playerAnimations.AnimateUpdateShooting(moveVect, lookVect);
                }
            }
            // Not actively shooting
            else
            {
                player.playerAnimations.AnimateUpdate(moveVect);
            }
        }
    }

    // Toggle if we are firing or not
    public void ToggleFiring(bool toggle)
    {
        if (!PauseMenu.isPaused)
        {
            if (toggle)
            {
                isLooking = true;
                // Toggle gun (needed for laser)
                if (player.playerInventory.getCurrentGun() != null)
                {
                    player.playerInventory.getCurrentGun().startFiring(transform);
                }
            }
            else
            {
                isLooking = false;
                if (player.playerInventory.getCurrentGun() != null)
                    player.playerInventory.getCurrentGun().stopFiring();
            }
        }
    }

    // Message from pause menu that game has resumed
    // Fixes issue of shooting continuing after pausing while shootign and unpausing
    // This interrupts shots when unpausing but I think it's very acceptable
    public void OnResumeGame()
    {
        ToggleFiring(false);
    }

    // Updates the direction we are moving when key is pressed
    public void OnMove(InputValue value) 
    {
        moveVect = value.Get<Vector2>();

        player.playerAnimations.AnimateOnMove(moveVect);
    }

    // Updates direction we are looking when key is pressed
    // and fires a bullet out (on cooldown)
    // This is arrow key/controller input
    // lookVect condenses mouse direction input and keyboard/controller input into one vec2
    public void OnLook(InputValue value)
    {
        lookVect = value.Get<Vector2>().normalized;
        // If we are looking any direction, we are inputting, thus firing
        ToggleFiring(lookVect != Vector2.zero);
    }

    // Only need this because OnClick doesnt know mouse position?
    private Vector2 mouseVect;

    public void OnPoint(InputValue value)
    {
        if(!player.playerUI.pauseMenu.isTouchEnabled)
        {
            mouseVect = value.Get<Vector2>();

            // Constantly update lookVect if we are clicking
            if (isLooking)
                lookVectMouseUpdate(value.Get<Vector2>());
        }
    }

    public void OnClick(InputValue value)
    {
        if (!player.playerUI.pauseMenu.isTouchEnabled)
        {
            // Look/ fire when we are clicking
            ToggleFiring(value.isPressed);
            // Send initial click position
            lookVectMouseUpdate(mouseVect);
        }
    }
    private void lookVectMouseUpdate(Vector2 screenVect)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenVect);
        worldPoint.z = 0;

        // Aim from the middle of the pivot
        lookVect = (worldPoint - player.playerAnimations.pivot.transform.position).normalized;
    }

    public void OnRightClick(InputValue value)
    {

    }

    void OnDebug()
    {
        // F5, debug key to skip hub and regenerate dungeon
        if (playerData.generatorRef != null)
            playerData.generatorRef.GetComponent<DungeonGenerator>().OnDebug();
        else
        {
            SceneManager.LoadScene("Dungeon");
        }
    }

    void OnDebug2()
    {
        // F6, teleport to pre boss room
        if (playerData.generatorRef != null)
        {
            Coord exit = playerData.generatorRef.GetComponent<DungeonGenerator>().exitCoord;
            transform.position = new Vector3(RoomInfo.width * exit.x + RoomInfo.halfwidth, RoomInfo.height * exit.y + RoomInfo.halfheight, 0);
        }
    }

    // Hotbar numbers
    public void OnNum1() { if (!PauseMenu.isPaused) player.playerInventory.setSlot(0); }
    public void OnNum2() { if (!PauseMenu.isPaused) player.playerInventory.setSlot(1); }
    public void OnNum3() { if (!PauseMenu.isPaused) player.playerInventory.setSlot(2); }
    public void OnNum4() { if (!PauseMenu.isPaused) player.playerInventory.setSlot(3); }

    // Could also add scrolling but that can be later (increment slot, decrement slot)

    public void OnScroll(InputValue value)
    {
        if (!PauseMenu.isPaused)
        {
            Vector2 scrollVal = value.Get<Vector2>();

            if (scrollVal.y > 0) player.playerInventory.decrementSlot();
            else if (scrollVal.y < 0) player.playerInventory.incrementSlot();
        }
    }

    void OnScrollInc()
    {
        player.playerInventory.incrementSlot();
    }

    void OnScrollDec()
    {
        player.playerInventory.decrementSlot();
    }

    // Passing on input to systems that use it
    public void OnEscape()
    {
        player.playerUI.pauseMenu.OnEscape();
    }

    public void OnMap()
    {
        if (!PauseMenu.isPaused)
            player.playerUI.mapMenu.OnMap();
    }

    // Interact key, default Space bar
    // More complex than just passing on input
    public void OnInteract()
    {
        if (!PauseMenu.isPaused)
        {
            // If there is a dialogue, that takes priority over pickups.
            if (player.playerUI.isShowingDialogue)
            {
                player.playerUI.ProgressDialogue();
            }

            // If there is an interactable, call it's OnInteract() function
            else if (interactables.Count > 0)
            {
                // Interact with the closest interactable
                GameObject closestInteractable = interactables.First();
                foreach (GameObject interactable in interactables)
                {
                    if (player.DistanceToPlayer(interactable) < player.DistanceToPlayer(closestInteractable))
                        closestInteractable = interactable;
                }

                closestInteractable.GetComponent<Interactable>().OnInteract(player);
            }
        }
    }

    // Picking up Items
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Interactable")
        {
            interactables.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Interactable")
        {
            interactables.Remove(collision.gameObject);
        }
    }
}
