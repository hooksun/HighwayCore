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
    public float TotalCost, StartCost;
}