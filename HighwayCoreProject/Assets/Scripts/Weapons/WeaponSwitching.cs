using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : PlayerBehaviour
{
    public Gun[] weapons;
    public GunData shotgun;
    public GunData rifle;
    public GunData pistol;
    public GunData sniper;
    public GunScript gunScript;
    public int currentWeapon;
    int prevSelectedWeapon;
    public int numOfSwitch;

    void Start()
    {
        numOfSwitch = 0;
        findWeapon();
        currentWeapon = 0;
        prevSelectedWeapon = 0;
        selectWeapon(currentWeapon);
    }

    void Update()
    {
        prevSelectedWeapon = currentWeapon;
        for(int i=0;i<keyCodes.Length;i++){
            if(Input.GetKeyDown(keyCodes[i])){
                currentWeapon = i;
            }
        }
        if(prevSelectedWeapon!=currentWeapon){
            //player.weaponAnim.Idle();
            selectWeapon(currentWeapon);
            gunScript.ReloadSound.Stop();
            numOfSwitch++;
        }
        //changeGunData();
    }

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1, 
        KeyCode.Alpha2,
        KeyCode.Alpha3
    };

    void findWeapon(){
        weapons = new Gun[transform.childCount];
        for(int i=0;i<transform.childCount;i++){
            weapons[i] = transform.GetChild(i).GetComponent<Gun>();
            weapons[i].data.currentAmmoInMag = weapons[i].data.magazineSize;
            weapons[i].data.ammoLeft = weapons[i].data.maxAmmo;
        }
    }

    void selectWeapon(int index){
        for(int i=0;i<weapons.Length;i++){
            weapons[i].gameObject.SetActive(i == index);
        }
        gunScript.gunData = weapons[index].data;
        gunScript.gun = weapons[index];
        gunScript.gunData.isReloading = false;
        gunScript.timeSinceLastSwitch = 0f;
        gunScript.secondaryInput = false;
    }

    public void AddAmmo(int ammount, AmmoType ammoType){
        switch(ammoType){
            case AmmoType.pistol:
                pistol.ammoLeft = Mathf.Clamp(pistol.ammoLeft+ammount, 0, pistol.maxAmmo);
                break;
            case AmmoType.ar:
                rifle.ammoLeft = Mathf.Clamp(rifle.ammoLeft+ammount, 0, rifle.maxAmmo);
                break;
            case AmmoType.sniper:
                sniper.ammoLeft = Mathf.Clamp(sniper.ammoLeft+ammount, 0, sniper.maxAmmo);
                break;
            case AmmoType.shotgun:
                shotgun.ammoLeft = Mathf.Clamp(shotgun.ammoLeft+ammount, 0, shotgun.maxAmmo);
                break;
        }
    }

}
