using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelAnim : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    public GunScript gun;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.deltaTime == 0f)
            return;
            
        if(gun.isReloading){
            anim.SetBool("reload", true);
        }
        else{
            anim.SetBool("reload", false);
        }
    }
}
