using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GunScript : PlayerBehaviour
{
    public GunData gunData;
    bool isShooting;
    float readyTime;
    public GameObject impact, bullet;
    float timeSinceLastShot;
    public TextMeshProUGUI ammoInMagCounter;
    public TextMeshProUGUI ammoLeftCounter;
    public WeaponSwitching weaponSwitching;
    public GameObject camGameObject;
    public Camera camera;
    float camZoomOut = 60f;
    float camZoomIn = 20f;
    float t = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        gunData.currentAmmoInMag = gunData.magazineSize;
        gunData.isReloading = false;
        camera.fieldOfView = camZoomOut;
    }

    // Update is called once per frame
    void Update()
    {
        readyTime = 1f/(gunData.fireRate/60f);
        timeSinceLastShot += Time.deltaTime;

        ZoomingOut(); // Camera zoom Reset

        if(Input.GetKey(KeyCode.Mouse0)){
            shooting();
        }
        if(Input.GetKey(KeyCode.R)){
            Reloading();
        }
        else if(Input.GetKey(KeyCode.Mouse1)){
            SecondaryFire();
        }

        ammoInMagCounter.SetText("AMMO : " + gunData.currentAmmoInMag.ToString());
        ammoLeftCounter.SetText(gunData.ammoLeft.ToString());
    }

    void whatIsWeaponShoot(){
        if(gunData.name == "Shotgun"){
            ShotgunBullet();
        }
        if(gunData.name == "AR" || gunData.name == "Pistol"){
            ARBullet();
        }
        if(gunData.name == "Sniper"){
            SniperBullet();
        }
    }

    void SecondaryFire(){
        if(gunData.name == "Shotgun"){
            
        }
        if(gunData.name == "AR" || gunData.name == "Pistol"){
            
        }
        if(gunData.name == "Sniper"){
            ZoomingIn();
        }
    }

    void ZoomingOut(){
        if(!Input.GetKey(KeyCode.Mouse1) && gunData.name == "Sniper"){
            camera.fieldOfView = Mathf.Lerp(camZoomOut, camZoomIn, t);
        
        if(t>=0.0f){
            t-=4f*Time.deltaTime;
        }
        }
    }
    void ZoomingIn(){
        camera.fieldOfView = Mathf.Lerp(camZoomOut, camZoomIn, t);
        
        if(t<=1.0f){
            t+=4f*Time.deltaTime;
        }
    }
    void shooting(){
        if(!gunData.isReloading && gunData.currentAmmoInMag > 0 && ReadyToShoot()){
            whatIsWeaponShoot();
            timeSinceLastShot = 0f;
            gunData.currentAmmoInMag--;
        }
    }

    // void ARBullet(){
    //     if(Physics.Raycast(camGameObject.transform.position, transform.forward + randomSpread(), out RaycastHit hit,gunData.maxDistance)){
    //         Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
    //     }
    // }

    void ARBullet(){
        BulletPool.Instance.SpawnFromPool("bullet", camGameObject.transform.position, camGameObject.transform.rotation * randomSpread());
    }
    void ShotgunBullet(){
        for(int i=0;i<8;i++){
            BulletPool.Instance.SpawnFromPool("bullet", camGameObject.transform.position, camGameObject.transform.rotation * randomSpread());
        }
    }

    void SniperBullet(){
        BulletPool.Instance.SpawnFromPool("bullet", camGameObject.transform.position, camGameObject.transform.rotation * randomSpread());
    }

    int prevSelectedWeapon;
    void Reloading(){
        bool indexSet = false;
        if(!indexSet){
            prevSelectedWeapon = weaponSwitching.currentWeapon;
            indexSet = true;
        }
        //Debug.Log(prevSelectedWeapon);
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

    Quaternion randomSpread(){
        float x = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);
        float y = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);
        float z = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);

        return new Quaternion(x, y, z, 1);
    }
}
