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
        // if(player.Guns.fireInput && player.Guns.gunData.currentAmmoInMag > 0 && player.Guns.timeSinceLastSwitch > player.Guns.gunData.switchSpeed){
        //     anim.SetBool("shoot", true);
        // }
        anim.SetBool("shoot", player.Guns.fireInput && player.Guns.gunData.currentAmmoInMag > 0&& player.Guns.timeSinceLastSwitch > player.Guns.gunData.switchSpeed);
        anim.SetBool("reload", player.Guns.isReloading);
    }

}
