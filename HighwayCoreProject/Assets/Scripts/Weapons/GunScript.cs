using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GunScript : PlayerBehaviour
{
    public GunData gunData;
    bool isShooting;
    public float fireRate;
    public GameObject impact, bullet;
    float timeSinceLastShot;
    public float timeSinceLastSwitch;
    public TextMeshProUGUI ammoInMagCounter;
    public TextMeshProUGUI ammoLeftCounter;
    public WeaponSwitching weaponSwitching;
    public GameObject camGameObject;
    public float t = 0f;
    bool isScope = false;
    public bool fireInput, secondaryInput;
    public bool isReloading;
    
    // Start is called before the first frame update
    void Start()
    {
        gunData.currentAmmoInMag = gunData.magazineSize;
        gunData.isReloading = false;
    }

    public void FireInput(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
            return;
        fireInput = ctx.started;
    }
    public void SecondaryInput(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
            return;
        secondaryInput = ctx.started;
    }
    public void ReloadInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || player.usingAbility || player.abilityCooldown)
            return;
        
        Reloading();
    }

    // Update is called once per frame
    void Update()
    {
        fireRate = 1f/(gunData.fireRate/60f);
        timeSinceLastShot += Time.deltaTime;
        timeSinceLastSwitch += Time.deltaTime;


        if(fireInput){
            shooting();
        }
        if(secondaryInput){
            SecondaryFire();
        }
        else{
            isScope = false;
        }

        player.Aim.ScopeIn(isScope);

        ammoInMagCounter.SetText("AMMO : " + gunData.currentAmmoInMag.ToString());
        ammoLeftCounter.SetText(gunData.ammoLeft.ToString());

        if(gunData.isReloading || isScope){
            player.usingWeapon = true;
        }
        else{
            player.usingWeapon = false;
        }
        
        isReloading = gunData.isReloading;
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
        if(gunData.name == "Sniper" && !player.usingAbility && !player.abilityCooldown){
            isScope = true;
            //player.Aim.ScopeIn(true);
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
    public bool ReadyToShoot(){
        if(!gunData.isReloading && timeSinceLastShot > fireRate && timeSinceLastSwitch > gunData.switchSpeed){
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
