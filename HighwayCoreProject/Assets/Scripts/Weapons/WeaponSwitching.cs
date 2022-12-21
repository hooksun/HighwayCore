using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : PlayerBehaviour
{
    public Transform[] weapons;
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
        shotgun.ammoLeft = shotgun.maxAmmo;
        rifle.ammoLeft = rifle.maxAmmo;
        pistol.ammoLeft = pistol.maxAmmo;
        sniper.ammoLeft = sniper.maxAmmo;
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
            numOfSwitch++;
        }
        //changeGunData();
    }

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1, 
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4
    };

    void findWeapon(){
        weapons = new Transform[transform.childCount];
        for(int i=0;i<transform.childCount;i++){
            weapons[i] = transform.GetChild(i);
        }
    }

    void selectWeapon(int index){
        if(index == 0 && pistol.available){
            for(int i=0;i<weapons.Length;i++){
                weapons[i].gameObject.SetActive(i == index);
            }
            gunScript.gunData = pistol;
            gunScript.gunData.isReloading = false;
            gunScript.timeSinceLastSwitch = 0f;
        }
        if(index == 1 && shotgun.available){
            for(int i=0;i<weapons.Length;i++){
                weapons[i].gameObject.SetActive(i == index);
            }
            gunScript.gunData = shotgun;
            gunScript.gunData.isReloading = false;
            gunScript.timeSinceLastSwitch = 0f;
        }
        if(index == 2 && rifle.available){
            for(int i=0;i<weapons.Length;i++){
                weapons[i].gameObject.SetActive(i == index);
            }
            gunScript.gunData = rifle;
            gunScript.gunData.isReloading = false;
            gunScript.timeSinceLastSwitch = 0f;
        }
        if(index == 3 && sniper.available){
            for(int i=0;i<weapons.Length;i++){
                weapons[i].gameObject.SetActive(i == index);
            }
            gunScript.gunData = sniper;
            gunScript.gunData.isReloading = false;
            gunScript.timeSinceLastSwitch = 0f;
        }
    }

}
