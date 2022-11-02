using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooterScript : MonoBehaviour
{
    public Transform projSpawnPoint;
    public GameObject bullet;
    public string desc = "TESTING";
    private bool isReady;
    
    
    void Start(){
        isReady = true;
    }
    void Update()
    {

        if(Input.GetKey(KeyCode.Mouse1) && isReady){
            Instantiate(bullet, projSpawnPoint.position, projSpawnPoint.rotation);
            isReady = false;
            Invoke("ReadyToShoot", .7f);
        }
    }

    void ReadyToShoot(){
        isReady = true;
    }
}
