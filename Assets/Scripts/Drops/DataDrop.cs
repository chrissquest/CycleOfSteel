using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDrop : MonoBehaviour
{
    public float pickupRange;
    public float pickupSpeed;
    public int value;

    private GameObject player;

    private void Start()
    {
        // I think this function is very inefficient... Perhaps make a singleton at start to grab player reference and keep
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if(isPlayerInRange())//PlayerData.isPlayerInRange(transform.position, pickupRange))
        {
            Vector3 target = (player.transform.position - transform.position);
            GetComponent<Rigidbody2D>().AddForce(target.normalized * pickupSpeed);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerData>().incrementDataCells(value);
            Destroy(gameObject);
        }
    }


    private bool isPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < pickupRange;
    }

}
