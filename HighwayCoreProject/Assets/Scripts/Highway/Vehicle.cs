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

public struct PlatformAddress
{
    public Lane lane;
    public Vehicle vehicle;
    public int platformIndex;

    public PlatformAddress(Lane _lane, Vehicle veh, int platI)
    {
        lane = _lane;
        vehicle = veh;
        platformIndex = platI;
    }

    public int laneIndex{get => lane.transform.GetSiblingIndex();}
    public int vehicleIndex{get => vehicle.transform.GetSiblingIndex();}
    public Platform platform{get => vehicle.Platforms[platformIndex];}
    public bool enabled{get => lane != null && vehicle != null && vehicle.gameObject.activeInHierarchy;}

    public TransformPoint ClosestPointTo(Vector3 point)
    {
        point += vehicle.transformOffset - vehicle.transform.position;
        point.x = Mathf.Clamp(point.x, platform.BoundsStart.x, platform.BoundsEnd.x);
        point.z = Mathf.Clamp(point.z, platform.BoundsStart.y, platform.BoundsEnd.y);
        point -= vehicle.transformOffset;
        point.y = platform.height - vehicle.transformOffset.y;
        return new TransformPoint(vehicle.transform, point);
    }

    public TransformPoint CenterPoint()
    {
        Vector3 point = Vector3.zero;
        point.x = (platform.BoundsStart.x + platform.BoundsEnd.x) * .5f;
        point.z = (platform.BoundsStart.y + platform.BoundsEnd.y) * .5f;
        point -= vehicle.transformOffset;
        point.y = platform.height - vehicle.transformOffset.y;
        return new TransformPoint(vehicle.transform, point);
    }

    public TransformPoint RandomPoint()
    {
        Vector3 point = Vector3.zero;
        point.x = Random.Range(platform.BoundsStart.x, platform.BoundsEnd.x);
        point.z = Random.Range(platform.BoundsStart.y, platform.BoundsEnd.y);
        point -= vehicle.transformOffset;
        point.y = platform.height - vehicle.transformOffset.y;
        return new TransformPoint(vehicle.transform, point);
    }

    public float DistanceFrom(Vector3 point) => vehicle.DistanceToPlatform(point, platformIndex);
}