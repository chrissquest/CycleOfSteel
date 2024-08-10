using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGrate : MonoBehaviour
{
    // Start is called before the first frame update
    BoxCollider2D boxCollider;
    [SerializeField] float duration;


    [SerializeField] bool water = false;
    [SerializeField] bool lava = false;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponent<ElementalEffectHandler>() != null)
        {
            if(water == true)
            {
                collision.GetComponent<ElementalEffectHandler>().SetWaterEffect(duration);
            }else if(lava == true)
            {
                collision.GetComponent<ElementalEffectHandler>().SetLavaEffect(duration);
            }

            
        }
    }
}
