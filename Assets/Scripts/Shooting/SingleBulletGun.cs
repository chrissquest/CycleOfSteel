using System;
using Unity.Mathematics;
using UnityEngine;

public class SingleBulletGun : Gun
{
    // Properties
    public float fireDelay;
    public float bulletSpeed;
    public float damage;
    public float knockback;

    public GameObject bulletFab;
    // Time, Amplitude, Frequency as XYZ because it's more compact in the editor view
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
        // TODO move a bunch of this stuff up into gun
        // I think this is a better way to do timers, calculate when the timer will be finished and check if we are fast that time.
        if (Time.time > heatCooldownFinished)
        {
            // If delay has passed, we can fire again
            // Gun should keep track of it's own delay, so no swapping abuse(?)
            if (Time.time - lastFire > fireDelay)
            {
                fire(dir, pos, playerMove, crit);
                lastFire = Time.time;

                TickHeat();
            }
        }
    }



    void fire(Vector2 dir, Vector3 pos, Vector2 playerMove, float crit)
    {
        // Shake cam?
        Camera.main.GetComponent<CameraController>().Shake(shake.x, shake.y, shake.z);
        if(fireSound != null)
            fireSound.Play();
        // Rotation from direction
        Quaternion quatRot = Quaternion.FromToRotation(Vector3.right, -dir);

        // Create a bullet with constant speed and set it's damage
        GameObject bullet = Instantiate(bulletFab, pos, quatRot);
        bullet.GetComponent<Rigidbody2D>().velocity = playerMove + dir * bulletSpeed;
        // Chance to hit a crit
        if (crit >= UnityEngine.Random.Range(0f, 1f))
            bullet.GetComponent<BulletScript>().isCrit = true;

        bullet.GetComponent<BulletScript>().Damage = damage;
        bullet.GetComponent<BulletScript>().Knockback = knockback;
    }


}
