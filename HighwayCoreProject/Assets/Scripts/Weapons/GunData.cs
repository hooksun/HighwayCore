using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName ="Weapon/Gun")]
public class GunData : ScriptableObject
{
    public bool available;
    public new string name;

    public float damage;
    public float maxDistance;

    public int currentAmmoInMag;
    public int magazineSize;
    public int maxAmmo;
    public int ammoLeft;
    public int bulletsPerShot = 1;
    public float fireRate;
    public float reloadTime, reloadRecovery;
    public bool isReloading;
    public float bulletSpread;
    public float bulletSpeed;
    public float switchSpeed;
    public RuntimeAnimatorController animator;
    public AudioClip shootAudio;
    public float reloadDelay;
    public AudioTime[] reloadSequence;

}
