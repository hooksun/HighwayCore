using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyBehaviour
{
    public float Lead, ForceLookSpeed, MaxAttackDistance;
    public EnemyState Aggro, Passive;

    EnemyState currentState;
    Vector3 currentDirection, targetDirection;
    float lookAtSpeed;
    bool overrideLook;

    public override void Activate()
    {
        currentDirection = Vector3.forward;
        targetDirection = Vector3.forward;
        overrideLook = false;
        SetAggro(enemy.aggro);
    }

    public virtual void SetAggro(bool yes)
    {
        currentState = (yes?Aggro:Passive);
    }

    void Update()
    {
        LookAtPlayer();
        LookAtTarget();
    }

    void LookAtPlayer()
    {
        if(overrideLook || enemy.stunned)
            return;
        
        targetDirection = (enemy.targetPlayer.trailPosition+(enemy.targetPlayer.position - enemy.targetPlayer.trailPosition) * Lead - enemy.Head.position).normalized;
        lookAtSpeed = currentState.rotateSpeed;
    }
    void LookAtTarget()
    {
        if(enemy.stunned)
            return;

        currentDirection = Vector3.RotateTowards(currentDirection, targetDirection, lookAtSpeed * Time.deltaTime, 0f);
        enemy.Head.rotation = Quaternion.LookRotation(currentDirection, transform.up);
        enemy.Animation.SetLook(currentDirection);
    }

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

    public void ForceLookAt(Vector3 point, bool force = true, float speed = 0f)
    {
        overrideLook = force;
        if(!force)
            return;

        targetDirection = point - enemy.Head.position;
        if(speed <= 0)
            speed = ForceLookSpeed;
        lookAtSpeed = speed;
    }
}

[System.Serializable]
public struct EnemyState
{
    public float desiredDistance, rotateSpeed;
}