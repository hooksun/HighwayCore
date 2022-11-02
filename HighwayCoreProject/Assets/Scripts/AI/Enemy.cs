using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float DesiredDistance, WalkSpeed, MaxJumpHeight;
    public PlatformAddress currentPlatform;


}

public struct PlatformAddress
{
    public Lane lane;
    public int laneIndex, vehicleIndex, platformIndex;

    public PlatformAddress(Lane _lane, int laneI, int vehI, int platI)
    {
        lane = _lane;
        laneIndex = laneI;
        vehicleIndex = vehI;
        platformIndex = platI;
    }

    public Vehicle vehicle{get => lane.Vehicles[vehicleIndex];}
    public Platform platform{get => vehicle.Platforms[platformIndex];}
}