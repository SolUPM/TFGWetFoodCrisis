using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange : Vegetable
{
    public static event System.Action<Vector2> onPickedUp;
    protected override void vegEffect()
    {
        
        PlayerMovement movement = playerRef.GetComponent<PlayerMovement>();

        //DIMINISHING UPGRADES:  cap - (cap - baseStat) / (1 + upgradeCount * factor);
        movement.speedUpgrades++;
        movement.moveSpeed = 13 - (13 - movement.baseSpeed) / (1 + movement.speedUpgrades * 0.08f);

        onPickedUp?.Invoke(transform.position);
    }
}
