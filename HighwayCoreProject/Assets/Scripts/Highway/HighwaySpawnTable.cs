using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="HighwayTable", menuName ="Highway/SpawnTable")]
public class HighwaySpawnTable : LootTable
{
    public SpawnVehicle[] SpawnVehicles;
    List<int> VehicleQueue;

    public int GetRandomVehicle()
    {
        if(VehicleQueue.Count == 0)
        {
            ResetQueue();
        }
        int random = Random.Range(0, VehicleQueue.Count);
        int vehicle = VehicleQueue[random];
        VehicleQueue.RemoveAt(random);
        return vehicle;
    }

    void ResetQueue()
    {
        VehicleQueue = GetLoot();
    }
    
    public void Reset()
    {
        ResetQueue();
        if(SpawnVehicles != null)
        {
            foreach(SpawnVehicle spawn in SpawnVehicles)
            {
                spawn.spawned = false;
            }
        }
    }
}