using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public VariablePool<int> Items;
    public Vector3 MinVelocity, MaxVelocity;

    public void SpawnItems(int amount, Vector3 position)
    {
        for(int i = 0; i < amount; i++)
        {
            Item item = ItemPool.GetObject(Items.GetRandomVar());
            Vector3 velocity = Vector3.zero;
            velocity.x = Random.Range(MinVelocity.x, MaxVelocity.x);
            velocity.y = Random.Range(MinVelocity.y, MaxVelocity.y);
            velocity.z = Random.Range(MinVelocity.z, MaxVelocity.z);
            item.Spawn(position, velocity);
        }
    }
}
