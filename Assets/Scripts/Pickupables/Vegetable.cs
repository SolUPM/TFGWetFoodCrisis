using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : Pickupable
{
    public override void OnPickup()
    {
        
        vegEffect();
        base.OnPickup();
    }

    protected virtual void vegEffect() { }
}
