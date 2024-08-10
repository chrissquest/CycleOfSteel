using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitUpgrade : MonoBehaviour
{
    public void OnExit()
    {
        // Probably save / override data here

        SceneManager.LoadScene("Hub");
    }
}
