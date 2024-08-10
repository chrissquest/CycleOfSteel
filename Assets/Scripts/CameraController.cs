using System;
using UnityEngine;
using Coordinate;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private PlayerData playerScript;

    private Coord roomCoord;

    private Vector3 instantCameraLocation;

    private float halfWiggleX, halfWiggleY;

    void Start()
    {
        // Wiggle room
        halfWiggleX = (RoomInfo.width - CameraInfo.camWidth) / 2f;
        halfWiggleY = (RoomInfo.height - CameraInfo.camHeight) / 2f;

        playerScript = player.GetComponent<PlayerData>();
    }

    private float camMinX, camMinY, camMaxX, camMaxY,
          instantX, instantY;
    void Update()
    {
        // Room coordinate
        roomCoord = playerScript.roomCoord;

        // Camera Boundries (World space)
        camMinX = RoomInfo.width * roomCoord.x + RoomInfo.halfwidth - halfWiggleX;
        camMaxX = RoomInfo.width * roomCoord.x + RoomInfo.halfwidth + halfWiggleX;
        camMinY = RoomInfo.height * roomCoord.y + RoomInfo.halfheight - halfWiggleY;
        camMaxY = RoomInfo.height * roomCoord.y + RoomInfo.halfheight + halfWiggleY;

        // Instant cam snap (teleports between rooms)
        instantX = Math.Clamp(player.transform.position.x, camMinX, camMaxX);
        instantY = Math.Clamp(player.transform.position.y, camMinY, camMaxY);

        instantCameraLocation.Set(instantX, instantY, -10);

        // Smooth cam movement
        transform.position = Vector3.MoveTowards(transform.position, instantCameraLocation, Time.deltaTime * 40.0f);

        // Shaking timer
        if(shaking && Time.time > stopShakeTime)
        {
            stopShake();
        }

    }

    private bool shaking;
    private float stopShakeTime;

    // Shake the camera for time and intensity
    public void Shake(float time, float intensity, float frequency)
    {
        if(time > 0)
        {
            // Start shaking
            stopShakeTime = Time.time + time;
            setShake(intensity, frequency);
            shaking = true;
        }
    }

    // Shortening very long call lol
    private void setShake(float amplitude, float frequency)
    {
        GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
    }

    public void stopShake()
    {
        setShake(0, 1);
        shaking = false;
        transform.rotation = Quaternion.identity;
    }

}
