using UnityEngine;

public class LaserGun : Gun
{
    // Prefabs
    public GameObject laserStart;
    public GameObject laserMiddle;
    public GameObject laserEnd;

    // Initialized objects
    private GameObject start;
    private GameObject middle;
    private GameObject end;

    public float damageDelay;
    public float damage;
    public float knockback;
    public Vector3 shake;

    // Mechanics
    private float lastDamage;
    private AudioSource fireSound;
    private bool coolingDown;

    public LayerMask ledgesLayer;


    new void Start()
    {
        base.Start();
        fireSound = GetComponent<AudioSource>();
        coolingDown = false;
    }

    public override void startFiring(Transform t)
    {
        base.startFiring(t);
        if (start == null) start = Instantiate(laserStart, t);
        if (middle == null) middle = Instantiate(laserMiddle, t);
        if (end == null) end = Instantiate(laserEnd, t);

        fireSound.Play();
    }

    public override void stopFiring()
    {
        base.stopFiring();
        Destroy(start);
        Destroy(middle);
        Destroy(end);

        fireSound.Pause();
    }

    public override void updateFire(Transform t, Vector2 dir, Vector3 pos, Vector2 playerMove, float crit)
    {
        if (Time.time > heatCooldownFinished)
        {
            // Cooldown was just refreshed
            if (coolingDown)
            {
                // Could possibly just do this every frame anyway, maybe StartFiring isn't that useful since we want most guns to shoot automatically instead of anything on click.
                startFiring(t);
                coolingDown = false;
            }

            Camera.main.GetComponent<CameraController>().Shake(shake.x, shake.y, shake.z);

            // Raycast out to see how long laser should be 
            RaycastHit2D hit = Physics2D.Raycast(pos, dir, 20f, ~ledgesLayer);
            float laserLength = 20f;

            // We hit something
            if (hit.collider != null)
            {
                laserLength = hit.distance;

                // Damage killables
                if (Time.time - lastDamage > damageDelay)
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        // Chance to hit a crit
                        if (crit >= UnityEngine.Random.Range(0f, 1f))
                            hit.transform.gameObject.GetComponent<Killable>().DamageKillable(damage * 2f, dir * knockback, true);
                        else
                            hit.transform.gameObject.GetComponent<Killable>().DamageKillable(damage, dir * knockback, false);
                    }
                   
                    TickHeat();
                    lastDamage = Time.time;
                }
            }

            // Update middle length / pos
            // And end pos 
            // based on raycast hit
            Vector3 laserDir3D = new Vector3(dir.x, dir.y, 0f);
            middle.transform.localScale = new Vector3(-laserLength * 4f, 1f, 1f);

            // Position to tip of gun
            start.transform.position = pos;
            middle.transform.position = pos + laserDir3D * (laserLength / 2f);
            end.transform.position = pos + laserDir3D * laserLength;

            // Rotate Sprites
            Quaternion quatRot = Quaternion.FromToRotation(Vector3.right, -laserDir3D);
            start.transform.localRotation = quatRot;
            middle.transform.localRotation = quatRot;
            end.transform.localRotation = quatRot;
        } 
        else
        {
            stopFiring();
            coolingDown = true;
        }
    }
}
