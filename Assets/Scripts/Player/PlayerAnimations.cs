using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public GameObject pivot;

    // Movement Anim, move to PlayerVisuals?
    public RuntimeAnimatorController runRight;
    public RuntimeAnimatorController runLeft;
    public RuntimeAnimatorController runFront;
    public RuntimeAnimatorController runBack;

    public RuntimeAnimatorController pivotAnim;
    public RuntimeAnimatorController pivotRight;
    public RuntimeAnimatorController pivotLeft;

    public AudioSource playerWalk;

    public void AnimateUpdateShooting(Vector2 moveVect, Vector2 lookVect)
    {
        // Player pivot movement, basically set the rotation of the pivot to the lookVect, and make sure to flip it when looking left or right
        pivot.transform.localRotation = Quaternion.FromToRotation(Vector3.right, -lookVect);
        // Looking left
        if (lookVect.x < 0)
            pivot.transform.localScale = new Vector3(1f, 1f, 1f);
        // Looking right
        else
            pivot.transform.localScale = new Vector3(1f, -1f, 1f);
    }

    public void AnimateUpdate(Vector2 moveVect)
    {
        // Update pivot locations, moving the gun left or right of the player depending on if we move left or right

        if (moveVect.x <= 0)
        {
            pivot.transform.localRotation = Quaternion.Euler(0f, 0f, 50f);
            pivot.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            pivot.transform.localRotation = Quaternion.Euler(0f, 0f, 130f);
            pivot.transform.localScale = new Vector3(1f, -1f, 1f);
        }
    }

    public void AnimateOnMove(Vector2 moveVect)
    {
        // When we begin moving
        if (!PauseMenu.isPaused && !Player.getPlayer.playerUI.isShowingDialogue && moveVect != Vector2.zero)
        {
            // Sound
            playerWalk.Play();

            // Set animations
            if (moveVect.x > 0f)
            {
                GetComponent<Animator>().runtimeAnimatorController = runRight;
                pivot.GetComponent<Animator>().runtimeAnimatorController = pivotRight;
            }
            else if (moveVect.x < 0f)
            {
                GetComponent<Animator>().runtimeAnimatorController = runLeft;
                pivot.GetComponent<Animator>().runtimeAnimatorController = pivotLeft;

            }
            else if (moveVect.y > 0f)
            {
                GetComponent<Animator>().runtimeAnimatorController = runBack;
                pivot.GetComponent<Animator>().runtimeAnimatorController = pivotAnim;
            }
            else if (moveVect.y < 0f)
            {
                GetComponent<Animator>().runtimeAnimatorController = runFront;
                pivot.GetComponent<Animator>().runtimeAnimatorController = pivotAnim;
            }
        }
        // Stop moving
        else
        {
            playerWalk.Pause();

            GetComponent<Animator>().runtimeAnimatorController = null;
            pivot.GetComponent<Animator>().runtimeAnimatorController = null;
        }
    }

}
