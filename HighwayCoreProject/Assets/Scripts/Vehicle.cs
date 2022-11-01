using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour, IMovingGround
{
    public float position;
    public float length, speed, accel, targetGap, gapTime;

    public void UpdateTransform()
    {
        transform.localPosition = Vector3.forward * (position - length * 0.5f);
    }

    public Vector3 velocity{get => Vector3.forward * speed;}
}