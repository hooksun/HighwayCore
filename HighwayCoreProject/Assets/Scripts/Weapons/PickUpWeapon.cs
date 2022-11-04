using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    public GameObject cam;
    
    void Update()
    {
        lookAt();
    }

    void lookAt(){
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit)){
            AddWeaponToPlayer weapon = hit.collider.gameObject.GetComponent<AddWeaponToPlayer>();
            if((hit.collider.name == "AssaultRifleObj" ||
            hit.collider.name == "ShotgunObj")&& weapon.inRange){
                Debug.Log("E to interact");
                if(Input.GetKey(KeyCode.E)){
                    weapon.pickedUp = true;
                }
            }
        }
    }
}
