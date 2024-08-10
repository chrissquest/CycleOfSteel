using Coordinate;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class RangedAITurret : MonoBehaviour
{
    public float attackRange;

    public GameObject bulletFab;


    private Player player;
    private Rigidbody2D enemy;

    private Vector2 target;
    private Vector2 targetGrav;
    private float lastFireTime = 0;
    public float fireInterval;
    public float damage;

    private Coord roomCoord;

    //[SerializeField] private Transform bulletSpawnPoint_1;
    //[SerializeField] private Transform bulletSpawnPoint_2;

    public Transform[] bulletSpawnPoints;

    void Start()
    {
        enemy = GetComponent<Rigidbody2D>();
        // This is an expensive call, would be cool to replace it but not sure how to get a reference of the player
        // Doesn't matter a ton since it's just at the start of entering the dungeon
        player = Player.getPlayer;
        // Room coordinate (If enemies dont move between rooms, we just need this once)
        roomCoord.x = (int)Math.Floor(transform.position.x / RoomInfo.width);
        roomCoord.y = (int)Math.Floor(transform.position.y / RoomInfo.height);
    }

    void Update()
    {
        // AI only moves if in the same room as rigidBody
        if(roomCoord == player.playerData.roomCoord)
        {
            // Determine if enemy should be agro based on range
            if (isInAttackRange())
            {
                if (Time.time - lastFireTime > fireInterval / player.playerData.difficulty)
                {
                    Attack();
                    lastFireTime = Time.time;
                }
            }
        }
    }

    void Attack()
    {
        // Direction enemy should target

        //target = player.transform.position - transform.position;

        //enemy.AddForce(target.normalized * moveSpeed * Time.deltaTime);

       // Quaternion quatRot = Quaternion.FromToRotation(Vector3.right, -target.normalized);

        // Create a bullet with constant speed and set it's damage

        foreach(Transform BSP in bulletSpawnPoints)
        {
            GameObject bullet = Instantiate(bulletFab, BSP.position, BSP.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right;
            bullet.GetComponent<EnemyBullet>().Damage = damage;
        }


    }

    private bool isInAttackRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < attackRange;
    }

}
