using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GunScript : PlayerBehaviour
{
    public GunData gunData;
    public Gun gun;
    public bool isShooting;
    float fireRate;
    float timeSinceLastShot;
    public float timeSinceLastSwitch;
    public WeaponSwitching weaponSwitching;
    public Transform scopedFirePoint;
    bool isScope = false;
    public bool fireInput, secondaryInput, reloadInput;
    public bool isReloading;
    public float scopeTime;
    public LayerMask bulletMask;
    public Audio ShootSound;
    public AudioSequence ReloadSound;
    public WeaponAnim anim;

    float spreadMulti;

    
    // Start is called before the first frame update
    void Start()
    {
        gunData.currentAmmoInMag = gunData.magazineSize;
        gunData.isReloading = false;
        isShooting = false;
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
        if(!ctx.started)
            return;
        
        reloadInput = true;
        //Reloading();
    }

    public void SwitchWeapon(Gun newGun)
    {
        gunData.isReloading = false;
        gunData.unequipedTime = 0f;
        gun = newGun;
        gunData = gun.data;
        gunData.isReloading = false;
        gun.Reset();
        if(gunData.unequipedTime > gunData.unequipedReloadTime)
            ReloadAmmo();
        UIManager.SetAmmo(gunData.currentAmmoInMag);
        UIManager.SetReserve(gunData.ammoLeft);
        timeSinceLastSwitch = 0f;
        secondaryInput = reloadInput = false;
    }

    // Update is called once per frame
    void Update()
    {
        // curWeapon = transform.GetChild(weaponSwitching.currentWeapon).gameObject;
        // anim = curWeapon.GetComponentInChildren<WeaponAnim>();
        // anim.Reload();

        if(Time.deltaTime == 0f)
            return;

        fireRate = 60f/gunData.fireRate;
        timeSinceLastShot += Time.deltaTime;
        timeSinceLastSwitch += Time.deltaTime;


        if(reloadInput)
        {
            Reloading();
        }
        if(fireInput){
            if(!player.Melee.isPunching)
                shooting();
        }
        else{
            isShooting = false;
            //Invoke("StopShooting", .4f);
        }
        if(secondaryInput){
            SecondaryFire();
        }
        else{
            isScope = false;

        }

        player.Aim.ScopeIn(isScope, scopeTime);
        spreadMulti = (isScope?Mathf.MoveTowards(spreadMulti, 0f, Time.deltaTime / scopeTime):1f);

        //ammoInMagCounter.SetText("AMMO : " + gunData.currentAmmoInMag.ToString());
        //ammoLeftCounter.SetText(gunData.ammoLeft.ToString());
        UIManager.SetAmmo(gunData.currentAmmoInMag);
        UIManager.SetReserve(gunData.ammoLeft);
        UIManager.SetCrosshairSpread(gunData.bulletSpread * spreadMulti);

        if(gunData.isReloading || isScope){
            player.usingWeapon = true;
        }
        else{
            player.usingWeapon = false;
        }
        
        isReloading = gunData.isReloading;
    }
    void StopShooting(){
        isShooting = false;
    }

    void whatIsWeaponShoot(){
        ARBullet();
    }

    void SecondaryFire(){
        if(gunData.name == "Sniper" && !player.usingAbility && !player.abilityCooldown && ReadyToScope() && !gunData.isReloading){
            isScope = true;
            //player.Aim.ScopeIn(true);
        }
    }

    void shooting(){
        if(ReadyToShoot()){
            if(gunData.currentAmmoInMag <= 0)
            {
                Reloading();
                return;
            }
            whatIsWeaponShoot();
            gun.ShootGfx();
            reloadInput = false;
            timeSinceLastShot = 0f;
            gunData.currentAmmoInMag--;
        }
        else{
            isShooting = false;
        }
    }

    // void ARBullet(){
    //     if(Physics.Raycast(camGameObject.transform.position, transform.forward + randomSpread(), out RaycastHit hit,gunData.maxDistance)){
    //         Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
    //     }
    // }

    void ARBullet(){
        ShootSound.clip = gunData.shootAudio;
        ShootSound.Play();
        if(!player.Melee.punchCooldown)
            anim.anim.SetTrigger("shootTrigger");
        isShooting = true;
        //anim.anim.SetTrigger("shoot");
        for(int i = 0; i < gunData.bulletsPerShot; i++)
        {
            Projectile bullet = ProjectilePool.GetObject();
            Vector3 firePoint = (UIManager.scoping?scopedFirePoint.position:gun.firePoint.position);
            bullet.Initiate(player.Head.position,player.Head.rotation,firePoint,gunData.bulletSpeed,gunData.damage,gunData.bulletSpread * spreadMulti,bulletMask);
        }
    }

    int prevSelectedWeapon;
    int prevNumOfswitch;
    void Reloading(){
        if(timeSinceLastShot > gunData.reloadRecovery && !player.usingAbility && !player.abilityCooldown){
            prevSelectedWeapon = weaponSwitching.currentWeapon;
            prevNumOfswitch = weaponSwitching.numOfSwitch;
            //Debug.Log(prevSelectedWeapon);
            if(!gunData.isReloading && gunData.ammoLeft > 0 && gunData.currentAmmoInMag < gunData.magazineSize && prevNumOfswitch == weaponSwitching.numOfSwitch && timeSinceLastSwitch > gunData.switchSpeed){
                gunData.isReloading = true;
                isShooting = false;
                isScope = false;
                player.usingWeapon = true;

                ReloadSound.SetSequence(gunData.reloadSequence, gunData.reloadDelay);
                ReloadSound.Play();
                StartCoroutine(ReloadingFinished(gunData.reloadTime, weaponSwitching.numOfSwitch));
                reloadInput = false;
            }
        }
    }
    
    void PlaySound(AudioClip clip, float time){
        
    }
    IEnumerator ReloadingFinished(float reloadTime, int numOfSwitch){ //This is where the ammo refill
        yield return new WaitForSeconds(reloadTime);
        if(numOfSwitch == weaponSwitching.numOfSwitch){
            
            ReloadAmmo();
            gunData.isReloading = false;
        }
    }
    void ReloadAmmo()
    {
        int numOfBulletNeeded = gunData.magazineSize - gunData.currentAmmoInMag;

        if(numOfBulletNeeded > gunData.ammoLeft){
            numOfBulletNeeded = gunData.ammoLeft;
        }

        if(gunData.ammoLeft > 0){
            gunData.currentAmmoInMag += numOfBulletNeeded;
            gunData.ammoLeft -= numOfBulletNeeded;
        }
    }
    public bool ReadyToShoot(){
        if(!gunData.isReloading && timeSinceLastShot > fireRate && timeSinceLastSwitch > gunData.switchSpeed){
            return true;
        }
        else{
            //isShooting = false;
            return false;
        }
    }
    public bool ReadyToScope(){
        if(!gunData.isReloading && timeSinceLastShot > gunData.reloadRecovery && timeSinceLastSwitch > gunData.switchSpeed){
            return true;
        }
        else{
            //isShooting = false;
            return false;
        }
    }

    Quaternion randomSpread(){
        float x = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);
        float y = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);
        float z = Random.Range(-gunData.bulletSpread, gunData.bulletSpread);

        return new Quaternion(x, y, z, 1);
    }
}
