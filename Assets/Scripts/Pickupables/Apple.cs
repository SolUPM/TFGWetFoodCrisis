using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Vegetable
{

    public static event System.Action<Vector2> onPickedUp;

    protected override void vegEffect()
    {
        
        PlayerMovement movement = playerRef.GetComponent<PlayerMovement>();

        //DIMINISHING UPGRADES:  floor + (baseStat - floor) / (1 + upgradeCount * factor);
        movement.attackUpgrades++;
        movement.attackCooldown = 0.5f + (movement.baseAttack - 0.5f) / (1 + movement.attackUpgrades * 0.1f);

        onPickedUp?.Invoke(transform.position);
    }
}
