using UnityEngine;

public class AttackColliderRelay : MonoBehaviour
{
    public bool initialHit;

    // I thoughttt this would be more useful but I think I overkilled it
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponentInParent<AttackCollidable>().AttackColliderRelay(initialHit);
        }
    }

}
