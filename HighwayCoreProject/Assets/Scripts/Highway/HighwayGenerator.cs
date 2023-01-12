using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayGenerator : MonoBehaviour
{
    public EnemyManager enemyManager;
    public HighwaySection[] Sections;
    public HighwaySection[] SectionsLoop;
    public VariablePool<int> Vehicles;
    public Lane[] Lanes;
    public float StartPosition, minGap, maxGap, accelRange, minGapTime, maxGapTime, gapDistance;

    public Transform player; // temp
    public float PlayerGenerateDist;

    HighwaySection currentSection{get => (hasLooped?SectionsLoop[sectionIndex]:Sections[sectionIndex]);}
    int sectionIndex;
    float sectionPosition, nextSectionPosition;

    void Start()
    {
        GenerateInitialHighway();
        GenerateForPlayer();
    }

    void GenerateInitialHighway()
    {
        sectionIndex = 0;
        sectionPosition = StartPosition;
        nextSectionPosition = StartPosition + currentSection.distance;
        currentSection.HighwayTable.Reset();
        foreach(Lane lane in Lanes)
        {
            AddVehicle(lane, StartPosition);
        }
    }

    void GenerateForPlayer()
    {
        int i;
        for(i = 0; i < Lanes.Length; i++)
        {
            float pos = Lanes[i].Vehicles[Lanes[i].Vehicles.Count - 1].position + Lanes[i].Vehicles[Lanes[i].Vehicles.Count - 1].length;
            while(pos < player.position.z)
            {
                pos = AddVehicle(Lanes[i], pos);
            }
        }
        i = Lanes.Length/2;
        AddNewVehicle(Lanes[i]);
        Vehicle playerVehicle = Lanes[i].Vehicles[Lanes[i].Vehicles.Count - 1];
        player.position = playerVehicle.ClosestPlatform(player.position).CenterPoint().worldPoint + Vector3.up;
    }

    void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        SimulateHighway();
    }

    void GenerateHighway(Lane lane)
    {
        Vehicle lastVehicle = lane.Vehicles[lane.Vehicles.Count - 1];
        if(lastVehicle.position - player.position.z < PlayerGenerateDist)
        {
            AddVehicle(lane, lastVehicle.position + lastVehicle.length);
        }
        if(player.position.z - lane.Vehicles[0].position > PlayerGenerateDist)
        {
            RemoveVehicle(lane, 0);
        }
    }

    void SimulateHighway()
    {
        foreach(Lane lane in Lanes)
        {
            GenerateHighway(lane);
            
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

    float AddNewVehicle(Lane lane)
    {
        float pos = lane.Vehicles[lane.Vehicles.Count - 1].position + lane.Vehicles[lane.Vehicles.Count - 1].length;
        return AddVehicle(lane, pos);
    }

    bool hasLooped;
    float AddVehicle(Lane lane, float position)
    {
        if(position >= nextSectionPosition)
        {
            sectionIndex++;
            if(hasLooped)
                sectionIndex %= SectionsLoop.Length;
            else if(sectionIndex >= Sections.Length)
            {
                hasLooped = true;
                sectionIndex = 0;
            }
            
            currentSection.HighwayTable.Reset();
            sectionPosition = nextSectionPosition;
            nextSectionPosition += currentSection.distance;
            if(currentSection.battle)
                enemyManager.NewBattle(sectionPosition + currentSection.battleCenter);
        }
        
        Vehicle newVehicle = null;
        if(currentSection.HighwayTable.SpawnVehicles != null)
        {
            foreach(SpawnVehicle veh in currentSection.HighwayTable.SpawnVehicles)
            {
                if(veh.spawned || position < sectionPosition + veh.distance || lane.transform.GetSiblingIndex() != veh.lane)
                    continue;
                newVehicle = VehiclePool.GetObject(veh.vehicle);
                veh.spawned = true;
            }
        }
        if(newVehicle == null)
            newVehicle = VehiclePool.GetObject(currentSection.HighwayTable.GetRandomVehicle());
        lane.Vehicles.Add(newVehicle);
        newVehicle.transform.parent = lane.transform;

        if(newVehicle.isSpawner)
        {
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
        VehiclePool.Return(vehicle);
        lane.Vehicles.RemoveAt(index);
        if(index == lane.Vehicles.Count)
            lane.Vehicles[lane.Vehicles.Count - 1].speed = 0f;

        if(vehicle.isSpawner)
        {
            enemyManager.Spawners.Remove((IEnemySpawner)vehicle);
        }
    }
}

[System.Serializable]
public struct HighwaySection
{
    public float distance;
    public bool battle;
    public float battleCenter;
    public HighwaySpawnTable HighwayTable;

}

[System.Serializable]
public class SpawnVehicle
{
    public int lane, vehicle;
    public float distance;
    [HideInInspector] public bool spawned;
}