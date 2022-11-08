using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform Head;
    public Vector3 transformOffset;
    public float StunTime;
    public EnemyAttack Attack;
    public EnemyPathfinding Pathfinding;
    public EnemyHealth Health;

    [HideInInspector] public Player targetPlayer;
    [HideInInspector] public EnemyManager manager;

    void OnEnable()
    {
        Attack.enemy = this;
        Pathfinding.enemy = this;
        Health.enemy = this;
    }

    public void Activate()
    {
        Attack.Activate();
        Pathfinding.Activate();
        Health.Activate();
    }

    public void Die()
    {
        Attack.Die();
        Pathfinding.Die();
        Health.Die();

        print("ded");
        manager.ActiveEnemies.Remove(this);
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
    }

    public void Stun(Vector3 knockback)
    {
        Attack.Stun(knockback);
        Pathfinding.Stun(knockback);
        Health.Stun(knockback);
    }
}

public enum EnemyState{idle, pathfinding, attack, stunned}

public abstract class EnemyBehaviour : MonoBehaviour
{
    [HideInInspector] public Enemy enemy;

    public virtual void Activate(){}
    public virtual void Die(){}
    public virtual void Stun(Vector3 knockback){}
}