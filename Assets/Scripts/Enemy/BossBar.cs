using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBar : MonoBehaviour
{
    //public Killable killable;
    public Slider bossBar;
    public TextMeshProUGUI bossName;

    public void SetKillable(Killable k)
    {
        k.healthBar = bossBar;
        bossName.text = k.enemyName;
    }

}
