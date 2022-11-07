using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageSource : PlayerBehaviour
{
    public float fallDamage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fall();
    }

    void fall()
    {
        if(!player.Status.isFallen && player.Movement.isGrounded && player.Movement.groundInfo.transform.gameObject.layer == 8)
        {
            player.Status.isFallen = true;
            player.Status.TakeDamage(fallDamage);
        }
    }
}
