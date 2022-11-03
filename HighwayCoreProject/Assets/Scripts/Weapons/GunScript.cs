using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunScript : MonoBehaviour
{
    public GunData gunData;
    bool isShooting;
    float readyTime;
    public GameObject impact;
    float timeSinceLastShot;
    public TextMeshProUGUI ammoInMagCounter;
    public TextMeshProUGUI ammoLeftCounter;
    public WeaponSwitching weaponSwitching;
    public GameObject cam;
    // Start is called before the first frame update
    void Start()
    {
        gunData.currentAmmoInMag = gunData.magazineSize;
        gunData.isReloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        readyTime = 1f/(gunData.fireRate/60f);
        timeSinceLastShot += Time.deltaTime;
        if(Input.GetKey(KeyCode.Mouse0)){
            shooting();
        }
        else if(Input.GetKey(KeyCode.R)){
            Reloading();
        }

        ammoInMagCounter.SetText("AMMO : " + gunData.currentAmmoInMag.ToString());
        ammoLeftCounter.SetText(gunData.ammoLeft.ToString());
    }

    void whatIsWeapon(){
        if(gunData.name == "Shotgun"){
            ShotgunBullet();
        }
        if(gunData.name == "AR"){
            ARBullet();
        }
    }

    void shooting(){
        if(!gunData.isReloading && gunData.currentAmmoInMag > 0 && ReadyToShoot()){
            whatIsWeapon();
            timeSinceLastShot = 0f;
            gunData.currentAmmoInMag--;
        }
    }

    void ARBullet(){
        if(Physics.Raycast(cam.transform.position, transform.forward + randomSpread(), out RaycastHit hit,gunData.maxDistance)){
            Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
    void ShotgunBullet(){
        for(int i=0;i<8;i++){
            if(Physics.Raycast(cam.transform.position, transform.forward + randomSpread(), out RaycastHit hit,gunData.maxDistance)){
                Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    int prevSelectedWeapon;
    void Reloading(){
        bool indexSet = false;
        if(!indexSet){
            prevSelectedWeapon = weaponSwitching.currentWeapon;
            indexSet = true;
        }
        Debug.Log(prevSelectedWeapon);
        if(!gunData.isReloading){
            gunData.isReloading = true;
            Invoke("ReloadingFinished", 1f);
            
        }
    }
    void ReloadingFinished(){ //This is where the ammo refill
        if(prevSelectedWeapon == weaponSwitching.currentWeapon){
            int numOfBulletNeeded = gunData.magazineSize - gunData.currentAmmoInMag;

            if(numOfBulletNeeded > gunData.ammoLeft){
                numOfBulletNeeded = gunData.ammoLeft;
            }

            if(gunData.ammoLeft > 0){
                gunData.currentAmmoInMag += numOfBulletNeeded;
                gunData.ammoLeft -= numOfBulletNeeded;
            }

            gunData.isReloading = false;
        }
    }
    bool ReadyToShoot(){
        if(!gunData.isReloading && timeSinceLastShot > readyTime){
            return true;
        }
        return false;
    }

    Vector3 randomSpread(){
        float x = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);
        float y = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);
        float z = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);

        return new Vector3(x,y,z);
    }
}
