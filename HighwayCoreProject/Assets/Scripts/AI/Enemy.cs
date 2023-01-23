using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Enemy : MonoBehaviour, IHurtBox
{
    public Transform Head;
    public Vector3 transformOffset;
    public float Cost, StunTime, StunResistance, iFrames;
    public int RagdollIndex;
    public EnemyAttack Attack;
    public EnemyPathfinding Pathfinding;
    public EnemyHealth Health;
    public EnemyWeapon Weapon;
    public EnemyAnimation Animation;
    public Collider Hitbox;

    [HideInInspector] public Player targetPlayer;
    [HideInInspector] public EnemyManager manager;
    [HideInInspector] public bool stunned, aggro, spawned;

    public bool crit{get => false;}

    float stunTime, iframes;

    public void Activate()
    {
        gameObject.SetActive(true);
        Attack.enemy = this;
        Pathfinding.enemy = this;
        Health.enemy = this;
        Weapon.enemy = this;
        Animation.enemy = this;
        stunned = false;
        aggro = false;
        Head.localRotation = Quaternion.identity;
        Animation.Activate();
        Attack.Activate();
        Pathfinding.Activate();
        Health.Activate();
        Weapon.Activate();
        iframes = iFrames;
        Hitbox.enabled = false;
    }

    public void Die()
    {
        Attack.Die();
        Pathfinding.Die();
        Health.Die();
        Weapon.Die();
        Animation.Die();

        spawned = false;

        manager.RequestDie(this);
        gameObject.SetActive(false);
    }

    public void SetPlatform(PlatformAddress plat, TransformPoint point, bool grounded)
    {
        Pathfinding.currentPlatform = plat;
        Pathfinding.transformPosition = point;
        Pathfinding.isGrounded = grounded;
    }

    public void TakeDamage(float amount)
    {
        UIManager.SetHitMarker();
        Attack.TakeDamage(amount);
        Health.TakeDamage(amount);
    }

    public void Stun(Vector3 knockback)
    {
        knockback *= 1f - StunResistance;

        Attack.Stun(knockback);
        Pathfinding.Stun(knockback);
        Health.Stun(knockback);
        Weapon.Stun(knockback);
        Animation.Stun(knockback);

        stunTime = StunTime;
        stunned = true;
    }

    public bool SetAggro(bool yes, bool prioritize = true)
    {
        if(!manager.UpdateAggro(this, yes, prioritize))
            return false;
        
        Attack.SetAggro(yes);
        return true;
    }

    public bool TrySetAggro()
    {
        return Attack.TrySetAggro();
    }

    void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        if(iframes > 0f)
        {
            iframes -= Time.deltaTime;
            if(iframes <= 0f)
                Hitbox.enabled = true;
        }

        if(stunned)
        {
            stunTime -= Time.deltaTime;
            if(stunTime <= 0)
            {
                stunned = false;
                Attack.StopStun();
                Pathfinding.StopStun();
                Health.StopStun();
                Weapon.StopStun();
                Animation.StopStun();
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
    public virtual void StopStun(){}
}