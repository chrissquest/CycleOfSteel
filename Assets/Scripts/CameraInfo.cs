using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInfo
{
    public static float camHeight, camWidth, camHalfHeight, camHalfWidth;
    static CameraInfo()
    {
        camHeight = Camera.main.orthographicSize * 2;
        camWidth = Camera.main.aspect * camHeight;
        camHalfHeight = camHeight / 2f;
        camHalfWidth = camWidth / 2f;
    }
}
