using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "LootTable")]
public class LootTable : ScriptableObject
{
    public VariablePool<Loot> LootPool;
    public int minItems, maxItems;

    public List<int> GetLoot()
    {
        List<int> loot = new List<int>();
        for(int i = 0; i < LootPool.Pool.Length; i++)
        {
            for(int j = 0; j < LootPool.Pool[i].variable.min; j++)
            {
                loot.Add(LootPool.Pool[i].variable.index);
            }
        }
        int random = Random.Range(minItems, maxItems+1);
        while(loot.Count < random)
        {
            loot.Add(LootPool.GetRandomVar().variable.index);
        }
        return loot;
    }
}

[System.Serializable]
public struct Loot
{
    public int index, min;
}