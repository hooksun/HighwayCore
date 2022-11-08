using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : EnemyBehaviour
{
    public float MaxHealth;
    float Health;

    public override void Activate()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if(Health <= 0f)
        {
            enemy.Die();
        }
    }
}
