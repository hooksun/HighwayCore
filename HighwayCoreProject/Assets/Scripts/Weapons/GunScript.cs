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
    public TextMeshProUGUI ammoCounter;
    // Start is called before the first frame update
    void Start()
    {
        gunData.currentAmmo = gunData.maxAmmo;
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

        ammoCounter.SetText(gunData.currentAmmo.ToString());
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
        if(!gunData.isReloading && gunData.currentAmmo > 0 && ReadyToShoot()){
            whatIsWeapon();
            timeSinceLastShot = 0f;
            gunData.currentAmmo--;
        }
    }

    void ARBullet(){
        if(Physics.Raycast(transform.position, transform.forward + randomSpread(), out RaycastHit hit,gunData.maxDistance)){
            Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
    void ShotgunBullet(){
        for(int i=0;i<8;i++){
            if(Physics.Raycast(transform.position, transform.forward + randomSpread(), out RaycastHit hit,gunData.maxDistance)){
                Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    void Reloading(){
        if(!gunData.isReloading){
            gunData.isReloading = true;
            Invoke("ReloadingFinished", 1f);
        }
    }
    void ReloadingFinished(){
        gunData.currentAmmo = gunData.maxAmmo;
        gunData.isReloading = false;
    }
    bool ReadyToShoot(){
        if(!gunData.isReloading && timeSinceLastShot > readyTime){
            return true;
        }
        return false;
    }

    Vector3 randomSpread(){
        float x = Random.Range(-.05f, .05f);
        float y = Random.Range(-.05f, .05f);
        float z = Random.Range(-.05f, .05f);

        return new Vector3(x,y,z);
    }
}
