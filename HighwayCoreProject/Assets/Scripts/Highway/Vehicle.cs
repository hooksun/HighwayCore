using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour, IMovingGround
{
    public float position;
    public float length, speed, accel, targetGap, gapTime;
    public Vector3 transformOffset;
    public Platform[] Platforms;
    public virtual bool isSpawner{get => false;}

    public void UpdateTransform()
    {
        transform.localPosition = Vector3.forward * (position) + transformOffset;
    }

    public Vector3 velocity{get => Vector3.forward * -speed;}
}

[System.Serializable]
public class Platform
{
    public float height;
    public Vector2 BoundsStart, BoundsEnd;
}