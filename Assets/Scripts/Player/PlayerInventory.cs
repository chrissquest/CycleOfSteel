using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public SpriteRenderer displayGunSprite;

    public Sprite slotSprite;
    public Sprite selectedSlotSprite;
    public GameObject[] UIInventorySlots;
    public GameObject[] inventory;
    public IDSystem idSystem;
    public int slotSelected;

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    public Gun getCurrentGun()
    {
        if (inventory[slotSelected] != null)
            return inventory[slotSelected].GetComponent<Gun>();
        else return null;
    }

    public void refreshDisplayItem()
    {
        // Change the display active item in the player's hand (pivot)
        if (inventory[slotSelected] != null)
            displayGunSprite.sprite = inventory[slotSelected].GetComponent<SpriteRenderer>().sprite;
        else
            displayGunSprite.sprite = null;
        // Ideally there would be a dedicated running animation without a gun, but I think holding nothing looks better than no hands lol
    }

    public void setSlot(int slot)
    {
        // Bug, slot swapping very fast while firing acts on a deleted game object... doesnt do anything though
        // Perhaps when input sets multiple slots in the same frame or something?
        // Can probably be fixed with a delay on laser if I ever implement that, not concerning enough to worry about and I've already made this code ugly enough lol
        if (slot != slotSelected)
        {
            // We have swapped a slot, effects on previous slot below
            if (getCurrentGun() != null)
            {
                // Let the gun know it is no longer equipped
                getCurrentGun().Dequipped();

                // If we are shooting, stop firing that gun (is the check for shooting good?
                //if (GetComponent<PlayerInput>().isLooking)
                getCurrentGun().stopFiring();
            }

            // When we slect a new slot, change the sprites so it looks selected
            UIInventorySlots[slotSelected].GetComponent<SpriteRenderer>().sprite = slotSprite;
            // Actually change slot selected (Changing our current gun)
            slotSelected = slot;
            UIInventorySlots[slotSelected].GetComponent<SpriteRenderer>().sprite = selectedSlotSprite;

            refreshDisplayItem();

            if (getCurrentGun() != null)
            {
                // Let the gun know it is equipped
                getCurrentGun().Equipped();

                // If we are shooting, start firing right away
                if (GetComponent<PlayerInput>().isLooking)
                    getCurrentGun().startFiring(transform);
            }

        }
    }

    public void InstantiateToSlot(GameObject prefab, int slot)
    {
        // Used in save loading so we don't drop the previous weapon or anything, destroy it
        if (inventory[slot] != null) Destroy(inventory[slot]);

        // Instantiate into slot as parent, reposition, change render order
        inventory[slot] = Instantiate(prefab, UIInventorySlots[slot].transform); // Create the game object
        inventory[slot].transform.position = UIInventorySlots[slot].transform.position; // Move item to slot position ( that's where the actual game object resides)
        Rect sprite = inventory[slot].GetComponent<SpriteRenderer>().sprite.textureRect;
        inventory[slot].transform.localScale = new Vector3(16f / sprite.width, 16f / sprite.height, 1f); // Auto scale to slot size
        inventory[slot].GetComponent<SpriteRenderer>().sortingLayerName = "HUD Items"; // Change render layer

        if (slot == slotSelected)
            inventory[slot].GetComponent<Gun>().EquippedNoUpdate();
    }

    public void GameObjectToSlot(GameObject item, int slot)
    {
        //if (inventory[slot] != null) Destroy(inventory[slot]);

        // Put item into slot, reparent to slot, reposition, change render order
        inventory[slot] = item;
        inventory[slot].transform.parent = UIInventorySlots[slot].transform;
        inventory[slot].transform.position = UIInventorySlots[slot].transform.position;
        Rect sprite = inventory[slot].GetComponent<SpriteRenderer>().sprite.textureRect;
        inventory[slot].transform.localScale = new Vector3(16f / sprite.width, 16f / sprite.height, 1f);
        inventory[slot].GetComponent<SpriteRenderer>().sortingLayerName = "HUD Items";

        if (slot == slotSelected)
            inventory[slot].GetComponent<Gun>().Equipped();
    }

    public void DropSlot(int slot)
    {
        // Reposition to rigidBody bottom, change render order
        Vector3 playerHalfHeight = new Vector3(0, (transform.GetComponent<CapsuleCollider2D>().size.y / 2f) - 0.1f, 0);
        inventory[slot].transform.position = transform.position - playerHalfHeight;
        inventory[slot].transform.localScale = new Vector3(1f, 1f, 1f); // Reset scale from shrinking into inventory slot
        inventory[slot].GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
        inventory[slot].GetComponent<Gun>().Dequipped();

        // Reparent to room if found, root if not
        // If the generator exists, we can get the current room
        GameObject currentRoom = player.playerData.getCurrentRoom();

        // If we can get the current room, we drop it there, otherwise it reparents to null (root in hierarchy
        if (currentRoom != null)
            inventory[slotSelected].transform.parent = currentRoom.transform;
        else
            inventory[slotSelected].transform.parent = null;
    }

    public void AddInventory(GameObject item)
    {
        // Basically the inventory icons is the same item, it never gets deleted, just reparented

        // From left to right check if there is an empty slot to put the item into first

        bool no_empty_slot = true;
        for (int empty_slot = 0; empty_slot < inventory.Length; empty_slot++)
            if (inventory[empty_slot] == null)
            {
                no_empty_slot = false;
                // Put item into slot
                GameObjectToSlot(item, empty_slot);
                refreshDisplayItem();
                break;
            }

        // If there is no empty slot, swap with selected slot
        if (no_empty_slot)
        {
            DropSlot(slotSelected);
            GameObjectToSlot(item, slotSelected);
            refreshDisplayItem();
        }

    }


    public void incrementSlot()
    {
        if (slotSelected + 1 >= inventory.Length) setSlot(0);
        else setSlot(slotSelected + 1);
    }

    public void decrementSlot()
    {
        if (slotSelected - 1 < 0) setSlot(inventory.Length - 1);
        else setSlot(slotSelected - 1);
    }
}
