using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpWeapon : PlayerBehaviour
{
    public GameObject interactUI;
    public GameObject cam;
    
    void Update()
    {
        lookAt();
    }

    void lookAt(){
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit)){
            AddWeaponToPlayer weapon = hit.collider.gameObject.GetComponent<AddWeaponToPlayer>();
            
            if((hit.collider.name == "AssaultRifleObj" ||
            hit.collider.name == "ShotgunObj" ||
            hit.collider.name == "SniperObj") && weapon.inRange){
                //Debug.Log("E to interact");
                interactUI.SetActive(true);
                if(Input.GetKey(KeyCode.E)){
                    weapon.pickedUp = true;
                }
            }
            else{
                interactUI.SetActive(false);
            }
        }
    }
}
