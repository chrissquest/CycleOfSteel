using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject upgradeMenu;

    public PlayerData playerdata;

    public AudioMixer mixer;

    public TextMeshProUGUI critText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI moveText;

    public TextMeshProUGUI priceCritText;
    public TextMeshProUGUI priceHpText;
    public TextMeshProUGUI priceMoveText;

    // Prices
    private int priceCrit;
    private int priceHp;
    private int priceMove;

    private float scaleCrit;
    private float scaleHp;
    private float scaleMove;

    // Save these in playerdata
    [NonSerialized] public int critLevel = 0;
    [NonSerialized] public int hpLevel = 0;
    [NonSerialized] public int moveLevel = 0;

    void Start()
    {
        upgradeMenu.SetActive(false);
        updateStats();
    }

    private void updateStats()
    {
        // Show what stats the player has currently
        critText.text = "Crit " + (playerdata.critChance * 100).ToString() + "%";
        hpText.text = "Hp  " + playerdata.maxHealth.ToString();
        moveText.text = "Move " + playerdata.moveSpeed.ToString();

        // Update price numbers and scaling based on level
        // CRIT
        // 5 is base price, 2.5f is price scaling
        // so 5, 7, 10, 12, 15, etc will be price scaling
        priceCrit = 5 + Mathf.FloorToInt(2.5f * critLevel);
        // 5 is base crit, 0.5f is scaling
        // 5, 5, 4, 4, 3, 3, 2, 2, 1, 1, 1... continue with only 1s
        scaleCrit = Math.Max(1, 5 - Mathf.FloorToInt(0.5f * critLevel)) / 100f;
        // HP
        // 10 base, 5 scaling
        priceHp = 10 + (5 * hpLevel);
        // Constant Hp scaling
        scaleHp = 200f;
        // MOVE
        priceMove = 6 + (3 * moveLevel);
        // Same as crit
        // 3 base, 0.5f scaling
        scaleMove = Math.Max(0.25f, 2 - 0.25f * moveLevel);

        // Update price visuals
        priceCritText.text = priceCrit.ToString();
        priceHpText.text = priceHp.ToString();
        priceMoveText.text = priceMove.ToString();
    }


    public void ShowMenu()
    {
        // If the menu is already showing and key is pressed, hide menu
        if (PauseMenu.isPaused)
        {
            GetComponent<PauseMenu>().ResumeShowMenu();
        }
        else
        {
            GetComponent<PauseMenu>().Pause();
            upgradeMenu.SetActive(true);
            updateStats();
        }
    }

    // might not need this
    public void HideMenu()
    {
        upgradeMenu.SetActive(false);
    }

    public void UpgradeCrit()
    {
        if (playerdata.dataCells >= priceCrit)
        {
            playerdata.setDataCells(playerdata.dataCells - priceCrit);
            playerdata.addCritChance(scaleCrit);
            critLevel++;
            updateStats();
        }
    }

    public void UpgradeHp()
    {
        // This should probably change price and amount as upgrades are purchased
        if(playerdata.dataCells >= priceHp)
        {
            playerdata.setDataCells(playerdata.dataCells - priceHp);
            playerdata.AddMaxHealth(scaleHp);
            hpLevel++;
            updateStats();
        }
    }

    public void UpgradeMove()
    {
        if (playerdata.dataCells >= priceMove)
        {
            playerdata.setDataCells(playerdata.dataCells - priceMove);
            playerdata.addMoveSpeed(scaleMove);
            moveLevel++;
            updateStats();
        }
    }


}
