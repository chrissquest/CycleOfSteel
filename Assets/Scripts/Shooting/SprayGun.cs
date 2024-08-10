using System;
using UnityEngine;
using static System.Linq.Enumerable;

public class SprayGun : Gun
{
    // Properties
    public float fireDelay;
    public float bulletSpeed;
    public float damage;
    public float knockback;
    public int bulletCount;
    // In degrees
    public float sprayAngle;
    public float randomSpeed;

    public GameObject bulletFab;
    public Vector3 shake;

    // Mechanics
    private float lastFire;
    private AudioSource fireSound;

    new void Start()
    {
        base.Start();
        fireSound = GetComponent<AudioSource>();
    }

    public override void startFiring(Transform t)
    {
        base.startFiring(t);
    }

    public override void stopFiring()
    {
        base.stopFiring();
    }

    public override void updateFire(Transform t, Vector2 dir, Vector3 pos, Vector2 playerMove, float crit)
    {
        if (Time.time > heatCooldownFinished)
        {
            // If delay has passed, we can fire again
            // Gun should keep track of it's own delay, so no swapping abuse(?)
            if (Time.time - lastFire > fireDelay)
            {
                fire(dir, pos, playerMove, crit);
                lastFire = Time.time;

                // Can be moved to individual bullet if that would work better for anything?
                TickHeat();
            }
        }
    }

    private void fire(Vector2 dir, Vector3 pos, Vector2 playerMove, float crit)
    {
        Camera.main.GetComponent<CameraController>().Shake(shake.x, shake.y, shake.z);
        if (fireSound != null)
            fireSound.Play();

        // I really like this formatting for looping through an index of a count, apologies if it confuses anyone
        foreach (int i in Range(0, bulletCount))
        {
            fireSingleBullet(dir, pos, playerMove, crit);
        }

    }

    private void fireSingleBullet(Vector2 dir, Vector3 pos, Vector2 playerMove, float crit)
    {
        // Rotation from direction
        Quaternion quatRot = Quaternion.FromToRotation(Vector3.left, dir);
        // Random degree deviation
        Quaternion randomRot = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-sprayAngle, sprayAngle));
        Quaternion modifiedRot = quatRot * randomRot;
        Vector2 modifiedDir = modifiedRot * Vector2.left;
        float modifiedSpeed = bulletSpeed * UnityEngine.Random.Range(1f, randomSpeed);
        

        // Create a bullet with constant speed and set it's damage
        GameObject bullet = Instantiate(bulletFab, pos, modifiedRot);
        bullet.GetComponent<Rigidbody2D>().velocity = playerMove + modifiedDir * modifiedSpeed;
        // Chance to hit a crit
        if (crit >= UnityEngine.Random.Range(0f, 1f))
            bullet.GetComponent<BulletScript>().isCrit = true;

        bullet.GetComponent<BulletScript>().Damage = damage;
        bullet.GetComponent<BulletScript>().Knockback = knockback;
    }
}
