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
        
        if(gun.isShooting && gun.gunData.currentAmmoInMag > 0)
            Shoot();
        else if(gun.isReloading){
            Reload();
        }
        //timeSinceLastSwitch > gunData.switchSpeed
        else if (gun.timeSinceLastSwitch < gun.gunData.switchSpeed){
            Equip();
        }
        else{
            Idle();
        }
    }

    public void WhatIsGun(){
        anim.runtimeAnimatorController = gun.gunData.animator;
        // if(gun.gunData.name == "Sniper"){
        //     //anim.runtimeAnimatorController = Resources.Load("Sniper/Player_Animator_Sniper") as RuntimeAnimatorController;
        //     Animator sniper = GameObject.Find("SniperAnim").GetComponent<Animator>();
        //     runTimeAnim = sniper.runtimeAnimatorController;
        //     anim.runtimeAnimatorController = runTimeAnim;
        // }
        // else if(gun.gunData.name == "AR"){
        //     //anim.runtimeAnimatorController = Resources.Load("AR/PlayerAnimator_AR") as RuntimeAnimatorController;
        //     Animator sniper = GameObject.Find("ARAnim").GetComponent<Animator>();
        //     runTimeAnim = sniper.runtimeAnimatorController;
        //     anim.runtimeAnimatorController = runTimeAnim;
        // }
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
