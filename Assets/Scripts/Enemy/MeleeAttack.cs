using UnityEngine;

public class BasicAIAttack : MonoBehaviour
{

    private float lastDamageTime = 0;
    public float damage;
    public float damageCooldown;

    public Animator animator;
    public RuntimeAnimatorController attackAnim;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (Time.time - lastDamageTime > damageCooldown)
            {
                collision.gameObject.GetComponent<PlayerData>().DamagePlayer(damage, Vector2.zero);
                if (attackAnim != null)
                    animator.runtimeAnimatorController = attackAnim;
                lastDamageTime = Time.time;
            }
        }
    }

}
