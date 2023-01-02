using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyBehaviour
{
    public float Lead, ForceLookSpeed, MaxAttackDistance, MaxAttackAngle, AggroDistance, LoseAggroDistance, MinVerticalAngle, MaxVerticalAngle;
    public EnemyState Aggro, Passive;
    public LayerMask ObstacleMask;

    EnemyState currentState;
    Vector3 currentDirection, targetDirection;
    float lookAtSpeed;
    bool overrideLook;

    bool hasLOS;

    public override void Activate()
    {
        currentDirection = Vector3.forward;
        targetDirection = Vector3.forward;
        overrideLook = false;
        SetAggro(enemy.aggro);
    }

    public override void TakeDamage(float amount)
    {
        enemy.SetAggro(true);
    }
    public override void Stun(Vector3 knockback)
    {
        ForceLookAtPoint(Player.ActivePlayer.position);
    }
    public override void StopStun() => overrideLook = false;

    public virtual void SetAggro(bool yes)
    {
        currentState = (yes?Aggro:Passive);
    }

    public virtual bool TrySetAggro()
    {
        if(enemy.aggro || (enemy.targetPlayer.position - transform.position).sqrMagnitude > AggroDistance * AggroDistance || !hasLOS || enemy.Weapon.cantShoot)
            return false;
        if(!enemy.targetPlayer.LookingAt(transform.position))
            return false;
        
        return enemy.SetAggro(true, false);
    }

    void Update()
    {
        hasLOS = !Physics.Linecast(enemy.Head.position, enemy.targetPlayer.position, ObstacleMask);

        DeAggro();
        FindLook();
        ShootWeapon();
        LookAtTarget();
    }

    protected virtual void DeAggro()
    {
        if(!enemy.aggro)
            return;
        if((GetPlayerPosition() - enemy.Head.position).sqrMagnitude > LoseAggroDistance * LoseAggroDistance)
        {
            enemy.SetAggro(false);
            enemy.Weapon.InitReload();
        }
    }

    protected virtual void FindLook()
    {
        if(overrideLook || enemy.stunned)
            return;

        lookAtSpeed = currentState.rotateSpeed;
        targetDirection = (enemy.aggro && hasLOS?(GetPlayerPosition() - enemy.Head.position).normalized:enemy.Pathfinding.GetMoveDirection());
        
        float sin = Mathf.Sin(Mathf.Deg2Rad*MinVerticalAngle);
        if(sin > targetDirection.y)
        {
            targetDirection.y = 0f;
            targetDirection = targetDirection.normalized * Mathf.Cos(Mathf.Deg2Rad*MinVerticalAngle);
            targetDirection.y = sin;
            return;
        }
        sin = Mathf.Sin(Mathf.Deg2Rad*MaxVerticalAngle);
        if(sin < targetDirection.y)
        {
            targetDirection.y = 0f;
            targetDirection = targetDirection.normalized * Mathf.Cos(Mathf.Deg2Rad*MaxVerticalAngle);
            targetDirection.y = sin;
        }
    }
    protected virtual void ShootWeapon()
    {
        if(enemy.Weapon.cantShoot)
        {
            enemy.SetAggro(false);
            return;
        }

        Vector3 playerDist = enemy.targetPlayer.position - transform.position;
        bool inRange = playerDist.sqrMagnitude <= MaxAttackDistance * MaxAttackDistance;
        bool inAngle = Vector3.Dot(playerDist.normalized, enemy.Head.forward) >= Mathf.Cos(Mathf.Deg2Rad * MaxAttackAngle);
        enemy.Weapon.shoot = (enemy.aggro && !enemy.stunned && hasLOS && inRange && inAngle);
    }
    protected virtual void LookAtTarget()
    {
        currentDirection = Vector3.RotateTowards(currentDirection, targetDirection, lookAtSpeed * Time.deltaTime, 0f);
        enemy.Head.rotation = Quaternion.LookRotation(currentDirection, transform.up);
        enemy.Animation.SetLook(currentDirection);
    }

    protected virtual Vector3 GetPlayerPosition() => Vector3.LerpUnclamped(enemy.targetPlayer.trailPosition, enemy.targetPlayer.position, Lead);

    public virtual PlatformAddress PickPlatform(List<PlatformAddress> neighbours, PlatformAddress current)
    {
        if(neighbours == null || neighbours.Count == 0)
            return current;

        List<PlatformAddress> closer = new List<PlatformAddress>();
        float currentDist = Mathf.Abs(current.DistanceFrom(enemy.targetPlayer.position) - currentState.desiredDistance);
        for(int i = 0; i < neighbours.Count; i++)
        {
            float newDist = Mathf.Abs(neighbours[i].DistanceFrom(enemy.targetPlayer.position) - currentState.desiredDistance);
            if(newDist < currentDist)
            {
                closer.Add(neighbours[i]);
            }
        }
        if(closer.Count == 0)
            return neighbours[Random.Range(0, neighbours.Count)];
        return closer[Random.Range(0, closer.Count)];
    }

    public void ForceLook(Vector3 direction, float speed = 0f)
    {
        overrideLook = true;
        targetDirection = direction;
        if(speed <= 0)
            speed = ForceLookSpeed;
        lookAtSpeed = speed;
    }

    public void ForceLookAtPoint(Vector3 point, float speed = 0f) => ForceLook(point - enemy.Head.position, speed);

    public void StopForceLook() => overrideLook = false;
}

[System.Serializable]
public struct EnemyState
{
    public float desiredDistance, rotateSpeed;
}