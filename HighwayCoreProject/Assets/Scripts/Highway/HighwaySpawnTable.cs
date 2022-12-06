using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="HighwayTable", menuName ="Highway/SpawnTable")]
public class HighwaySpawnTable : ScriptableObject
{
    public VariablePool<int> VehiclePool;
    public SpawnVehicle[] SpawnVehicles;

    public int GetRandomVehicle()
    {
        return VehiclePool.GetRandomVar();
    }
    
    public void Reset()
    {
        if(SpawnVehicles != null)
        {
            foreach(SpawnVehicle spawn in SpawnVehicles)
            {
                spawn.spawned = false;
            }
        }
    }
}