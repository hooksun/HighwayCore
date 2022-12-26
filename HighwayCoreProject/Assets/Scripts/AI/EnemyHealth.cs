using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : EnemyBehaviour
{
    public float MaxHealth;
    public int minItems, maxItems;
    float Health;

    public override void Activate()
    {
        Health = MaxHealth;
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
        if(enemy.stunned)
        {
            DropItems();
        }
    }

    public void DropItems()
    {
        enemy.manager.itemSpawner.SpawnItems(Random.Range(minItems, maxItems), transform.position);
    }
}
