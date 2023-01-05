using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : EnemyBehaviour
{
    public float MaxHealth;
    public ItemDrop normalDrops, stunnedDrops;
    public Vector3 MinVelocity, MaxVelocity;
    float Health;

    [HideInInspector] public ItemDrop currentDrop;

    public override void Activate()
    {
        Health = MaxHealth;
        currentDrop = normalDrops;
    }

    public override void Stun(Vector3 knockback)
    {
        currentDrop = stunnedDrops;
    }
    public override void StopStun()
    {
        currentDrop = normalDrops;
    }

    public override void TakeDamage(float amount)
    {
        Health -= amount;
        if(Health <= 0f)
        {
            enemy.Die();
        }
    }

    public override void Die()
    {
        SpawnItems(currentDrop);
    }

    public void SetStunItems(bool stun)
    {
        currentDrop = (stun?stunnedDrops:normalDrops);
    }

    void SpawnItems(ItemDrop drop)
    {
        int random = Random.Range(drop.minItems, drop.maxItems);
        for(int i = 0; i < random; i++)
        {
            Item item = ItemPool.GetObject(drop.items.GetRandomVar());
            Vector3 velocity = Vector3.zero;
            velocity.x = Random.Range(MinVelocity.x, MaxVelocity.x);
            velocity.y = Random.Range(MinVelocity.y, MaxVelocity.y);
            velocity.z = Random.Range(MinVelocity.z, MaxVelocity.z);
            item.Spawn(transform.position, velocity);
        }
    }
}

[System.Serializable]
public struct ItemDrop
{
    public int minItems, maxItems;
    public VariablePool<int> items;
}