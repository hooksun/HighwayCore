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
        collided = Physics.SphereCast(transform.position, .1f, transform.forward, out collideInfo, .1f);

        if(collided && collideInfo.transform.gameObject.layer == 7 ){
            Debug.Log("hit enemy");
            EnemyHealth enemyHealth = collideInfo.collider.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(-1*damage); //delete -1 biar kasih damage, buat test aja karena .Die() blom ada
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
