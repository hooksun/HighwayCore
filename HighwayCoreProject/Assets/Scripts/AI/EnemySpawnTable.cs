using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="EnemyTable", menuName ="Enemy/SpawnTable")]
public class EnemySpawnTable : ScriptableObject
{
    public VariablePool<EnemyType> EnemyPool;
    public float ActiveCost, AggroCost, SpawnCost, FailCost, StartInterval, SpawnIntervalMin, SpawnIntervalMax;

    public EnemyType GetRandomEnemy()
    {
        return EnemyPool.GetRandomVar();
    }
}
