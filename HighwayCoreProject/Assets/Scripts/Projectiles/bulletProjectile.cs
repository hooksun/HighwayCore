using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletProjectile : MonoBehaviour
{
    Vector3 startPos;
    Vector3 currPos;
    [SerializeField] LayerMask groundMask;
    GunScript gundScript;
    private bool collide;

    void Start()
    {   
        startPos = transform.position;
        gundScript = GameObject.Find("Weapon Holder").GetComponent<GunScript>();
    }

    void Update(){
        currPos = transform.position;
        travel();
        
        DestroyObj();
    }

    void travel(){
        transform.position += transform.forward * Time.deltaTime * gundScript.gunData.bulletSpeed; 
    }
    void DestroyObj(){
        collide = Physics.CheckSphere(transform.position, .1f, groundMask);
        if(collide){
            Destroy(gameObject);
        }
        if(Vector3.Distance(startPos, currPos) > 10f){
            Destroy(gameObject);
        }
    }

    
}
