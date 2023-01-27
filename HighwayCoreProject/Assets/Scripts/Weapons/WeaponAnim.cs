using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnim : PlayerBehaviour
{
    public Animator anim;
    public GunScript gun;
    RuntimeAnimatorController runTimeAnim;
    public PlayerMelee melee;

    bool punch = false;
    

    void Update()
    {
        punch = melee.isPunching;
        WhatIsGun();
        anim.SetBool("punching", punch);

        if(gun.timeSinceLastSwitch < gun.gunData.switchSpeed){
            Equip();
        }
        else if(gun.isShooting){
            Shoot();
        }
        else if(gun.isReloading){
            Reload();
        }
        else{
            Idle();
        }
    }

    public void WhatIsGun(){
        anim.runtimeAnimatorController = gun.gunData.animator;
        
    }
    
    public void Reload(){
        anim.SetBool("Ready", true);
        anim.SetBool("reload", true);
        anim.SetFloat("Blend", 0f);
        anim.SetBool("idle", false);
        //anim.SetBool("punching", false);
    }
    public void Shoot(){
        anim.SetBool("Ready", true);
        anim.SetBool("reload", false);
        anim.SetFloat("Blend", 1f);
        anim.SetBool("idle", false);
        //anim.SetBool("punching", false);
    }
    public void Equip(){
        anim.SetBool("Ready", false);
        anim.SetBool("reload", false);
        anim.SetFloat("Blend", 0f);
        anim.SetBool("idle", false);
       //anim.SetBool("punching", false);
    }
    public void Idle(){
        anim.SetBool("Ready", true);
        anim.SetBool("reload",false);
        anim.SetFloat("Blend", 0f);
        anim.SetBool("idle", true);
        //anim.SetBool("punching", false);
    }
    public void Punch(){
        anim.SetBool("punching", true);
    }
}
