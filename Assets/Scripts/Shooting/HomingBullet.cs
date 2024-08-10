using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HomingBullet : BulletScript
{
    public float homingPower = 500f;

    void Update()
    {
        if(Player.getPlayer.playerData.getCurrentRoom() != null)
        {
            // Get closet enemy to bullet
            // This seems perhaps inefficient, but it's simple and should just be non changing references so it's not that bad
            List<Killable> enemies = Player.getPlayer.playerData.getCurrentRoom().GetComponent<RoomScript>().enemies;
            if (enemies.Count > 0)
            {
                Killable closestEnemy = enemies.First();
                foreach (Killable enemy in enemies)
                    if (Player.getPlayer.DistanceToPlayer(enemy.gameObject) < Player.getPlayer.DistanceToPlayer(closestEnemy.gameObject))
                        closestEnemy = enemy;

                //Vector2 v = closestEnemy.GetComponent<Rigidbody2D>().velocity;
                // Target accounts for velocity?
                Vector2 target = closestEnemy.transform.position - transform.position;
                GetComponent<Rigidbody2D>().AddForce(target.normalized * 500f * Time.deltaTime);
                //GetComponent<Rigidbody2D>().velocity = target.normalized * 3f;
                transform.rotation = Quaternion.FromToRotation(Vector3.left, target.normalized);
            }
        }
    }

}
