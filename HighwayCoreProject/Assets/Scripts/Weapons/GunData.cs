using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName ="Weapon/Gun")]
public class GunData : ScriptableObject
{
    public new string name;

    public float damage;
    public float maxDistance;

    public int currentAmmoInMag;
    public int magazineSize;
    public int maxAmmo;
    public int ammoLeft;
    public float fireRate;
    public float reloadTime;
    public bool isReloading;
    public float bulletSpread;
}
