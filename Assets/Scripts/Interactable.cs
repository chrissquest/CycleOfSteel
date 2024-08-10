using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public Material defaultMaterial;
    public Material outlineMaterial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(outlineMaterial != null)
                GetComponent<SpriteRenderer>().material = outlineMaterial;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(defaultMaterial != null)
                GetComponent<SpriteRenderer>().material = defaultMaterial;
        }
    }

    public abstract void OnInteract(Player player);
}
