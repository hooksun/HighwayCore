using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletProjectile : MonoBehaviour, IPooledBullet
{
    Vector3 startPos;
    Vector3 currPos;
    [SerializeField] LayerMask enemyMask;
    GunScript gundScript;
    public bool hitSomething;
    bool collided;
    RaycastHit collideInfo;
    float speed;
    float damage;

    public void onObjectSpawn()
    {   
        startPos = transform.position;
        gundScript = GameObject.Find("Weapon Holder").GetComponent<GunScript>();
        hitSomething = false;
        speed = gundScript.gunData.bulletSpeed;
        damage = gundScript.gunData.damage;
    }

    void Update(){
        currPos = transform.position;
        travel();
        hit();
        DestroyOnMaxDistance();
    }

    void hit(){
        //collided = Physics.SphereCast(transform.position, .2f, transform.forward, out collideInfo, .2f);
        collided = Physics.Raycast(transform.position, transform.forward, out collideInfo, .2f);
        if(collided && collideInfo.transform.gameObject.layer == 7 ){
            Debug.Log("hit enemy");
            Enemy enemy = collideInfo.collider.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            gameObject.SetActive(false);
            
        }
    }
    
    void DestroyOnMaxDistance(){
        if(Vector3.Distance(transform.position,startPos) > gundScript.gunData.maxDistance){
            gameObject.SetActive(false);
        }
    }
    void travel(){
        transform.position += transform.forward * Time.deltaTime * speed; 
    }
    

    
}
