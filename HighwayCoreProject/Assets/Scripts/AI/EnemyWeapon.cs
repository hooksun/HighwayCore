using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : EnemyBehaviour
{
    public Transform firePoint;
    public float damage, fireRate, reloadSpeed, bulletSpread, bulletSpeed;
    public int magSize, bulletsPerShot, bulletIndex;
    public LayerMask hitMask;
    public string shootAnimation, reloadAnimation;
    public AudioSource shootSound;

    [HideInInspector] public bool shoot, cantShoot, reloading;
    float fireTime, reloadTime;
    int mag;

    public override void Activate()
    {
        shoot = false;
        cantShoot = false;
        reloading = false;
        fireTime = 0f;
        reloadTime = 0f;
        mag = magSize;
    }

    public override void Stun(Vector3 knockback)
    {
        if(!cantShoot)
            return;
        reloadTime = reloadSpeed;
        reloading = false;
    }

    void Update()
    {
        Shoot();
        Reload();

        if(enemy.stunned)
            return;

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
            return;
        }

        FireBullet();

        fireTime += 60f/fireRate;
    }

    protected virtual void FireBullet()
    {
        for(int i = 0; i < bulletsPerShot; i++)
        {
            Projectile proj = ProjectilePool.GetObject(bulletIndex, false);
            proj.Initiate(enemy.Head.position, enemy.Head.rotation, firePoint.position, bulletSpeed, damage, bulletSpread, hitMask);
        }
        mag--;
        enemy.Animation.Play(shootAnimation);
        shootSound.Play();
    }

    protected virtual void Reload()
    {
        if(cantShoot && !reloading)
        {
            reloading = true;
            reloadTime = reloadSpeed;
            enemy.Animation.Play(reloadAnimation);
        }

        if(!cantShoot || reloadTime > 0)
            return;
        
        mag = magSize;
        cantShoot = false;
        reloading = false;
    }
}
