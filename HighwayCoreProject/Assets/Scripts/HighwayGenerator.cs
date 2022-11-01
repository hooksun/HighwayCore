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
                Vehicle newVehicle = Pool.GetVehicle(Vehicles.GetRandomVar());
                lane.Vehicles.Add(newVehicle);
                newVehicle.transform.parent = lane.transform;

                position += newVehicle.length + Random.Range(minGap, maxGap);
                newVehicle.position = position;
                newVehicle.targetGap = Random.Range(minGap, maxGap);
                newVehicle.gapTime = Random.Range(minGapTime, maxGapTime);
            }
        }
    }

    void SimulateHighway()
    {
        foreach(Lane lane in Lanes)
        {
            for(int i = 1; i < lane.Vehicles.Count; i++)
            {
                Vehicle vehicle = lane.Vehicles[i];
                Vehicle lastVehicle = lane.Vehicles[i-1];
                float gap = vehicle.position - vehicle.length - lastVehicle.position;
                if(Mathf.Abs(vehicle.targetGap - gap) < .2f)
                {
                    vehicle.targetGap = Random.Range(minGap, maxGap);
                    vehicle.gapTime = Random.Range(minGapTime, maxGapTime);
                    continue;
                }
                vehicle.position = Mathf.SmoothDamp(gap, vehicle.targetGap, ref vehicle.speed, vehicle.gapTime) + vehicle.length + lastVehicle.position;
            }
        }
    }
}

[System.Serializable]
public class Lane
{
    public List<Vehicle> Vehicles = new List<Vehicle>();
    public Transform transform;
}