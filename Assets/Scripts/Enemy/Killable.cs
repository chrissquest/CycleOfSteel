using System;
using UnityEngine;
using UnityEngine.UI; // Needed to reference slider healthbar
using static System.Linq.Enumerable; // Range for loop


[Serializable]
public class Drop
{
    public GameObject drop;
    public float weight;
}


public class Killable : MonoBehaviour
{    
    public float maxHealth; // Health the killable begins with when brought into the world
    private float health; // Internal counter for health
    private bool alive; // Needed because if two damage sources hit on the same frame, could trigger death twice

    public Slider healthBar; // SliderUI healthbar that should display health

    public GameObject damageNumber;
    public GameObject damageNumberCrit;

    [SerializeField] public Drop[] lootPool;

    public Sprite damagedSprite;
    public GameObject deathObject;

    public bool explosive; // If this killable explodes on death
    public GameObject explosionPrefab; // Explosion prefab

    public bool countAsEnemy; // If we count this as an enemy required to be killed to clear the room
    public bool bossBar;
    public string enemyName;

    public void Start()
    {
        health = maxHealth;
        alive = true;

        // Let the room keep count of enemies (This will crash if you organize the enemy in a specific way... but there's enough error checking to be safe)
        if (safeToUpdateRoomCounter())
            transform.parent.parent.gameObject.GetComponent<RoomScript>().AddEnemyCount(this);
    }


    // Damage the killable, with a direction for knockback
    public void DamageKillable(float dmg, Vector2 knockback, bool isCrit)
    {
        health -= dmg;

        UpdateHealthBar(health, maxHealth);

        PopupDamageNumber(dmg, isCrit);

        GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);

        // If health is < 70%, show damaged sprite
        if (damagedSprite != null && health < maxHealth * 0.7f)
        {
            GetComponent<SpriteRenderer>().sprite = damagedSprite;
        }

        if (health <= 0 && alive)
        {
            Die();
            alive = false;
        }
    }

    // Refresh the killable healthbar
    public void UpdateHealthBar(float currentValue, float maxValue) 
    {
        if(healthBar != null)
            healthBar.value = currentValue / maxValue;
    }

    // Makes a little number pop up for damage
    public void PopupDamageNumber(float dmg, bool isCrit)
    {
        if (dmg > 0)
        {
            Vector3 posVariance = new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f), 0f);
            GameObject damageObject = Instantiate(isCrit ? damageNumberCrit : damageNumber, transform.position + posVariance, transform.rotation);
            damageObject.GetComponent<DamageNumberScript>().setNumber(dmg);
        }
    }


    public void Die()
    {
        if (explosive) // If explosive, create explosion
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        if(bossBar)
        {
            Player.getPlayer.playerUI.bossBar.SetActive(false);
        }

        // TODO make loot table better

        foreach (int i in Range(0, lootPool.Length))
        {
            if (lootPool[i] != null)
            {
                // Weight is from 0-100 for now
                if (lootPool[i].weight >= UnityEngine.Random.Range(0f, 100f))
                    Instantiate(lootPool[i].drop, transform.position, transform.rotation);
            }
        }

        // Create dead body (if we set one)
        if(deathObject != null)
            Instantiate(deathObject, transform.position, transform.rotation, transform.parent);

        // Update room counter
        if (safeToUpdateRoomCounter())
            transform.parent.parent.gameObject.GetComponent<RoomScript>().SubEnemyCount(this);

        Destroy(gameObject);
    }

    // Called when a player enters the room, useful for boss bar
    public void PlayerEnteredRoom(PlayerData player)
    {
        if(bossBar)
        {
            // Set a UI element to have a reference to this and enable it
            player.AddBossBar(this);
        }
    }

    public RoomScript GetRoom()
    {
        if (safeToUpdateRoomCounter())
            return transform.parent.parent.gameObject.GetComponent<RoomScript>();
        else
             return null;
    }

    // Uhhh just checking if the hierarchy format is right so we don't crash toooo much
    private bool safeToUpdateRoomCounter()
    {
        return countAsEnemy && transform.parent != null && transform.parent.gameObject.name == "Enemies" && transform.parent.parent != null;
    }

}
