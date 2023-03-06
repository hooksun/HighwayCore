using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="EnemyTable", menuName ="Enemy/SpawnTable")]
public class EnemySpawnTable : ScriptableObject
{
    public VariablePool<EnemyType> EnemyPool;
    public float StartInterval, SpawnIntervalMin, SpawnIntervalMax, AggroInterval;
    public EnemyCost Cost;

    public EnemyType GetRandomEnemy()
    {
        return EnemyPool.GetRandomVar();
    }
}

[System.Serializable]
public struct EnemyCost
{
    public float Active, Aggro, Spawn, Fail;

    public EnemyCost(float active, float aggro, float spawn, float fail)
    {
        Active = active;
        Aggro = aggro;
        Spawn = spawn;
        Fail = fail;
    }

    public static EnemyCost operator +(EnemyCost a, EnemyCost b) => new EnemyCost(a.Active+b.Active,a.Aggro+b.Aggro,a.Spawn+b.Spawn,a.Fail+b.Fail);
}