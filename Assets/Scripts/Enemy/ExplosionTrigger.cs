using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{

    public GameObject explosionPrefab;

    void Start()
    {
    }
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            this.GetComponentInParent<Killable>().Die();
            //If this trigger senses the rigidBody it kills itself
        }
        
    }

}
