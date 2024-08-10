using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public GameObject door;
    public RuntimeAnimatorController doorOpenAnim;
    public Sprite pressedSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet_Freeze bulletOut;
        if (collision.tag == "Bullet" && collision.TryGetComponent<Bullet_Freeze>(out bulletOut))
        {
            // If shot with a freeze bullet, then open the door
            door.GetComponent<Animator>().runtimeAnimatorController = doorOpenAnim;
            GetComponent<SpriteRenderer>().sprite = pressedSprite;
        }
    }
}
