using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : EnemyBehaviour, IProjectileSpawner
{
    public Transform firePoint;
    public float damage, fireRate, reloadSpeed, bulletSpread, bulletSpeed;
    public int magSize, bulletsPerShot, bulletIndex;
    public LayerMask hitMask;
    public string shootAnimation, reloadAnimation;

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
            enemy.Animation.Play(reloadAnimation);
            return;
        }

        FireBullet();

        fireTime = 60f/fireRate;
    }

    protected virtual void FireBullet()
    {
        for(int i = 0; i < bulletsPerShot; i++)
        {
            Projectile proj = enemy.manager.projectilePool.GetObject(bulletIndex, false);
            proj.Initiate(enemy.Head.position, enemy.Head.rotation * Projectile.RandomSpread(bulletSpread), firePoint.position, bulletSpeed, hitMask, this);
        }
        mag--;
        enemy.Animation.Play(shootAnimation);
    }

    protected virtual void Reload()
    {
        if(!cantShoot || reloadTime > 0)
            return;
        
        mag = magSize;
        cantShoot = false;
    }

    public void OnTargetHit(RaycastHit hit)
    {
        IHurtBox hurtBox = hit.transform.GetComponent<IHurtBox>();
        if(hurtBox != null)
        {
            hurtBox.TakeDamage(damage);
        }
    }
    public void OnTargetNotFound(){}
    public void OnReset(){}
}
