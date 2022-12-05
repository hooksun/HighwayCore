using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Player player; // temp
    public HighwayGenerator Highway;
    public EnemyPool enemyPool;
    public ItemSpawner itemSpawner;
    public int maxItems, minItems;

    public VariablePool<EnemyType> Enemies;
    public List<IEnemySpawner> Spawners = new List<IEnemySpawner>();
    public List<Enemy> ActiveEnemies = new List<Enemy>();

    IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        SpawnEnemies(10);
    }
    
    void EnemyUpdate()
    {

    }

    void SpawnEnemies(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            EnemyType enemy = Enemies.GetRandomVar();
            int[] seq = Util.RandomSequence(Spawners.Count);
            for(int j = 0; j < seq.Length; j++)
            {
                IEnemySpawner spawner = Spawners[seq[j]];
                if(!spawner.canSpawn)
                    continue;
                float dist = spawner.DistanceFrom(player.position);
                if(dist < enemy.minDistance || dist > enemy.maxDistance)
                    continue;

                Enemy nme = enemyPool.GetObject(enemy.enemyIndex);
                nme.manager = this;
                nme.targetPlayer = player;

                spawner.SpawnEnemy(nme);
                ActiveEnemies.Add(nme);
                nme.Activate();
                break;
            }
        }
    }

    public void RequestDie(Enemy enemy)
    {
        if(enemy.stunned)
        {
            itemSpawner.SpawnItems(Random.Range(minItems, maxItems), enemy.transform.position);
        }
        
        ActiveEnemies.Remove(enemy);
        if(ActiveEnemies.Count == 0)
            SpawnEnemies(5);
    }

    public List<PlatformAddress> RequestPlatformNeighbours(PlatformAddress platform, float boundsOffset)
    {
        List<PlatformAddress> answer = new List<PlatformAddress>();

        if(platform.platformIndex > 0)
        {
            PlatformAddress newPlat = platform;
            newPlat.platformIndex--;
            answer.Add(newPlat);
        }
        if(platform.platformIndex < platform.vehicle.Platforms.Length - 1)
        {
            PlatformAddress newPlat = platform;
            newPlat.platformIndex++;
            answer.Add(newPlat);
        }
        if(platform.platformIndex == 0 && platform.vehicleIndex > 0)
        {
            Vehicle nextVehicle = platform.lane.Vehicles[platform.vehicleIndex-1];
            answer.Add(new PlatformAddress(platform.lane, nextVehicle, nextVehicle.Platforms.Length - 1));
        }
        if(platform.platformIndex == platform.vehicle.Platforms.Length - 1 && platform.vehicleIndex < platform.lane.Vehicles.Count - 1)
        {
            Vehicle nextVehicle = platform.lane.Vehicles[platform.vehicleIndex+1];
            answer.Add(new PlatformAddress(platform.lane, nextVehicle, 0));
        }
        float boundsStart = platform.vehicle.position + platform.platform.BoundsStart.y;
        float boundsEnd = platform.vehicle.position + platform.platform.BoundsEnd.y;
        int laneIndex = platform.laneIndex - 1;
        for(int h = 0; h < 2; h++)
        {
            if(laneIndex >= 0 && laneIndex < Highway.Lanes.Length)
            {
                Lane nextLane = Highway.Lanes[laneIndex];
                for(int i = 0; i < nextLane.Vehicles.Count; i++)
                {
                    Vehicle vehicle = nextLane.Vehicles[i];
                    if(vehicle.position > boundsEnd + boundsOffset)
                        break;
                    if(vehicle.position + vehicle.length < boundsStart - boundsOffset)
                        continue;
                    
                    for(int j = 0; j < vehicle.Platforms.Length; j++)
                    {
                        Platform plat = vehicle.Platforms[j];
                        if(vehicle.position + plat.BoundsStart.y > boundsEnd + boundsOffset || vehicle.position + plat.BoundsEnd.y < boundsStart - boundsOffset)
                            continue;
                        
                        answer.Add(new PlatformAddress(nextLane, vehicle, j));
                    }
                }
            }
            laneIndex = platform.laneIndex + 1;
        }

        return answer;
    }
}

[System.Serializable]
public struct EnemyType
{
    public int enemyIndex;
    public float minDistance, maxDistance;
}

[System.Serializable]
public struct EnemyWave
{
    public float TotalCost, ActiveCost, AggroCost, StartCost;
}