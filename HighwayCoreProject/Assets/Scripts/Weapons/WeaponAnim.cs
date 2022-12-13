using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnim : PlayerBehaviour
{
    [SerializeField] Animator anim;
    

    // Update is called once per frame
    void Update()
    {
        AnimationStateControl();
    }

    void AnimationStateControl(){
        if(player.Guns.fireInput && !player.Guns.isReloading && player.Guns.gunData.currentAmmoInMag>0){
            anim.SetBool("shoot", true);
            anim.SetBool("reload", false);
        }
        else if(player.Guns.isReloading){
            anim.SetBool("reload", true);
            anim.SetBool("shoot", false);
        }
        else{
            Idle();
        }
    }

    public void Idle(){
        anim.SetBool("shoot", false);
        anim.SetBool("reload", false);
    }

}
