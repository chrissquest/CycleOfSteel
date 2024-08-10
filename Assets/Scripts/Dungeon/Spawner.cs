using System;
using UnityEngine;
using static UnityEngine.GridBrushBase;

[Serializable]
public class Spawn
{
    public GameObject gameObject;
    public float weight;
}

public class Spawner : MonoBehaviour
{
    // Array of "pools" (Loot pools),
    // basically rolls through a list and spawns something
    public Spawn[] pool;
    public bool multiplePoolDrops; // Whether or not multiple pools can drop, true means there can.
    public Vector2 difficultyRange; // Min and Max difficulty to spawn at

    void Start()
    {
        // Roll through each spawn and just have a chance to drop it or not
        if (multiplePoolDrops)
        {
            foreach (Spawn spawn in pool)
                if (spawn.weight > UnityEngine.Random.Range(0f, 100f))
                {
                    Instantiate(spawn.gameObject, transform.position, transform.rotation);
                }
        }
        else
        {
            // Drop only one item based on it's chance, so order matters alot and chances can be smaller
            bool spawned = false;
            while (!spawned)
            {
                foreach (Spawn spawn in pool)
                    if (!spawned && spawn.weight > UnityEngine.Random.Range(0f, 100f))
                    {
                        Instantiate(spawn.gameObject, transform.position, transform.rotation);
                        spawned = true;
                    }
            }

        }

        Destroy(this.gameObject);
    }



    void Update()
    {
        
    }
}
