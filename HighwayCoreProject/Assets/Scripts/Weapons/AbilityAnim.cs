using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAnim : MonoBehaviour
{
    public Animator anim;
    public PlayerMelee melee;


    // Update is called once per frame
    void Update()
    {
        if(Time.deltaTime > 0f)
            anim.SetBool("punching", melee.isPunching);
    }
}
