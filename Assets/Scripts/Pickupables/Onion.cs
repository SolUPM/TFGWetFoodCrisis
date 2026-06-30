using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onion : Vegetable
{
    public static event System.Action<Vector2> onPickedUp;

    protected override void vegEffect()
    {
        
        PlayerMovement movement = playerRef.GetComponent<PlayerMovement>();

        //DIMINISHING UPGRADES:  cap - (cap - baseStat) / (1 + upgradeCount * factor);
        movement.rangeUpgrades++;
        movement.attackRange = 3.25f - (3.25f - movement.baseRange) / (1 + movement.rangeUpgrades * 0.1f);

        onPickedUp?.Invoke(transform.position);
    }
}
