using Coordinate;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System.Reflection;

public enum AttackState
{
    NONE,
    BULLET_SPIRAL, // This one looks really cool, easy to dodge by just going far away
    BULLET_SPIRAL_8,
    LASER_SPIRAL,
    BULLET_HOMING, // I like the state of this one for phase 2, make an easier version for phase 1
    NECROMANCY,
    LASER_DELAYED
}

public class BossAI : MonoBehaviour
{
    public float moveSpeed;

    public GameObject staffOrigin;
    public GameObject bulletFab;
    public GameObject farBulletFab;
    public GameObject homingBulletFab;
    public float bulletDamage;
    public float spiralBulletInterval;
    private float spiralBulletIntervalEnd;
    public float spiralBulletEightInterval;
    private float spiralBulletEightIntervalEnd;
    public float homingBulletInterval;
    private float homingBulletIntervalEnd;

    // Private quick references
    private Player player;
    private Rigidbody2D rigidBody;
    private Coord roomCoord;

    // Attack states
    AttackState currentAttack;


    private const int numHoming = 8;
    private float[] angles = new float[numHoming];
    void Start()
    {
        player = Player.getPlayer;
        rigidBody = GetComponent<Rigidbody2D>();
        // Room coordinate (If enemies dont move between rooms, we just need this once)
        roomCoord.x = (int)Math.Floor(transform.position.x / RoomInfo.width);
        roomCoord.y = (int)Math.Floor(transform.position.y / RoomInfo.height);

        spiralBulletIntervalEnd = 0;
        homingBulletIntervalEnd = 0;

        // Start Phase cycle timer
        currentAttack = AttackState.NONE;
        //StartLaserSpiral();

        StateLoopDelay();

        for (int i = 0; i < numHoming; i++)
        {
            angles[i] = (360f / numHoming) * i;
        }
    }

    void StateLoopDelay()
    {
        currentAttack = AttackState.NONE;
        StopLaserSpiral();
        Invoke("StateLoop", 2f);
    }

    void StateLoop()
    {
        // Also check health perhaps to determine which states are possible?
        // second phase could consist of faster versions of these states or ones not present normally

        // Choose state randomly
        int randomState = Random.Range(0, 4);
        switch (randomState)
        {
            case 0:
                currentAttack = AttackState.BULLET_HOMING;
                break;
            case 1:
                currentAttack = AttackState.BULLET_SPIRAL;
                break;
            case 2:
                currentAttack = AttackState.BULLET_SPIRAL_8;
                break;
            case 3:
                currentAttack = AttackState.LASER_SPIRAL;
                StartLaserSpiral();
                break;
        }


        Invoke("StateLoopDelay", 5f);
    }

    void Update()
    {
        // AI only moves if in the same room as rigidBody
        if(roomCoord == player.playerData.roomCoord)
        {
            switch (currentAttack)
            {
                case AttackState.BULLET_SPIRAL:
                    Center();
                    BulletSpiral();
                    break;
                case AttackState.BULLET_HOMING:
                    Follow();
                    BulletHoming();
                    break;
                case AttackState.BULLET_SPIRAL_8:
                    Center();
                    BulletSpiralEight();
                    break;
                case AttackState.LASER_SPIRAL:
                    Center();
                    UpdateLaserSpiral();
                    break;
            }

        }
    }

    void Center()
    {
        // target is room center
        Vector2 target = GetComponent<Killable>().GetRoom().GetPositionCenter() - new Vector2(transform.position.x, transform.position.y);
        rigidBody.AddForce(target.normalized * moveSpeed);
    }

    void Follow()
    {
        // Direction enemy should target
        Vector2 target = player.transform.position - transform.position;
        rigidBody.AddForce(target.normalized * moveSpeed);
    }

    void BulletSpiral()
    {
        if(Time.time > spiralBulletIntervalEnd)
        {
            // Create a bullet with constant speed and set it's damage
            // From staff origin
            GameObject bullet = Instantiate(bulletFab, staffOrigin.transform.position, staffOrigin.transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 360) * Vector2.left * 2f;
            bullet.GetComponent<EnemyBullet>().Damage = bulletDamage;
            //bullet.transform.rotation = Quaternion.Euler(0, 0, 45);


            // Check spray gun for rotation logic

            spiralBulletIntervalEnd = Time.time + spiralBulletInterval;
        }
    }

    void BulletSpiralEight()
    {
        if (Time.time > spiralBulletEightIntervalEnd)
        {
            foreach (float a in angles)
                createBullet(a + Time.time * 35f, farBulletFab, 5f);

            // Check spray gun for rotation logic

            spiralBulletEightIntervalEnd = Time.time + spiralBulletEightInterval;
        }
    }

    void BulletHoming()
    {
        if (Time.time > homingBulletIntervalEnd)
        {
            foreach(float a in angles)
                createHomingBullet(a);

            homingBulletIntervalEnd = Time.time + homingBulletInterval;
        }
    }

    void createBullet(float angle, GameObject b, float speed)
    {
        GameObject bullet = Instantiate(b, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, angle) * Vector2.left * speed;
        bullet.GetComponent<EnemyBullet>().Damage = bulletDamage;
    }

    void createHomingBullet(float angle)
    {
        GameObject bullet = Instantiate(homingBulletFab, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, angle) * Vector2.left * 3f;
        bullet.GetComponent<HomingEnemyBullet>().Damage = bulletDamage;
    }

    // Laser Variables
    public GameObject laserFab;
    private GameObject[] lasers = new GameObject[4];

    void StartLaserSpiral()
    {
        for (int i = 0; i < lasers.Length; i++) 
            if (lasers[i] == null) lasers[i] = Instantiate(laserFab, transform);
    }

    void StopLaserSpiral()
    {
        for (int i = 0; i < lasers.Length; i++)
            if (lasers[i] != null) Destroy(lasers[i]);
    }

    void UpdateLaserSpiral()
    {
        for (int i = 0; i < lasers.Length; i++)
            if (lasers[i] != null) lasers[i].GetComponent<Laser>().UpdateLaser(Time.time * 25f + i * 90f);
    }

}


