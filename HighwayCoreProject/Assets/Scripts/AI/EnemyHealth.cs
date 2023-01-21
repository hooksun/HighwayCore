using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : EnemyBehaviour
{
    public float MaxHealth;
    public LootTable normalDrops, stunnedDrops;

    float Health;

    [HideInInspector] public bool stunDrops;

    public override void Activate()
    {
        Health = MaxHealth;
        stunDrops = false;
    }

    public override void Stun(Vector3 knockback)
    {
        stunDrops = true;
    }
    public override void StopStun()
    {
        stunDrops = false;
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
        SpawnItems(stunDrops);
    }

    public void SetStunItems(bool stun)
    {
        stunDrops = stun;
    }

    void SpawnItems(bool drop)
    {
        List<int> drops = (drop?stunnedDrops:normalDrops).GetLoot();
        foreach(int i in drops)
        {
            Item item = ItemPool.GetObject(i);
            item.Spawn(transform.position, Vector3.up * Random.Range(0f, 360f));
        }
    }
}