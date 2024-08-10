using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBodyScript : MonoBehaviour
{
    // Chooses a random sprite to display on instantiation
    public Sprite[] possibleSprites;

    public AudioSource deathSound;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
        deathSound.Play();
    }

}
