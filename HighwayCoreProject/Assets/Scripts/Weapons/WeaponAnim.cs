using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnim : PlayerBehaviour
{
    [SerializeField] Animator anim;
    public GunScript gun;
    

    void Update()
    {
        if(gun.isShooting)
            Shoot();
        else if(gun.isReloading){
            Reload();
        }
        else{
            Idle();
        }
    }
    
    public void Reload(){
        anim.SetBool("reload", true);
    }
    public void Shoot(){
        //anim.SetBool("shoot", true);
        anim.SetFloat("Blend", 1f);
    }
    public void Idle(){
        anim.SetBool("reload",false);
        anim.SetFloat("Blend", 0f);
    }
}
