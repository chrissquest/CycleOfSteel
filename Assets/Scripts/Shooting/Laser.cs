using UnityEngine;

public class Laser : MonoBehaviour
{
    // Laser variables
    // Prefabs
    public GameObject laserStart;
    public GameObject laserMiddle;
    public GameObject laserEnd;

    // Initialized objects
    private GameObject start;
    private GameObject middle;
    private GameObject end;

    public float laserDamageDelay;
    private float laserDamageEnd;
    public float laserDamage;
    public LayerMask ignoreLayer;

    void Start()
    {
        if (start == null) start = Instantiate(laserStart, transform);
        if (middle == null) middle = Instantiate(laserMiddle, transform);
        if (end == null) end = Instantiate(laserEnd, transform);
    }

    public void UpdateLaser(float angle)
    {
        // Raycast out to see how long laser should be 
        float laserLength = 20f;
        //float angle = Time.time * 15f;
        Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;
        Vector3 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, laserLength, ~ignoreLayer);

        // We hit something
        if (hit.collider != null)
        {
            laserLength = hit.distance;

            // Damage killables
            if (Time.time > laserDamageEnd)
            {
                if (hit.collider.tag == "Player")
                    hit.transform.gameObject.GetComponent<PlayerData>().DamagePlayer(laserDamage, dir);

                laserDamageEnd = Time.time + laserDamageDelay;
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
}
