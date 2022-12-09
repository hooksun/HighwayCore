using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : EnemyBehaviour
{
    public Transform firePoint;
    public float damage, fireRate, reloadSpeed;
    public int magSize;

    [HideInInspector] public bool shoot, cantShoot;
    float fireTime, reloadTime;
    int mag;

    public override void Activate()
    {
        shoot = false;
        cantShoot = false;
        fireTime = 0f;
        reloadTime = 0f;
        mag = magSize;
    }

    void Update()
    {
        Shoot();
        Reload();

        if(fireTime > 0)
            fireTime -= Time.deltaTime;
        if(reloadTime > 0)
            reloadTime -= Time.deltaTime;
    }

    protected virtual void Shoot()
    {
        if(!shoot || fireTime > 0 || cantShoot)
            return;
        
        if(mag <= 0)
        {
            cantShoot = true;
            shoot = false;
            reloadTime = reloadSpeed;
            return;
        }

        //fire bullet

        mag--;
        fireTime = 60f/fireRate;
    }

    protected virtual void Reload()
    {
        if(!cantShoot || reloadTime > 0)
            return;
        
        mag = magSize;
        cantShoot = false;
    }
}
