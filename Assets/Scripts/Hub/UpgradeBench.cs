using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeBench : Interactable
{

    public override void OnInteract(Player player)
    {
        player.playerUI.upgradeMenu.ShowMenu();
    }
}
