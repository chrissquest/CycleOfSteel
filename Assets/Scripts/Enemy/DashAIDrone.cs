using Coordinate;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class DashAIDrone : MonoBehaviour
{

    public float followRange;

    public float moveSpeed;

    private Player player;
    private Rigidbody2D enemy;

    private Vector2 target;
    private Vector2 targetGrav;
    private float lastWanderTime = 0;
    public float wanderInterval;

    private Coord roomCoord;

    private int firstSight = 0;// for raising drone off ground

    void Start()
    {
        enemy = GetComponent<Rigidbody2D>();
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
            if (isPlayerInRange())
            {
                if (firstSight <= 10)//for raising drone off ground, linked to fps kinda so prolly shit
                {
                    this.gameObject.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);//Changes the scale of the drone to make it look like it rises
                    firstSight++;
                }
                Follow();

            }
            else
                Wander();
        }
    }

    void Wander()
    {
        // Random direction to walk towards
        if (Time.time - lastWanderTime > wanderInterval)
        {
            target = new Vector2(Random.Range(0, 3) - 1, Random.Range(0, 3) - 1);

            lastWanderTime = Time.time;
        }

        targetGrav = Vector2.MoveTowards(targetGrav, target.normalized, Time.deltaTime * 4.0f);
        enemy.velocity = targetGrav * moveSpeed;
        // new can be removed if we have global var
        float friction = 0.985f;
        target.Scale(new Vector2(friction, friction));
    }

    void Follow()
    {
        // Direction enemy should target
        target = player.transform.position - transform.position;

        enemy.AddForce(target.normalized * 7f * player.playerData.difficulty);
    }

    private bool isPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= followRange;
    }



}
