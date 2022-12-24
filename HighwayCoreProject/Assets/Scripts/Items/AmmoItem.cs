using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item
{
    public int ammoAmount;
    public AmmoType ammoType;

    protected override void AddToPlayer()
    {
        //Player.ActivePlayer.WeaponSwitch.AddAmmo(ammoAmount, ammoType);
    }
}

public enum AmmoType{pistol, shotgun, ar, sniper}