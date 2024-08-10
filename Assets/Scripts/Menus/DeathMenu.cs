using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public PlayerData playerData;
    public void Died()
    {
        timer.text = "4";
        StartCoroutine(Timer(4));
    }


    public IEnumerator Timer(int i) 
    { 
        timer.text = i.ToString();
        yield return new WaitForSecondsRealtime(1);
        if(i >= 0)
            StartCoroutine(Timer(i - 1));
        else
            playerData.Respawn();
    }


}
