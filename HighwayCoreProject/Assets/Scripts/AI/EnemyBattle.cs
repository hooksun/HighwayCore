using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="EnemyBattle", menuName ="Enemy/Battle")]
public class EnemyBattle : ScriptableObject
{
    public EnemySpawnTable SpawnTable;
    public EnemyWave[] Waves;
}

[System.Serializable]
public struct EnemyWave
{
    public float TotalCost, EndCost;

    public EnemyWave(float total, float end)
    {
        TotalCost = total;
        EndCost = end;
    }

    public static EnemyWave operator +(EnemyWave a, EnemyWave b) => new EnemyWave(a.TotalCost + b.TotalCost, a.EndCost + b.EndCost);
}