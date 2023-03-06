using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackPathfinding : EnemyPathfinding
{
    public Jetpack jetpack;
    public float jetpackForce, launchTime, flyCooldown, jetpackCooldown, airTime, jetpackStunTime, jetpackJumpGravity, minJetpackJumpHeight, jetpackTiltMulti;
    public string jetpackAnimation;
    public Vector3 flyDistance, flySpeed, minPos, maxPos;
    
    float groundTime, flyTime;
    Vector3 targetFlyPos, targetFlyTime;
    bool jetpacking, jetpackJump;

    protected override float jumpGrav{get => (jetpackJump?jetpackJumpGravity:JumpGravity);}
    protected override string jumpAnim{get => (jetpackJump||longJump?jetpackAnimation:base.jumpAnim);}

    public override void Activate()
    {
        groundTime = flyCooldown;
        flyTime = 0f;
        jetpacking = false;
        jetpackJump = false;
        base.Activate();
        jetpack.Activate();
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
        if(!enemy.stunned && jetpacking)
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

    public override void Stun(Vector3 knockback)
    {
        if(jetpacking)
            enemy.stunTime = jetpackStunTime;
        jetpackJump = false;
        jetpack.Enable(false);
        base.Stun(knockback);
    }
    public override void StopStun()
    {
        jetpacking = false;
    }

    protected override void Jump()
    {
        jetpackJump = jumpPoint.worldPoint.y - transformPosition.worldPoint.y > minJetpackJumpHeight;
        base.Jump();
    }

    protected override void Jumping()
    {
        base.Jumping();
        jetpack.Enable((jetpackJump && airVelocity.y > 0f) || tilt != Vector3.up);
    }

    protected override void Land()
    {
        base.Land();
        if(jetpacking)
        {
            groundTime = jetpackCooldown + flyCooldown;
            StopFly();
        }
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
        jetpack.Enable(true);
        enemy.Animation.Play(jetpackAnimation, 0, JumpFadeTime);
    }

    protected virtual void StopFly()
    {
        jetpacking = false;
        jetpack.Enable(false);
        tilt = Vector3.up;
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
                isJumping = true;
                groundTime = jetpackCooldown + flyCooldown;
                StopFly();
                return;
            }
        }
        flyTime -= Time.deltaTime;

        //fly logic

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
        tilt = (Vector3.up + (airVelocity * jetpackTiltMulti)).normalized;
    }
}