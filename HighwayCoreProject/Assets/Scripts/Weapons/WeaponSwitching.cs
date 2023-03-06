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
        if(Time.deltaTime == 0f || player.dead)
            return;

        for(int i=0;i<keyCodes.Length;i++){
            if(Input.GetKeyDown(keyCodes[i])){
                currentWeapon = i;
            }
        }
        if(!player.Melee.isPunching && prevSelectedWeapon!=currentWeapon){
            //player.weaponAnim.Idle();
            prevSelectedWeapon = currentWeapon;
            selectWeapon(currentWeapon);
            numOfSwitch++;
        }
        //changeGunData();
        for(int i=0;i<weapons.Length;i++){
            if(i == prevSelectedWeapon)
                continue;
            weapons[i].data.unequipedTime += Time.deltaTime;
        }
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
        UIManager.SetWeapon(index);
        gunScript.ReloadSound.Stop();
        for(int i=0;i<weapons.Length;i++){
            weapons[i].gameObject.SetActive(i == index);
        }
        gunScript.SwitchWeapon(weapons[index]);
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
