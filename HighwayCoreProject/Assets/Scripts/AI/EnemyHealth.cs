using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : EnemyBehaviour
{
    public float MaxHealth;
    public float Health;//tadinya gk public

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

    public override void Die()
    {
        if(enemy.stunned)
        {
            //drop items
        }
    }
}
