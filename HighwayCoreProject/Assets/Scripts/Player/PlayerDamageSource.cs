using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageSource : PlayerBehaviour
{
    public Transform groundChecker;
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

    void fall(){
        if(Physics.Raycast(groundChecker.transform.position, Vector3.down, out RaycastHit hit, .2f)){
            if(hit.collider.name == "Road" && !player.Status.isFallen){
                player.Status.TakeDamage(fallDamage);
                player.Status.isFallen = true;
            }
        }
    }
}
