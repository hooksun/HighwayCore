using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayGenerator : MonoBehaviour
{
    public EnemyManager enemyManager;
    public VehiclePool Pool;
    public VariablePool<int> Vehicles;
    public Lane[] Lanes;
    public float StartLength;
    public float StartPosition, minGap, maxGap, accelRange, minGapTime, maxGapTime, gapDistance;

    public Transform player; // temp
    public float PlayerGenerateDist;

    void Start()
    {
        GenerateHighway(StartLength);
    }

    void Update()
    {
        SimulateHighway();
    }

    void GenerateHighway(float length)
    {
        length += StartPosition;
        foreach(Lane lane in Lanes)
        {
            float position = StartPosition;
            while(position < length)
            {
                position = AddVehicle(lane, position);
            }
        }
    }

    void SimulateHighway()
    {
        foreach(Lane lane in Lanes)
        {
            if(Mathf.Abs(lane.Vehicles[0].position - player.position.z) > PlayerGenerateDist)
            {
                RemoveVehicle(lane, 0);
            }
            Vehicle lastVehicle = lane.Vehicles[lane.Vehicles.Count - 1];
            if(Mathf.Abs(lastVehicle.position - player.position.z) < PlayerGenerateDist)
            {
                AddVehicle(lane, lastVehicle.position + lastVehicle.length);
            }
            
            for(int i = lane.Vehicles.Count - 2; i >= 0; i--)
            {
                Vehicle vehicle = lane.Vehicles[i];
                Vehicle vehicleFront = lane.Vehicles[i+1];
                float gap = vehicleFront.position - vehicle.position - vehicle.length;
                if(gap < minGap)
                {
                    vehicle.gapTime = Random.Range(minGapTime, Mathf.Min(vehicle.gapTime, vehicleFront.gapTime));
                }
                if(Mathf.Abs(vehicle.targetGap - gap) < gapDistance)
                {
                    vehicle.targetGap = Random.Range(minGap, maxGap);
                    vehicle.gapTime = Random.Range(minGapTime, maxGapTime);
                    continue;
                }
                vehicle.position = vehicleFront.position - Mathf.SmoothDamp(gap, vehicle.targetGap, ref vehicle.speed, vehicle.gapTime) - vehicle.length;
                vehicle.UpdateTransform();
            }
        }
    }

    float AddVehicle(Lane lane, float position)
    {
        Vehicle newVehicle = Pool.GetObject(Vehicles.GetRandomVar());
        lane.Vehicles.Add(newVehicle);
        newVehicle.transform.parent = lane.transform;

        if(newVehicle.isSpawner)
        {
            ((SpawnerVehicle)newVehicle).address = new PlatformAddress(lane, newVehicle, 0);
            enemyManager.Spawners.Add((SpawnerVehicle)newVehicle);
        }

        position += Random.Range(minGap, maxGap);
        newVehicle.position = position;
        newVehicle.speed = 0f;
        newVehicle.UpdateTransform();
        newVehicle.targetGap = Random.Range(minGap, maxGap);
        newVehicle.gapTime = Random.Range(minGapTime, maxGapTime);
        return position + newVehicle.length;
    }

    void RemoveVehicle(Lane lane, int index = 0)
    {
        Vehicle vehicle = lane.Vehicles[index];
        vehicle.gameObject.SetActive(false);
        vehicle.transform.parent = Pool.transform;
        lane.Vehicles.RemoveAt(index);
        if(index == lane.Vehicles.Count)
            lane.Vehicles[lane.Vehicles.Count - 1].speed = 0f;

        if(vehicle.isSpawner)
        {
            enemyManager.Spawners.Remove((IEnemySpawner)vehicle);
        }
    }
}