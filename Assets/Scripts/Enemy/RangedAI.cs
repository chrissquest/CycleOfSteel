using Coordinate;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class RangedAI : MonoBehaviour
{
    [SerializeField] LayerMask LayersToHit;

    public float followRange;
    public float attackRange;

    public GameObject bulletFab;

    public float moveSpeed;

    private Player player;
    private Rigidbody2D enemy;

    private Vector2 target;
    private Vector2 targetGrav;
    private float lastWanderTime = 0;
    public float wanderInterval; 
    private float lastFireTime = 0;
    public float fireInterval;
    public float damage;

    private float spottingTimer = 0f;
    public float spottingLength = 5f;

    private Coord roomCoord;

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
            if (isInAttackRange() && spottingTimer > 0)
            {
                if (Time.time - lastFireTime > fireInterval / player.playerData.difficulty)
                {
                    Attack();
                    lastFireTime = Time.time;
                }
                PlayerInLineOfSight();
                spottingTimer -= Time.deltaTime;
            }
            else if (isInFollowRange() && spottingTimer > 0)
                Follow();
            else
                Wander();
        }
    }

    bool PlayerInLineOfSight()//CHECK IF THE RAYCAST ACTUALLY HITS THE PLAYER OR NOT AND RETURN BOOL
    {
        Vector2 rayTarget = player.transform.position - transform.position;
        Quaternion quatRot = Quaternion.FromToRotation(Vector3.right, rayTarget.normalized);
        Vector3 faceTarget = quatRot * transform.right;//to figure out direction of raycast?

        RaycastHit2D hit = Physics2D.Raycast(transform.position, faceTarget, 1000f, LayersToHit);
        if (hit.collider.gameObject.name == "Player")
        {
            spottingTimer = spottingLength;
            return true;
        }
        else
            return false;
    }

    void Wander()
    {
        // Random direction to walk towards
        // Every 10 frames choose a new direction
        if (Time.time - lastWanderTime > wanderInterval)
        {
            target = new Vector2(Random.Range(0, 3) - 1, Random.Range(0, 3) - 1);

            lastWanderTime = Time.time;
        }

        // Simplifies below movement by using the built in force system ;)
        // TODO: Do I need Time.deltaTime/ does that make it better or worse?
        enemy.AddForce(target.normalized * moveSpeed * Time.deltaTime);
        if(isInAttackRange() || isInFollowRange())
            PlayerInLineOfSight();
    }

    void Follow()
    {
        // Direction enemy should target
        target = player.transform.position - transform.position;
        enemy.AddForce(target.normalized * moveSpeed * Time.deltaTime);
        PlayerInLineOfSight();
        spottingTimer -= Time.deltaTime;
    }

    void Attack()
    {
        // Direction enemy should target
        target = player.transform.position - transform.position;
        //enemy.AddForce(target.normalized * moveSpeed * Time.deltaTime);
        Quaternion quatRot = Quaternion.FromToRotation(Vector3.right, -target.normalized);

        // Create a bullet with constant speed and set it's damage
        GameObject bullet = Instantiate(bulletFab, transform.position, quatRot);
        bullet.GetComponent<Rigidbody2D>().velocity = target.normalized * 3f;
        bullet.GetComponent<EnemyBullet>().Damage = damage;
    }

    private bool isInFollowRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < followRange;
    }
    private bool isInAttackRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < attackRange;
    }

}
