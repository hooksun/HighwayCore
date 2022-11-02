using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public Transform[] weapons;
    public GunData shotgun;
    public GunData rifle;
    public GunScript gunScript;
    int currentWeapon;
    int prevSelectedWeapon;
    

    // Start is called before the first frame update
    void Start()
    {
        findWeapon();
        currentWeapon = 0;
        selectWeapon(currentWeapon);
        prevSelectedWeapon = -1;
    }

    // Update is called once per frame
    void Update()
    {
        prevSelectedWeapon = currentWeapon;
        
        for(int i=0;i<keyCodes.Length;i++){
            if(Input.GetKeyDown(keyCodes[i])){
                currentWeapon = i;
                Debug.Log(currentWeapon);
            }
        }
        if(prevSelectedWeapon!=currentWeapon){
            selectWeapon(currentWeapon);
        }
        changeGunData();
    }

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1, 
        KeyCode.Alpha2
    };

    void findWeapon(){
        weapons = new Transform[transform.childCount];
        for(int i=0;i<transform.childCount;i++){
            weapons[i] = transform.GetChild(i);
        }
    }
    void selectWeapon(int index){
        for(int i=0;i<weapons.Length;i++){
            weapons[i].gameObject.SetActive(i == index);
        }
    }

    void changeGunData(){
        if(currentWeapon == 0){
            gunScript.gunData = shotgun;
        }
        else if(currentWeapon == 1){
            gunScript.gunData = rifle;
        }
    }
}
