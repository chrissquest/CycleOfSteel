using Coordinate;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class DashAI : MonoBehaviour
{

    public float followRange;

    public float moveSpeed;

    private GameObject player;
    private PlayerData playerScript;
    private Rigidbody2D enemy;

    private Vector2 target;
    private Vector2 targetGrav;
    private float lastWanderTime = 0;
    public float wanderInterval;

    private Coord roomCoord;

    // Movement Anim
    public RuntimeAnimatorController runRight;
    public RuntimeAnimatorController runLeft;
    public RuntimeAnimatorController runFront;
    public RuntimeAnimatorController runBack;

    void Start()
    {
        enemy = GetComponent<Rigidbody2D>();
        // This is an expensive call, would be cool to replace it but not sure how to get a reference of the player
        // Doesn't matter a ton since it's just at the start of entering the dungeon
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerData>();
        // Room coordinate (If enemies dont move between rooms, we just need this once)
        roomCoord.x = (int)Math.Floor(transform.position.x / RoomInfo.width);
        roomCoord.y = (int)Math.Floor(transform.position.y / RoomInfo.height);
    }

    void Update()
    {
        // AI only moves if in the same room as rigidBody
        if(roomCoord == playerScript.roomCoord)
        {
            // Determine if enemy should be agro based on range
            if (isPlayerInRange())
                Follow();
            else
                Wander();

            // Movement Animation
            float dirAngle = Vector2.Angle(new Vector2(0, 1), enemy.velocity);
            // Right
            if(Vector2.Angle(new Vector2(1, 0), enemy.velocity) < 45) 
                GetComponent<Animator>().runtimeAnimatorController = runRight;
            else if(Vector2.Angle(new Vector2(-1, 0), enemy.velocity) < 45)
                GetComponent<Animator>().runtimeAnimatorController = runLeft;
            else if (Vector2.Angle(new Vector2(0, 1), enemy.velocity) < 45)
                GetComponent<Animator>().runtimeAnimatorController = runBack;
            else if (Vector2.Angle(new Vector2(0, -1), enemy.velocity) < 45)
                GetComponent<Animator>().runtimeAnimatorController = runFront;

        }
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
    }

    void Follow()
    {
        // Direction enemy should target
        target = player.transform.position - transform.position;
        enemy.AddForce(target.normalized * moveSpeed * Time.deltaTime);
    }

    private bool isPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < followRange;
    }

}
