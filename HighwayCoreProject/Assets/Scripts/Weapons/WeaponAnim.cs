using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnim : PlayerBehaviour
{
    [SerializeField] Animator anim;
    

    void Update()
    {
        
    }
    
    public void Reload(){
        anim.SetBool("reload", true);
    }
    public void Idle(){
        
    }
}
