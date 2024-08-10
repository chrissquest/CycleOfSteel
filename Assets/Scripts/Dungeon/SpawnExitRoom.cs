using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : RoomScript
{
    public GameObject topWall;
    public GameObject bottomWall;
    public GameObject leftWall;
    public GameObject rightWall;

    public override void SealRoom()
    {
        if (top) topWall.SetActive(true);
        if (bottom) bottomWall.SetActive(true);
        if (left) leftWall.SetActive(true);
        if (right) rightWall.SetActive(true);
    }


}
