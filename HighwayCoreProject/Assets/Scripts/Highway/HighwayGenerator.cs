using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayGenerator : MonoBehaviour
{
    public VariablePool<int> Vehicles;
    public VehiclePool Pool;
    public Lane[] Lanes;
    public float StartLength;
    public float StartPosition, minGap, maxGap, accelRange, minGapTime, maxGapTime;

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
            Vehicle firstVehicle = lane.Vehicles[0];
            if(Mathf.Abs(firstVehicle.position - firstVehicle.length - player.position.z) > PlayerGenerateDist)
            {
                firstVehicle.gameObject.SetActive(false);
                firstVehicle.transform.parent = Pool.transform;
                lane.Vehicles.RemoveAt(0);
            }
            Vehicle lastVehicle = lane.Vehicles[lane.Vehicles.Count - 1];
            if(Mathf.Abs(lastVehicle.position - player.position.z) < PlayerGenerateDist)
            {
                AddVehicle(lane, lastVehicle.position);
            }
            
            for(int i = 1; i < lane.Vehicles.Count; i++)
            {
                Vehicle vehicle = lane.Vehicles[i];
                Vehicle vehicleBehind = lane.Vehicles[i-1];
                float gap = vehicle.position - vehicle.length - vehicleBehind.position;
                if(gap < minGap)
                {
                    vehicle.gapTime = Random.Range(minGapTime, Mathf.Min(vehicle.gapTime, vehicleBehind.gapTime));
                }
                if(Mathf.Abs(vehicle.targetGap - gap) < .2f)
                {
                    vehicle.targetGap = Random.Range(minGap, maxGap);
                    vehicle.gapTime = Random.Range(minGapTime, maxGapTime);
                    continue;
                }
                vehicle.position = Mathf.SmoothDamp(gap, vehicle.targetGap, ref vehicle.speed, vehicle.gapTime) + vehicle.length + vehicleBehind.position;
                vehicle.UpdateTransform();
            }
        }
    }

    float AddVehicle(Lane lane, float position)
    {
        Vehicle newVehicle = Pool.GetVehicle(Vehicles.GetRandomVar());
        lane.Vehicles.Add(newVehicle);
        newVehicle.transform.parent = lane.transform;

        position += newVehicle.length + Random.Range(minGap, maxGap);
        newVehicle.position = position;
        newVehicle.UpdateTransform();
        newVehicle.targetGap = Random.Range(minGap, maxGap);
        newVehicle.gapTime = Random.Range(minGapTime, maxGapTime);
        return position;
    }
}

[System.Serializable]
public class Lane
{
    public List<Vehicle> Vehicles = new List<Vehicle>();
    public Transform transform;
}