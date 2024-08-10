using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene("Dungeon");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void DeleteSave()
    {
        File.Delete(Application.persistentDataPath + "/save1.dat");
    } 

}
