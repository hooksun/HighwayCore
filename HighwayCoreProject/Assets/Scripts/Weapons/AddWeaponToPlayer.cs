using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWeaponToPlayer : MonoBehaviour
{
    public bool inRange;
    public bool pickedUp;
    public float radius;
    public LayerMask playerMask;
    public GunData thisWeapon;
    // Start is called before the first frame update
    void Start()
    {
        pickedUp = false;
        inRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        inRange = Physics.CheckSphere(transform.position, radius, playerMask);

        if(pickedUp && !thisWeapon.available){
            SendToPlayer();
        }
    }

    void SendToPlayer(){
        thisWeapon.available = true;
        Destroy(gameObject);
    }
}
