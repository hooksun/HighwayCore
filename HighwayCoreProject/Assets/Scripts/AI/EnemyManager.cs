using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Player player; // temp
    public HighwayGenerator Highway;
    public EnemySpawnTable[] EnemyTables;
    public EnemyBattle[] Battles;

    public VariablePool<EnemyType> Enemies;
    public List<IEnemySpawner> Spawners = new List<IEnemySpawner>();
    [HideInInspector] public List<Enemy> ActiveEnemies = new List<Enemy>();

    EnemySpawnTable currentTable;
    EnemyBattle currentBattle;
    EnemyWave currentWave;
    float TotalCost, ActiveCost, AggroCost, StartCost;
    int iTable, iBattle, iWave;
    bool battling;

    void Start()
    {
        currentTable = EnemyTables[0];
        spawnTime = currentTable.StartInterval;
        aggroTime = currentTable.AggroInterval;
    }

    void Update()
    {
        SpawnUpdate();
        AggroUpdate();
    }
    
    float spawnTime;
    void SpawnUpdate()
    {
        if(spawnTime > 0)
        {
            spawnTime -= Time.deltaTime;
            return;
        }

        if(battling && StartCost >= 0f && ActiveCost > StartCost)
        {
            spawnTime = currentTable.StartInterval;
            return;
        }
        
        spawnTime = Random.Range(currentTable.SpawnIntervalMin, currentTable.SpawnIntervalMax);
        //StartCost = currentTable.MinCost;
        SpawnEnemies(currentTable.SpawnCost);
        if(!battling)
            return;

        if(StartCost >= 0f)
            aggroTime = 0f;
        
        StartCost = -1f;
        if(TotalCost >= currentWave.TotalCost)
            StartWave();
    }

    void SpawnEnemies(float cost)
    {
        float spawned = 0f;
        while(spawned < cost)
        {
            if(ActiveCost >= currentTable.ActiveCost || Spawners.Count == 0)
                return;

            EnemyType enemy = currentTable.GetRandomEnemy();
            if(ActiveCost + enemy.enemyCost > currentTable.ActiveCost)
            {
                spawned += currentTable.FailCost;
                continue;
            }
            int[] seq = Util.RandomSequence(Spawners.Count);
            bool success = false;
            for(int j = 0; j < seq.Length; j++)
            {
                IEnemySpawner spawner = Spawners[seq[j]];
                if(!spawner.canSpawn)
                    continue;
                float dist = spawner.DistanceFrom(player.position);
                if(dist < enemy.minDistance || dist > enemy.maxDistance)
                    continue;

                Enemy nme = EnemyPool.GetObject(enemy.enemyIndex);
                nme.manager = this;
                nme.targetPlayer = player;

                spawner.SpawnEnemy(nme);
                nme.Cost = enemy.enemyCost;
                ActiveEnemies.Add(nme);
                ActiveCost += nme.Cost;
                TotalCost += nme.Cost;
                spawned += nme.Cost;
                nme.Activate();
                success = true;
                break;
            }
            if(!success)
                spawned += currentTable.FailCost;
        }
    }

    float aggroTime;
    void AggroUpdate()
    {
        if(aggroTime > 0)
        {
            aggroTime -= Time.deltaTime;
            return;
        }

        aggroTime = currentTable.AggroInterval;
        AggroEnemies();
    }

    void AggroEnemies()
    {
        if(AggroCost >= currentTable.AggroCost || ActiveEnemies.Count == 0)
            return;
        int[] seq = Util.RandomSequence(ActiveEnemies.Count);
        for(int i = 0; i < ActiveEnemies.Count; i++)
        {
            if(AggroCost + ActiveEnemies[seq[i]].Cost <= currentTable.AggroCost && ActiveEnemies[seq[i]].TrySetAggro())
                return;
        }
    }

    public void StartBattle()
    {
        battling = true;
        currentBattle = Battles[iBattle];
        currentTable = currentBattle.SpawnTable;
        iWave = 0;
        StartWave();
    }

    void StartWave()
    {
        if(iWave >= currentBattle.Waves.Length)
            EndBattle();

        currentWave = currentBattle.Waves[iWave];
        StartCost = currentWave.StartCost;
        TotalCost = 0f;
        iWave++;
    }

    void EndBattle()
    {
        iBattle++;
        iTable++;
        currentTable = EnemyTables[iTable];
        battling = false;
    }

    public void RequestDie(Enemy enemy)
    {
        UpdateAggro(enemy, false);
        ActiveEnemies.Remove(enemy);
        ActiveCost -= enemy.Cost;
    }

    public bool UpdateAggro(Enemy enemy, bool newAggro, bool prioritize = false)
    {
        if(enemy.aggro == newAggro)
            return false;

        AggroCost += enemy.Cost * (newAggro?1f:-1f);

        enemy.aggro = newAggro;
        if(!newAggro)
            return true;
        
        if(AggroCost > currentTable.AggroCost)
        {
            if(!prioritize)
            {
                AggroCost -= enemy.Cost;
                enemy.aggro = !newAggro;
                return false;
            }

            int[] seq = Util.RandomSequence(ActiveEnemies.Count);
            for(int i = 0; i < ActiveEnemies.Count; i++)
            {
                if(ActiveEnemies[seq[i]].aggro && ActiveEnemies[seq[i]] != enemy)
                {
                    if(ActiveEnemies[seq[i]].SetAggro(false))
                        return true;
                }
            }
        }
        return true;
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
    public float enemyCost, minDistance, maxDistance;
}