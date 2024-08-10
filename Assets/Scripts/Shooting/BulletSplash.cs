using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSplash : MonoBehaviour
{
    public float TimeToLive;

    void Start()
    {
        Destroy(gameObject, TimeToLive);
    }

    void Update()
    {
        
    }
}
