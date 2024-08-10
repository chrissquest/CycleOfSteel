using System;
using UnityEngine;

public class Gun : Interactable
{
    public int ID;
    protected bool equipped = false;

    public float heatCapacity;
    public float heatCooldownTime;

    // Overheat mechanic
    private float heat;
    private int cooldownTicks;
    private float cooldownTickFin;
    private float heatDecrement;
    protected float heatCooldownFinished;

    private Player player;
    private bool firing;
    private bool hasHeatVars;

    protected void Start()
    {
        heatCooldownFinished = 0;
        cooldownTickFin = 0;
        //equipped = false; // Can be set by InstantiateToSlot in PlayerData before gun start gets called, should be respected
        player = Player.getPlayer;
        //
        cooldownTicks = (int)(heatCooldownTime / 0.25f);
        heatDecrement = heatCapacity / cooldownTicks;
        hasHeatVars = heatCapacity > 0 && heatCooldownTime > 0;
    }

    void Update()
    {

        //If the gun is equipped, not firing, has heat variables set, and can cooldown a heat tick
        if (!firing && hasHeatVars && Time.time > heatCooldownFinished && Time.time > cooldownTickFin)
        {
            if (heat - heatDecrement > 0)
            {
                heat -= heatDecrement;
                updateHeatBar();
            }
            else if (heat > 0)
            {
                heat = 0;
                updateHeatBar();
            }

            cooldownTickFin = Time.time + 0.25f;
        }
    }

    void CooldownHeatProg()
    {
        heat -= heatDecrement;
        updateHeatBar();
    }

    void CooldownHeatFinal()
    {
        heat = 0;
        updateHeatBar();
    }

    void updateHeatBar()
    {
        if(equipped)
        {
            if (hasHeatVars)
                player.playerUI.heatBar.value = heat / heatCapacity;
            else
                player.playerUI.heatBar.value = 0;
        }
    }

    public virtual void updateFire(Transform t, Vector2 dir, Vector3 pos, Vector2 playerMove, float crit) 
    {
    
    }
    public virtual void startFiring(Transform t) 
    { 
        firing = true;
    }
    public virtual void stopFiring() 
    { 
        firing = false;
    }

    protected void TickHeat()
    {
        // Increment heat
        if (heat + 1f < heatCapacity)
        {
            heat += 1f;
            updateHeatBar();
        }
        // If we've reached max heat
        else
        {
            heat = heatCapacity;
            updateHeatBar();
            heatCooldownFinished = Time.time + heatCooldownTime;
            // Begin cooldown
            for (int i = 1; i < cooldownTicks; i++)
                Invoke("CooldownHeatProg", i * 0.25f);
            Invoke("CooldownHeatFinal", heatCooldownTime);
        }
    }

    public void Equipped()
    {
        equipped = true;
        updateHeatBar();
    }

    public void EquippedNoUpdate()
    {
        equipped = true;
    }

    public void Dequipped()
    {
        equipped = false;
    }

    public override void OnInteract(Player player)
    {
        player.playerInventory.AddInventory(this.gameObject);
    }
}



