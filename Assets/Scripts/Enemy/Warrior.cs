using Coordinate;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;

public class Warrior : AttackCollidable
{
    [SerializeField] LayerMask LayersToHit;
    private float spottingTimer = 0f;
    public float spottingLength = 5f;

    public float followRange;

    public float moveSpeed;

    private Player player;
    private Rigidbody2D rigidBody;

    private Vector2 target;
    private float lastWanderTime = 0;
    public float wanderInterval;

    private Coord roomCoord;

    // Movement Anim
    public RuntimeAnimatorController runRight;
    public RuntimeAnimatorController runLeft;
    public RuntimeAnimatorController runFront;
    public RuntimeAnimatorController runBack;

    // Attacking
    private float lastDamageTime = 0;
    public float damage;
    public float damageCooldown;
    bool isAttacking = false;
    private float hittingTimeEnd = 0;
    private float hittingCooldown = 0.4f;

    public RuntimeAnimatorController attackRight;
    public RuntimeAnimatorController attackLeft;
    public RuntimeAnimatorController attackUp;
    public RuntimeAnimatorController attackDown;

    // This is pretty messy but the attack just is complex
    public GameObject colliderAttackRight;
    public GameObject colliderAttackLeft;
    public GameObject colliderAttackUp;
    public GameObject colliderAttackDown;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
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
            if (!isAttacking)
            {
                // Determine if enemy should be agro based on range
                if (isPlayerInRange() && spottingTimer > 0)
                    Follow();
                else
                    Wander();

                // Movement Animation
                if(rigidBody.velocity.magnitude > 0.2f)
                {
                    float dirAngle = Vector2.Angle(new Vector2(0, 1), rigidBody.velocity);
                    // Right
                    if (Vector2.Angle(new Vector2(1, 0), rigidBody.velocity) < 45)
                    {
                        //GetComponent<Animator>().runtimeAnimatorController = attackRight;
                        GetComponent<Animator>().runtimeAnimatorController = runRight;
                    }
                    else if (Vector2.Angle(new Vector2(-1, 0), rigidBody.velocity) < 45)
                        GetComponent<Animator>().runtimeAnimatorController = runLeft;
                    else if (Vector2.Angle(new Vector2(0, 1), rigidBody.velocity) < 45)
                        GetComponent<Animator>().runtimeAnimatorController = runBack;
                    else if (Vector2.Angle(new Vector2(0, -1), rigidBody.velocity) < 45)
                        GetComponent<Animator>().runtimeAnimatorController = runFront;
                }

            }
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

        // So turns out multiplying by Time.deltaTime is not needed and possibly bad, but i've kinda used it for everything at this point
        // ill fix it next game :)
        rigidBody.AddForce(target.normalized * moveSpeed * player.playerData.difficulty);
        if (isPlayerInRange())
            PlayerInLineOfSight();
    }

    void Follow()
    {
        // Direction enemy should target
        target = player.transform.position - transform.position;
        rigidBody.AddForce(target.normalized * moveSpeed * player.playerData.difficulty);
        PlayerInLineOfSight();
        spottingTimer -= Time.deltaTime;
    }

    private bool isPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < followRange;
    }

    public override void AttackColliderRelay(bool initialHit)
    {
        if (initialHit)
        {
            if (Time.time - lastDamageTime > damageCooldown)
            {
                // Delay damage to when animation hits
                //p.playerData.DamagePlayer(damage);

                // So here I'll start animation and invoke it to check hitboxes when swing comes down
                // Depends on direction attacking...

                if (Vector2.Angle(new Vector2(1, 0), target.normalized) < 45)
                {
                    GetComponent<Animator>().runtimeAnimatorController = attackRight;
                    StartCoroutine(SwingHitCheck(Direction.RIGHT));
                }
                else if (Vector2.Angle(new Vector2(-1, 0), target.normalized) < 45)
                {
                    GetComponent<Animator>().runtimeAnimatorController = attackLeft;
                    StartCoroutine(SwingHitCheck(Direction.LEFT));
                }
                else if (Vector2.Angle(new Vector2(0, 1), target.normalized) < 45)
                {
                    GetComponent<Animator>().runtimeAnimatorController = attackUp;
                    StartCoroutine(SwingHitCheck(Direction.UP));
                }
                else if (Vector2.Angle(new Vector2(0, -1), target.normalized) < 45)
                {
                    GetComponent<Animator>().runtimeAnimatorController = attackDown;
                    StartCoroutine(SwingHitCheck(Direction.DOWN));
                }

                isAttacking = true;
                rigidBody.velocity = Vector2.zero;
                lastDamageTime = Time.time;
            }
        }
        else if (!initialHit && Time.time > hittingTimeEnd)
        {
            player.playerData.DamagePlayer(damage, Vector2.zero);
            hittingTimeEnd = Time.time + hittingCooldown;
        }
    }

    private IEnumerator SwingHitCheck(Direction d)
    {
        // 0.917s total
        yield return new WaitForSecondsRealtime(0.5f);

        if (d == Direction.RIGHT)
            colliderAttackRight.SetActive(true);
        else if(d == Direction.LEFT)
            colliderAttackLeft.SetActive(true);
        else if(d == Direction.UP)
            colliderAttackUp.SetActive(true);
        else if(d == Direction.DOWN)
            colliderAttackDown.SetActive(true);

        yield return new WaitForSecondsRealtime(hittingCooldown);

        colliderAttackRight.SetActive(false);
        colliderAttackLeft.SetActive(false);
        colliderAttackUp.SetActive(false);
        colliderAttackDown.SetActive(false);

        isAttacking = false;
    }

}
