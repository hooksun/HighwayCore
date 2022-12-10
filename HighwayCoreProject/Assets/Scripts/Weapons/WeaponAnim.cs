using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnim : PlayerBehaviour
{
    [SerializeField] Animator anim;
    public string shootAnim;
    public string reloadAnim;
    public string idleAnim;
    
    void Start()
    {
        //Play(reloadAnim);
    }

    void Update()
    {   
        animation();
    }

    void animation(){
        if(player.Guns.fireInput && player.Guns.gunData.currentAmmoInMag > 0 && player.Guns.ReadyToShoot()){
            anim.SetBool("shoot", true);
        }
        
        anim.SetBool("shoot", player.Guns.fireInput && player.Guns.gunData.currentAmmoInMag > 0);
        anim.SetBool("reload", player.Guns.isReloading);
    }

     
    void LoopReset(){
        anim.SetBool("shoot", false);
    }

}
