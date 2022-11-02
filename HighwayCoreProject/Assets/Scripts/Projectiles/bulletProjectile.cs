using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    Vector3 startPos;
    Vector3 currPos;
    [SerializeField] LayerMask groundMask;
    private bool collide;

    void Start()
    {   
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward*500f);


    }

    void Update(){
        DestroyObj();
    }

    void DestroyObj(){
        collide = Physics.CheckSphere(transform.position, .5f, groundMask);
        if(collide){
            Destroy(gameObject);
        }
        if(Vector3.Distance(startPos, currPos) > 10f){
            Destroy(gameObject);
        }
    }

    
}
