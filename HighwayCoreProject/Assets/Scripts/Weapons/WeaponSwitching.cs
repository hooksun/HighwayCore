using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public Transform[] weapons;
    public GunData shotgun;
    public GunData rifle;
    public GunData pistol;
    public GunScript gunScript;
    public int currentWeapon;
    int prevSelectedWeapon;
    

    // Start is called before the first frame update
    void Start()
    {
        findWeapon();
        currentWeapon = 0;
        prevSelectedWeapon = 0;
        selectWeapon(currentWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        prevSelectedWeapon = currentWeapon;
        for(int i=0;i<keyCodes.Length;i++){
            if(Input.GetKeyDown(keyCodes[i])){
                currentWeapon = i;
            }
        }
        if(prevSelectedWeapon!=currentWeapon){
            selectWeapon(currentWeapon);
        }
        //changeGunData();
    }

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1, 
        KeyCode.Alpha2,
        KeyCode.Alpha3
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
        }
        if(index == 1 && shotgun.available){
            for(int i=0;i<weapons.Length;i++){
                weapons[i].gameObject.SetActive(i == index);
            }
            gunScript.gunData = shotgun;
            gunScript.gunData.isReloading = false;
        }
        if(index == 2 && rifle.available){
            for(int i=0;i<weapons.Length;i++){
                weapons[i].gameObject.SetActive(i == index);
            }
            gunScript.gunData = rifle;
            gunScript.gunData.isReloading = false;

        }
    }

    void changeGunData(){
        if(currentWeapon == 0){
            if(pistol.available){
                gunScript.gunData = pistol;
                gunScript.gunData.isReloading = false;
                weapons[0].gameObject.SetActive(0 == 0);
            }
        }
        if(currentWeapon == 1){
            if(shotgun.available){
                gunScript.gunData = shotgun;
                gunScript.gunData.isReloading = false;
            }
        }
        else if(currentWeapon == 2){
            if(rifle.available){
                gunScript.gunData = rifle;
                gunScript.gunData.isReloading = false;
            }
        }
    }
}
