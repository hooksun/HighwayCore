using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Player player; // temp
    public HighwayGenerator Highway;
    public EnemySpawnTable[] EnemyTables;
    public EnemyBattle[] Battles;
    public EnemyBattle BattleIncrement;
    public float barrierDistance, battleClearDist = 70f;
    public Barrier forwardBarrier;
    public HighwayBarrier backBarrier;

    public VariablePool<EnemyType> Enemies;
    public List<IEnemySpawner> Spawners = new List<IEnemySpawner>();
    [HideInInspector] public List<Enemy> ActiveEnemies = new List<Enemy>();

    EnemySpawnTable currentTable;
    EnemyBattle currentBattle;
    EnemyWave currentWave;
    EnemyWave[] waves;
    EnemyCost costs, costBuffer;
    float TotalCost, ActiveCost, AggroCost, battlePos;
    int iTable, iBattle, iWave;
    bool battling, battleReady;

    void Start()
    {
        currentTable = EnemyTables[0];
        costs = currentTable.Cost;
        spawnTime = currentTable.StartInterval;
        aggroTime = currentTable.AggroInterval;
    }

    void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        SpawnUpdate();
        AggroUpdate();

        if(battleReady && player.position.z > battlePos)
        {
            battleReady = false;
            StartBattle();
        }
    }
    
    float spawnTime;
    void SpawnUpdate()
    {
        if(spawnTime > 0)
        {
            spawnTime -= Time.deltaTime;
            return;
        }

        if(battling && TotalCost >= currentWave.TotalCost)
            return;
        
        spawnTime = Random.Range(currentTable.SpawnIntervalMin, currentTable.SpawnIntervalMax);
        SpawnEnemies(costs.Spawn);
    }

    void SpawnEnemies(float cost)
    {
        float spawned = 0f;
        while(spawned < cost)
        {
            if(ActiveCost >= costs.Active || Spawners.Count == 0)
                return;

            EnemyType enemy = currentTable.GetRandomEnemy();
            if(ActiveCost + enemy.enemyCost > costs.Active)
            {
                spawned += costs.Fail;
                continue;
            }
            int[] seq = Util.RandomSequence(Spawners.Count);
            bool success = false;
            for(int j = 0; j < seq.Length; j++)
            {
                SpawnerVehicle spawner = (SpawnerVehicle)Spawners[seq[j]];
                if(!spawner.canSpawn || spawner.position < backBarrier.transform.position.z)
                    continue;
                if(forwardBarrier.gameObject.activeInHierarchy && spawner.position + spawner.length > forwardBarrier.transform.position.z)
                    continue;
                float dist = spawner.DistanceFrom(player.position);
                if(dist < enemy.minDistance || dist > enemy.maxDistance)
                    continue;

                Enemy nme = EnemyPool.GetObject(enemy.enemyIndex, false);
                nme.manager = this;
                nme.targetPlayer = player;
                nme.Cost = enemy.enemyCost;
                ActiveEnemies.Add(nme);
                ActiveCost += nme.Cost;
                TotalCost += nme.Cost;
                spawned += nme.Cost;
                spawner.SpawnEnemy(nme);
                nme.spawned = true;
                success = true;
                break;
            }
            if(!success)
                spawned += costs.Fail;
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
        if(AggroCost >= costs.Aggro || ActiveEnemies.Count == 0)
            return;
        int[] seq = Util.RandomSequence(ActiveEnemies.Count);
        for(int i = 0; i < ActiveEnemies.Count; i++)
        {
            Enemy current = ActiveEnemies[seq[i]];
            if(current.gameObject.activeInHierarchy && AggroCost + current.Cost <= costs.Aggro && current.TrySetAggro())
                return;
        }
    }

    public void NewBattle(float position)
    {
        battleReady = true;
        battlePos = position;
    }

    public void StartBattle()
    {
        player.freezeScore = true;
        player.score = battlePos;
        forwardBarrier.transform.position = Vector3.forward * (battlePos + barrierDistance);
        forwardBarrier.Fade(true);
        backBarrier.visible = true;

        for(int i = ActiveEnemies.Count - 1; i >= 0; i--)
        {
            if(ActiveEnemies[i].transform.position.z < battlePos - battleClearDist)
                ActiveEnemies[i].Die();
        }

        battling = true;
        if(iBattle < Battles.Length)
        {
            currentBattle = Battles[iBattle];
            currentTable = currentBattle.SpawnTable;
            costs = currentTable.Cost;
            waves = currentBattle.Waves.Clone() as EnemyWave[];
            costBuffer = costs;
            iBattle++;
        }
        else
        {
            currentTable = currentBattle.SpawnTable;
            for(int i = Mathf.Min(waves.Length, BattleIncrement.Waves.Length)-1; i >= 0; i--)
            {
                waves[i] += BattleIncrement.Waves[i];
            }
            costBuffer += BattleIncrement.SpawnTable.Cost;
            costs = costBuffer;
        }
        iWave = 0;
        StartWave();
    }

    void StartWave()
    {
        if(iWave >= waves.Length)
        {
            EndBattle();
            return;
        }

        currentWave = waves[iWave];
        spawnTime = currentTable.StartInterval;
        TotalCost = 0f;
        iWave++;
    }

    void EndBattle()
    {
        if(iTable+1 < EnemyTables.Length)
            iTable++;
        currentTable = EnemyTables[iTable];
        costs = currentTable.Cost;
        spawnTime = currentTable.StartInterval;
        battling = false;
        forwardBarrier.Fade(false);
        backBarrier.visible = false;
        player.freezeScore = false;
    }

    public void RequestDie(Enemy enemy)
    {
        UpdateAggro(enemy, false);
        ActiveEnemies.Remove(enemy);
        ActiveCost -= enemy.Cost;
        if(battling && TotalCost >= currentWave.TotalCost && ActiveCost <= currentWave.EndCost)
            StartWave();
    }

    public bool UpdateAggro(Enemy enemy, bool newAggro, bool prioritize = false)
    {
        if(enemy.aggro == newAggro)
            return false;

        AggroCost += enemy.Cost * (newAggro?1f:-1f);

        enemy.aggro = newAggro;
        if(!newAggro)
            return true;
        
        if(AggroCost > costs.Aggro)
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
        bool inBarrier = WithinBarrier(platform);

        if(platform.platformIndex > 0)
        {
            PlatformAddress newPlat = platform;
            newPlat.platformIndex--;
            if(!inBarrier || WithinBarrier(newPlat))
                answer.Add(newPlat);
        }
        if(platform.platformIndex < platform.vehicle.Platforms.Length - 1)
        {
            PlatformAddress newPlat = platform;
            newPlat.platformIndex++;
            if(!inBarrier || WithinBarrier(newPlat))
                answer.Add(newPlat);
        }
        if(platform.platformIndex == 0 && platform.vehicleIndex > 0)
        {
            Vehicle nextVehicle = platform.lane.Vehicles[platform.vehicleIndex-1];
            PlatformAddress newPlat = new PlatformAddress(platform.lane, nextVehicle, nextVehicle.Platforms.Length - 1);
            if(!inBarrier || WithinBarrier(newPlat))
                answer.Add(newPlat);
        }
        if(platform.platformIndex == platform.vehicle.Platforms.Length - 1 && platform.vehicleIndex < platform.lane.Vehicles.Count - 1)
        {
            Vehicle nextVehicle = platform.lane.Vehicles[platform.vehicleIndex+1];
            PlatformAddress newPlat = new PlatformAddress(platform.lane, nextVehicle, 0);
            if(!inBarrier || WithinBarrier(newPlat))
                answer.Add(newPlat);
        }
        float boundsStart = platform.vehicle.position + platform.platform.BoundsStart.y - boundsOffset;
        float boundsEnd = platform.vehicle.position + platform.platform.BoundsEnd.y + boundsOffset;
        int laneIndex = platform.laneIndex - 1;
        for(int h = 0; h < 2; h++)
        {
            if(laneIndex >= 0 && laneIndex < Highway.Lanes.Length)
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
                        
                        PlatformAddress newPlat = new PlatformAddress(nextLane, vehicle, j);
                        if(!inBarrier || WithinBarrier(newPlat))
                            answer.Add(newPlat);
                    }
                }
            }
            laneIndex = platform.laneIndex + 1;
        }

        return answer;
    }

    public bool WithinBarrier(PlatformAddress platform)
    {
        if(platform.vehicle.position + platform.platform.BoundsEnd.y < backBarrier.transform.position.z)
            return false;
        if(battling && platform.vehicle.position + platform.platform.BoundsStart.y > forwardBarrier.transform.position.z)
            return false;
        return true;
    }
}

[System.Serializable]
public struct EnemyType
{
    public int enemyIndex;
    public float enemyCost, minDistance, maxDistance;
}