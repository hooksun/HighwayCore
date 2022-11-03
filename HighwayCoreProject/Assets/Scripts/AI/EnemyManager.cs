using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform player; // temp
    public HighwayGenerator Highway;
    public EnemyPool enemyPool;

    public VariablePool<EnemyType> Enemies;
    public List<IEnemySpawner> Spawners;

    List<Enemy> ActiveEnemies = new List<Enemy>();

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
            }
        }
    }

    public List<PlatformAddress> RequestPlatformNeighbours(PlatformAddress platform)
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
        float boundsStart = platform.vehicle.position + platform.platform.BoundsStart.y;
        float boundsEnd = platform.vehicle.position + platform.platform.BoundsEnd.y;
        int laneIndex = platform.laneIndex - 1;
        for(int h = 0; h < 2; h++)
        {
            if(laneIndex > 0 && laneIndex < Highway.Lanes.Length - 1)
            {
                Lane nextLane = Highway.Lanes[laneIndex];
                for(int i = 0; i < nextLane.Vehicles.Count; i++)
                {
                    Vehicle vehicle = nextLane.Vehicles[i];
                    if(vehicle.position > boundsEnd)
                        break;
                    if(vehicle.position + vehicle.length < boundsStart)
                        continue;
                    
                    for(int j = 0; j < vehicle.Platforms.Length; j++)
                    {
                        Platform plat = vehicle.Platforms[j];
                        if(vehicle.position + plat.BoundsStart.y > boundsEnd || vehicle.position + plat.BoundsEnd.y < boundsStart)
                            continue;
                        
                        answer.Add(new PlatformAddress(nextLane, laneIndex, i, j));
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