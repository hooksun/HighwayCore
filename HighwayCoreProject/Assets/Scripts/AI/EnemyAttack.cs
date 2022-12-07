using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyBehaviour
{
    public float MaxRotateSpeed, Lead, ForceLookSpeed, MaxAttackDistance, desiredDistance;
    public EnemyState Aggro, Passive;

    float lookAtSpeed;
    Vector3 currentDirection, targetDirection;
    bool overrideLook;

    public override void Activate()
    {
        currentDirection = Vector3.forward;
        targetDirection = Vector3.forward;
        overrideLook = false;
    }

    void Update()
    {
        LookAtPlayer();
        LookAtTarget();
    }

    void LookAtPlayer()
    {
        if(overrideLook)
            return;
        
        targetDirection = (enemy.targetPlayer.trailPosition+(enemy.targetPlayer.position - enemy.targetPlayer.trailPosition) * Lead - enemy.Head.position).normalized;
        lookAtSpeed = MaxRotateSpeed;
    }
    void LookAtTarget()
    {
        currentDirection = Vector3.RotateTowards(currentDirection, targetDirection, lookAtSpeed * Time.deltaTime, 0f);
        enemy.Head.rotation = Quaternion.LookRotation(currentDirection);
    }

    public virtual PlatformAddress PickPlatform(List<PlatformAddress> neighbours, PlatformAddress current)
    {
        if(neighbours == null || neighbours.Count == 0)
            return current;

        List<PlatformAddress> closer = new List<PlatformAddress>();
        float currentDist = Mathf.Abs(current.DistanceFrom(enemy.targetPlayer.position) - desiredDistance);
        for(int i = 0; i < neighbours.Count; i++)
        {
            float newDist = Mathf.Abs(neighbours[i].DistanceFrom(enemy.targetPlayer.position) - desiredDistance);
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

public struct EnemyState
{
    public float desiredDistance;
}