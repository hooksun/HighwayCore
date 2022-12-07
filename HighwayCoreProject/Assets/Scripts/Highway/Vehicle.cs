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

    public PlatformAddress ClosestPlatform(Vector3 point)
    {
        int min = 0;
        float dist = DistanceToPlatform(point, 0);
        for(int i = 1; i < Platforms.Length; i++)
        {
            if(dist == 0f)
                break;
            float newDist = DistanceToPlatform(point, i);
            if(newDist <= dist)
            {
                min = i;
                dist = newDist;
            }
        }
        return new PlatformAddress(transform.parent.GetComponent<Lane>(), this, min);
    }
    public float DistanceToPlatform(Vector3 point, int index)
    {
        float dist = 0f;
        Platform platform = Platforms[index];
        point -= transform.position - transformOffset;
        dist += Mathf.Max(0f, platform.BoundsStart.x - point.x);
        dist += Mathf.Max(0f, platform.BoundsStart.y - point.z);
        dist += Mathf.Max(0f, point.x - platform.BoundsEnd.x);
        dist += Mathf.Max(0f, point.z - platform.BoundsEnd.y);
        return dist;
    }

    public Vector3 velocity{get => Vector3.forward * -speed;}
}

[System.Serializable]
public class Platform
{
    public float height;
    public Vector2 BoundsStart, BoundsEnd;
}