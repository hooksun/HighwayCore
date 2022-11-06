using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float smooth;
    public float swayMultiplayer;

    private void Update(){
        float mouseX = Input.GetAxisRaw("Mouse X") *  swayMultiplayer ;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplayer;




        Quaternion rotationX = Quaternion.AngleAxis( mouseY, Vector3.left);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);


        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

}