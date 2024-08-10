using Coordinate;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public class RangedAI_Laser : MonoBehaviour
{
    [SerializeField] LayerMask LayersToHit;
    public float followRange;
    public float attackRange;

    public GameObject bulletFab;

    public float moveSpeed;

    private GameObject player;
    private PlayerData playerScript;
    private Rigidbody2D enemy;

    private Vector2 target;
    private Vector2 rayTarget;
    private Vector2 targetGrav;
    private float lastWanderTime = 0;
    public float wanderInterval; 
    public float maxFireTime;
    private float fireTime;
    public float fireInterval;
    private float nextFireTime = 5;
    public float damage;
    public float damageRate = 0.5f;
    private float nextDamageTick = 0;

    private float spottingTimer = 0f;
    public float spottingLength = 5f;
    private LineRenderer lineRenderer;
    private RaycastHit2D hit;
    public float laserRotSpeed = 100f;
    private Vector3 laserTarget;
    Quaternion laserRot;
    Quaternion quatRot;

    Color newGreen = new Color(0.5f, 0.5f, 0.5f, 0f);
    Color newRed = new Color(1f, 1f, 1f, 0f);
    Color newYellow = new Color(1f, 0.92f, 0.016f, 0f);
    ParticleSystem particles;

    private Coord roomCoord;
    
    void Start()
    {
        particles = transform.Find("laserParticles").GetComponent<ParticleSystem>();
        nextFireTime = maxFireTime;
        laserRot = transform.rotation;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        enemy = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerData>();
        // Room coordinate
        roomCoord.x = (int)Math.Floor(transform.position.x / RoomInfo.width);
        roomCoord.y = (int)Math.Floor(transform.position.y / RoomInfo.height);
    }

    void Update()
    {
        // AI only moves if in the same room as rigidBody
        if(roomCoord == playerScript.roomCoord)
        {
            // Determine if enemy should be agro based on range
            //Shoot raycast at player first
            if(nextDamageTick > 0)
            {
                nextDamageTick -= Time.deltaTime;
            }

            if(spottingTimer <= 0)
            {
                fireTime = 0;
                nextFireTime = fireInterval;
                Wander();
                laserRot = quatRot;
                if(isInAttackRange() || isInFollowRange())
                    PlayerInLineOfSight();
            }
            else if (isInAttackRange() && fireTime > 0)
            {
                PlayerInLineOfSight();
                
                if (fireTime <= maxFireTime - 1)
                {
                    Attack();
                }
                else
                {
                    lineRenderer.endColor = Color.yellow;
                    lineRenderer.startColor = newYellow;
                }
                fireTime -= Time.deltaTime;
                nextFireTime = fireInterval;
                spottingTimer -= Time.deltaTime;
            }
            else if (isInFollowRange())
            {
                PlayerInLineOfSight();
                Follow();
                spottingTimer -= Time.deltaTime;
                laserRot = quatRot;
                fireTime = 0;
            }

            if (nextFireTime > 0)
            {
                nextFireTime -= Time.deltaTime;
                
            }
            else
            {
                fireTime = maxFireTime;
            }
           
        }
    }

    void Wander()
    {
        // Random direction to walk towards
        // Every 10 frames choose a new direction
        if (particles.isPlaying) particles.Stop();
        lineRenderer.enabled = false;
        if (Time.time - lastWanderTime > wanderInterval)
        {
            target = new Vector2(Random.Range(0, 3) - 1, Random.Range(0, 3) - 1);

            lastWanderTime = Time.time;
        }

        // Simplifies below movement by using the built in force system ;)
        enemy.AddForce(target.normalized * moveSpeed * Time.deltaTime);
    }

    void Follow()
    {
        // Direction enemy should target
        //particles.Stop();
        if (particles.isPlaying) particles.Stop();
        lineRenderer.enabled = true;
        lineRenderer.endColor = Color.green;
        lineRenderer.startColor = newGreen;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hit.point);
        target = player.transform.position - transform.position;
        enemy.AddForce(target.normalized * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        // Direction enemy should target
        laserTarget = player.transform.position - transform.position;
        laserTarget.Normalize();
        float zAngle = Mathf.Atan2(laserTarget.y, laserTarget.x) * Mathf.Rad2Deg;
        Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
        laserRot = Quaternion.RotateTowards(laserRot, desiredRot, laserRotSpeed * Time.deltaTime);
        laserTarget = laserRot * transform.right;

        RaycastHit2D laser = Physics2D.Raycast(transform.position, laserTarget.normalized, attackRange, LayersToHit);

        lineRenderer.enabled = true;
        lineRenderer.endColor = Color.red;
        lineRenderer.startColor = newRed;
        lineRenderer.SetPosition(0, transform.position);
        if(laser.collider != null)
        {
            lineRenderer.SetPosition(1, transform.position + laserTarget * laser.distance);

            if (laser.collider.gameObject.name == "Player" & nextDamageTick <= 0)
            {
                laser.collider.gameObject.GetComponent<PlayerData>().DamagePlayer(damage, Vector2.zero);
                nextDamageTick = damageRate;
            }
        }
        else
            lineRenderer.SetPosition(1, transform.position + laserTarget * attackRange);

        if (!particles.isPlaying) particles.Play();
        particles.transform.position = laser.point;
    }

    bool PlayerInLineOfSight()//CHECK IF THE RAYCAST ACTUALLY HITS THE PLAYER OR NOT AND RETURN BOOL
    {
        rayTarget = player.transform.position - transform .position;
        quatRot = Quaternion.FromToRotation(Vector3.right, rayTarget.normalized);
        Vector3 faceTarget = quatRot * transform.right;//to figure out direction of raycast?

        hit = Physics2D.Raycast(transform.position, faceTarget, 100f, LayersToHit);
        if (hit.collider.gameObject.name == "Player")
        {
            spottingTimer = spottingLength;
            return true;
        }else
            return false;
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
