using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElementalEffectHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool isPlayer = false;
    Rigidbody2D rigidBody;
    Killable   killabledamageHandler;
    PlayerData playerDamageHandler;
    [NonSerialized] public float lavaTimer = 0;
    [NonSerialized] public float waterTimer = 0;
    [NonSerialized] public float burnTimer = 0;
    [SerializeField] float maxFreezeEffect = 20;
    [SerializeField] float maxBurnTime = 5;
    [SerializeField] float burnSpeed = .05f;
    float freezeStrength = 0;
    float burnStrength = 0;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (isPlayer)
        {
            playerDamageHandler = GetComponent<PlayerData>();
        }
        else
        {
            killabledamageHandler = GetComponent<Killable>();
        }
    }

    private void Update()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        if (waterTimer < 0)
        {
            StopWaterEffect();
        }
        else if(waterTimer > 0)
        {
            waterTimer -= 0.1f;
            if (isPlayer)
                Debug.Log(waterTimer);
        }

        if (lavaTimer < 0)
        {
            StopLavaEffect();
        }
        else if (lavaTimer > 0)
        {
            lavaTimer -= 0.1f;
            if (isPlayer)
                Debug.Log(lavaTimer);
                
        }

        if(freezeStrength > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.3f, 1, 1);
            //Debug.Log(rigidBody.drag);
            freezeStrength -= 0.1f;
            rigidBody.drag -= 0.1f;
        }
   
        if(burnTimer > 0)
        {
            burnTimer -= 0.1f;
            burn();
        }
    }
    public void SetBurnEffect(float strength)
    {
        EndFreezeEffect();
        EndWaterEffect();

        burnTimer = maxBurnTime;
        if(burnStrength < strength)
        {
            burnStrength = strength;
        }
    }
    void burn()
    {
        if (burnTimer % burnSpeed == 0)
        {
            if (isPlayer)
            {
                playerDamageHandler.DamagePlayer(playerDamageHandler.maxHealth * burnStrength, Vector2.zero);
            }
            else
            {
                killabledamageHandler.DamageKillable(killabledamageHandler.maxHealth * burnStrength, Vector2.zero, false);
            }
        }
        
    }
    public void EndBurnEffect()
    {
        burnTimer = 0;
    }
    public void SetFreezeEffect(float strength)
    {
        EndBurnEffect();
        EndLavaEffect();

        if(freezeStrength + strength <= maxFreezeEffect)
        {
            freezeStrength += strength;
            rigidBody.drag += strength;
        }
    }
    public void EndFreezeEffect()
    {
        if (freezeStrength > 0)
        {
            rigidBody.drag -= freezeStrength;
            freezeStrength = 0;
        }
    }

    public void SetWaterEffect(float duration)
    {
        EndLavaEffect();
        EndBurnEffect();
        if(waterTimer == 0)
        {
            rigidBody.drag = rigidBody.drag * 2;
        }
        if(waterTimer < duration)
        {
            waterTimer = duration;
        }
    }
    public void EndWaterEffect()
    {
        if (waterTimer > 0)
        {
            rigidBody.drag = rigidBody.drag / 2;
            waterTimer = 0;
        }
    }

    void StopWaterEffect()
    {
        rigidBody.drag = rigidBody.drag / 2;
        waterTimer = 0;
    }

    private const float lavaStrength = 2f;

    public void SetLavaEffect(float duration)
    {
        EndWaterEffect();
        EndFreezeEffect();

        if (lavaTimer == 0)
        {
            rigidBody.drag = rigidBody.drag - lavaStrength;
        }
        if (lavaTimer < duration)
        {
            lavaTimer = duration;
        }
    }
    public void EndLavaEffect()
    {
        if(lavaTimer > 0)
        {
            rigidBody.drag = rigidBody.drag + lavaStrength;
            lavaTimer = 0;
        }
    }

    void StopLavaEffect()
    {
        rigidBody.drag = rigidBody.drag + lavaStrength;
        lavaTimer = 0;
    }

}
