using UnityEngine;

public class Bullet_Freeze : MonoBehaviour
{
    [SerializeField] float freezeStrength = 5f;
    [SerializeField] bool hitsPlayer = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ElementalEffectHandler>() != null)
        {
            if(collision.tag == "Player" && hitsPlayer)
                collision.GetComponent<ElementalEffectHandler>().SetFreezeEffect(freezeStrength);
            else if (collision.tag != "Player" && !hitsPlayer)
                collision.GetComponent<ElementalEffectHandler>().SetFreezeEffect(freezeStrength);
        }
    }

    private void Start()
    {
    }
}
