using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Enemy : MonoBehaviour
{
    public Transform Head;
    public Vector3 transformOffset;
    public float Cost, StunTime, StunResistance;
    public EnemyAttack Attack;
    public EnemyPathfinding Pathfinding;
    public EnemyHealth Health;
    public EnemyWeapon Weapon;
    public EnemyAnimation Animation;

    [HideInInspector] public Player targetPlayer;
    [HideInInspector] public EnemyManager manager;
    [HideInInspector] public bool stunned, aggro;

    float stunTime;

    public void Activate()
    {
        Attack.enemy = this;
        Pathfinding.enemy = this;
        Health.enemy = this;
        Weapon.enemy = this;
        Animation.enemy = this;
        stunned = false;
        Attack.Activate();
        Pathfinding.Activate();
        Health.Activate();
        Weapon.Activate();
        Animation.Activate();
    }

    public void Die()
    {
        Attack.Die();
        Pathfinding.Die();
        Health.Die();
        Weapon.Die();
        Animation.Die();

        print("ded");
        manager.RequestDie(this);
        gameObject.SetActive(false);
    }

    public void SetPlatform(PlatformAddress plat, TransformPoint point)
    {
        Pathfinding.currentPlatform = plat;
        Pathfinding.transformPosition = point;
    }

    public void TakeDamage(float amount)
    {
        Health.TakeDamage(amount);
        Attack.TakeDamage(amount);
    }

    public void Stun(Vector3 knockback)
    {
        knockback *= 1f - StunResistance;

        Attack.Stun(knockback);
        Pathfinding.Stun(knockback);
        Health.Stun(knockback);
        Weapon.Stun(knockback);

        stunTime = StunTime;
        stunned = true;
    }

    public bool SetAggro(bool yes, bool prioritize = true)
    {
        if(yes == aggro)
            return false;
        
        if(!manager.UpdateAggro(this, prioritize))
            return false;
        
        aggro = yes;
        
        Attack.SetAggro(yes);
        return true;
    }

    public bool TrySetAggro()
    {
        return Attack.TrySetAggro();
    }

    void Update()
    {
        if(stunned)
        {
            stunTime -= Time.deltaTime;
            if(stunTime <= 0)
            {
                stunned = false;
            }
        }
    }
}

public abstract class EnemyBehaviour : MonoBehaviour
{
    [HideInInspector] public Enemy enemy;

    public virtual void Activate(){}
    public virtual void Die(){}
    public virtual void TakeDamage(float amount){}
    public virtual void Stun(Vector3 knockback){}
}