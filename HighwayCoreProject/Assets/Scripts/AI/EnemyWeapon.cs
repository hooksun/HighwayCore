using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : EnemyBehaviour
{
    public Transform firePoint, muzzleFlash;
    public float damage, fireRate, muzzleTime = 0.05f, reloadSpeed, idleTime = 1f, bulletSpread, bulletSpeed, burstCooldown;
    public int burstAmount, magSize, bulletsPerShot, bulletIndex;
    public LayerMask hitMask;
    public string shootAnimation, reloadAnimation;
    public Audio shootSound, reloadSound;

    [HideInInspector] public bool shoot, cantShoot, reloading;
    float fireTime, burstTime, reloadTime;
    int mag, burst;

    public override void Activate()
    {
        shoot = false;
        cantShoot = false;
        reloading = false;
        fireTime = 0f;
        burstTime = 0f;
        reloadTime = 0f;
        mag = magSize;
        muzzleFlash.gameObject.SetActive(false);
        burst = 0;
    }

    public override void Stun(Vector3 knockback)
    {
        if(!cantShoot)
            return;
        if(reloadTime > idleTime)
        {
            reloadTime = reloadSpeed+idleTime;
            reloading = false;
            reloadSound.Stop();
        }
    }

    void Update()
    {
        if(Time.deltaTime == 0f || enemy.stunned)
            return;

        Shoot();
        Reload();

        if(enemy.stunned)
            return;

        if(fireTime > 0)
            fireTime -= Time.deltaTime;
        if(burstTime > 0)
            burstTime -= Time.deltaTime;
        if(reloadTime > 0)
            reloadTime -= Time.deltaTime;
    }

    protected virtual void Shoot()
    {
        if(burstTime <= 0f)
        {
            burst = 0;
        }

        if(mag <= 0)
        {
            InitReload();
            return;
        }

        if(!shoot || fireTime > 0 || cantShoot)
            return;
        

        if(burst >= burstAmount)
            return;

        FireBullet();

        burstTime = burstCooldown;
        fireTime += 60f/fireRate;
        mag--;
        burst++;
    }

    protected virtual void FireBullet()
    {
        for(int i = 0; i < bulletsPerShot; i++)
        {
            Projectile proj = ProjectilePool.GetObject(bulletIndex);
            proj.Initiate(enemy.Head.position, enemy.Head.rotation, firePoint.position, bulletSpeed, damage, bulletSpread, hitMask, true);
        }
        enemy.Animation.Play(shootAnimation, 1);
        shootSound.Play();
        StartCoroutine(MuzzleFlash());
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlash.Rotate(Vector3.forward * Random.Range(0f, 360f));
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(muzzleTime);
        muzzleFlash.gameObject.SetActive(false);
    }

    public virtual void InitReload()
    {
        if(mag == magSize)
            return;
        cantShoot = true;
        shoot = false;
    }

    protected virtual void Reload()
    {
        if(cantShoot && !reloading)
        {
            reloading = true;
            reloadTime = reloadSpeed + idleTime;
            enemy.Animation.Play(reloadAnimation, 1);
            reloadSound.Play();
        }

        if(!cantShoot || reloadTime > 0)
            return;
        
        mag = magSize;
        cantShoot = false;
        reloading = false;
        burst = 0;
    }
}
