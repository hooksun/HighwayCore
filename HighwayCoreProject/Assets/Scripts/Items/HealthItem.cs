using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    public float HealAmount;

    protected override void AddToPlayer()
    {
        Player.ActivePlayer.Status.TakeDamage(-HealAmount);
    }
}
