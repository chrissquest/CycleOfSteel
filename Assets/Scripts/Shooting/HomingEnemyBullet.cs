using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingEnemyBullet : MonoBehaviour
{
    [NonSerialized] public float Damage; 
    //[NonSerialized] public float velocity;
    public float TimeToLive;
    public GameObject Splash;

    void Start()
    {
        Destroy(gameObject, TimeToLive);
    }

    void Update()
    {
        Vector2 target = Player.getPlayer.transform.position - transform.position;
        GetComponent<Rigidbody2D>().AddForce(target.normalized * 250 * Time.deltaTime);
        transform.rotation = Quaternion.FromToRotation(Vector3.left, target.normalized);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerData>().DamagePlayer(Damage, Vector2.zero);
            DestroyBullet();
        }
        else if(collision.tag == "Enemy" || collision.tag == "Interactable" || collision.tag == "Bullet")
        {
            // Do nothing
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
