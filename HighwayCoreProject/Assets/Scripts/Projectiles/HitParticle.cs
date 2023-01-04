using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticle : MonoBehaviour, IPooledBullet
{
    public void onObjectSpawn(){
        Invoke("Destroy", .08f);
    }

    public void Destroy(){
        gameObject.SetActive(false);
    }
    
}
