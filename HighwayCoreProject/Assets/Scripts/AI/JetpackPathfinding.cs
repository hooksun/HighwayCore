using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackPathfinding : EnemyPathfinding
{
    public float jetpackForce, launchTime, flyCooldown, jetpackCooldown, airTime;
    public Vector3 flyDistance, flySpeed, minPos, maxPos;
    
    float groundTime, flyTime;
    Vector3 targetFlyPos, targetFlyTime;
    bool jetpacking;

    public override void Activate()
    {
        base.Activate();
        groundTime = flyCooldown;
        flyTime = 0f;
        jetpacking = false;
    }

    protected override void Simulate()
    {
        if(!enemy.stunned && groundTime <= 0f)
        {
            Fly();
            return;
        }
        groundTime -= Time.deltaTime;
        
        base.Simulate();
    }

    protected override void AirSimulate()
    {
        if(jetpacking)
        {
            Flying();
            return;
        }
        if(!enemy.stunned && !isJumping && groundTime <= flyCooldown)
        {
            Fly();
            return;
        }
        base.AirSimulate();
    }

    protected virtual void Fly()
    {
        targetFlyPos = transform.position;
        targetFlyPos.y = Random.Range(minPos.y, maxPos.y);
        targetFlyTime.y = launchTime;
        flyTime = airTime;
        airVelocity.y = jetpackForce;
        isGrounded = false;
        jetpacking = true;
    }

    protected virtual void Flying()
    {
        if(!enemy.stunned && flyTime <= 0)
        {
            if(Physics.Raycast(transform.position, Vector3.down, out groundInfo, maxPos.y, GroundMask) && groundInfo.transform.gameObject.layer == 3)
            {
                targetPlatform = groundInfo.transform.GetComponent<Vehicle>().ClosestPlatform(transform.position);
                jumpPoint = targetPlatform.ClosestPointTo(transform.position);
                float height = transform.position.y - transformOffset.y - jumpPoint.worldPoint.y;
                jumpTime = (2f * height / Mathf.Sqrt(2f * FallGravity * height));
                airVelocity = Vector3.zero;
                jetpacking = false;
                isJumping = true;
                groundTime = jetpackCooldown + flyCooldown;
                return;
            }
        }
        flyTime -= Time.deltaTime;

        //fly logic

        if(enemy.stunned)
        {
            transform.position += airVelocity * Time.deltaTime;
            return;
        }
        Vector3 newPos = transform.position;
        for(int i = 0; i < 3; i++)
        {
            if(Mathf.Abs(newPos[i] - targetFlyPos[i]) < .2f)
            {
                airVelocity[i] = 0f;
                float min = targetFlyPos[i] - flyDistance[i];
                float max = targetFlyPos[i] + flyDistance[i];
                float pPos = 0f;
                if(i == 2)
                    pPos = enemy.targetPlayer.position.z;
                min = Mathf.Min(max, Mathf.Max(min, pPos+minPos[i]));
                max = Mathf.Max(min, Mathf.Min(max, pPos+maxPos[i]));
                float random = Random.Range(min, max);
                targetFlyTime[i] = Mathf.Abs(targetFlyPos[i] - random) / flySpeed[i];
                targetFlyPos[i] = random;
            }

            float refVel = airVelocity[i];
            newPos[i] = Mathf.SmoothDamp(newPos[i], targetFlyPos[i], ref refVel, targetFlyTime[i]);
            airVelocity[i] = refVel;
        }
        transform.position = newPos;
    }
}