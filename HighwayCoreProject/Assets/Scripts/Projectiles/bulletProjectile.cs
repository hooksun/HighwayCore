using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletProjectile : MonoBehaviour, IPooledBullet
{
    Vector3 startPos;
    Vector3 currPos;
    [SerializeField] LayerMask groundMask;
    GunScript gundScript;
    private bool hitSomething;

    public void onObjectSpawn()
    {   
        startPos = transform.position;
        gundScript = GameObject.Find("Weapon Holder").GetComponent<GunScript>();
    }

    void Update(){
        currPos = transform.position;
        travel();
    }

    void travel(){
        transform.position += transform.forward * Time.deltaTime * gundScript.gunData.bulletSpeed; 
    }
    

    
}
