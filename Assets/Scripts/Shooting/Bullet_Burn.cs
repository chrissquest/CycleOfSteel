using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Burn : MonoBehaviour
{
    [SerializeField] float burnStrength = .05f;
    [SerializeField] bool hitsPlayer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ElementalEffectHandler>() != null)
        {
            if (collision.tag == "Player" && hitsPlayer)
                collision.GetComponent<ElementalEffectHandler>().SetBurnEffect(burnStrength);
            else if (collision.tag != "Player" && !hitsPlayer)
                collision.GetComponent<ElementalEffectHandler>().SetBurnEffect(burnStrength);
        }
    }
}
