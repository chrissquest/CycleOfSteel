using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float TimeToLive;
    public float Damage;

    public AudioSource explosion;

    void Start()
    {
        explosion.Play();
        Destroy(gameObject, TimeToLive);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float maxDist = GetComponent<CircleCollider2D>().radius;
        Vector2 knockback = (collision.transform.position - transform.position) / maxDist;

        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerData>().DamagePlayer(Damage, knockback * 3); // Does damage to player
            // Maybe damage and crit based on distance?
        }
        else if(collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Killable>().DamageKillable(Damage, knockback * 3, true); // Does damage to enemy
        } 
    }

}
