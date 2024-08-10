using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float TimeToLive;
    public GameObject Splash;
    public bool EnemyPiercing = false;
    [NonSerialized] public float Damage;
    [NonSerialized] public float Knockback;
    [NonSerialized] public bool isCrit = false;


    void Start()
    {
        Destroy(gameObject, TimeToLive);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Interactable" || collision.tag == "Bullet" || collision.tag == "NonShootable") 
        { 
            // Do nothing
        }
        else if (collision.tag == "Enemy")
        {
            // Instead of die, damage with bullet damage
            Vector2 bulletDir = GetComponent<Rigidbody2D>().velocity.normalized;
            collision.gameObject.GetComponent<Killable>().DamageKillable(isCrit ? Damage * 2 : Damage, bulletDir * Knockback, isCrit);
            if (EnemyPiercing == false)
            {
                DestroyBullet();
            }

        }
        else // Anything else w collision, so walls and stuff
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        // Added a little delay because it looks better for hitting walls, otherwise it dissapears befor it hits... kinda scuffed but it works
        Destroy(gameObject, 0.02f);
        if (Splash != null)
        {
            GameObject splash = Instantiate(Splash, gameObject.transform.position, gameObject.transform.rotation);
            // Set splash color to bullet color
            splash.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        }
    }
}
