using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePathfinding : EnemyPathfinding
{
    public LineRenderer grapple;
    public float minGrappleJumpHeight, grappleSpeed, grappleGravity, grappleHeight;
    public Audio GrappleShoot, GrappleRetract;

    protected override float jumpGrav{get => (grappleLanded?grappleGravity:JumpGravity);}

    Vector3 grapplePos;
    float grapplingSpeed;
    bool grappling, grappleLanded;

    public override void Activate()
    {
        StopGrapple();
        grapple.enabled = false;
        base.Activate();
    }

    protected override void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        base.Update();
        if(grapple.enabled)
        {
            if(!grappling)
            {
                grapplePos = Vector3.MoveTowards(grapplePos, grapple.transform.position, grappleSpeed * Time.deltaTime);
                if(grapplePos == grapple.transform.position)
                {
                    grapple.enabled = false;
                    GrappleRetract.Stop();
                    return;
                }
            }
            grapple.SetPosition(0, grapple.transform.position);
            grapple.SetPosition(1, grapplePos);
        }
    }

    protected virtual void Grappling()
    {
        if(!grappling)
        {
            if(enemy.stunned || jumpPoint.worldPoint.y < transform.position.y - transformOffset.y)
                return;
            grapple.enabled = true;
            grapplePos = grapple.transform.position;
            grappling = true;
            grappleLanded = false;
            GrappleShoot.Play();
        }
        enemy.Attack.ForceLookAtPoint(jumpPoint.worldPoint);
        if(!grappleLanded)
        {
            grapplePos = Vector3.MoveTowards(grapplePos, jumpPoint.worldPoint, grappleSpeed * Time.deltaTime);
            if(grapplePos == jumpPoint.worldPoint)
            {
                grappleLanded = true;
                GrappleRetract.Play();
            }
            return;
        }
        if(!enemy.stunned && !isJumping)
        {
            InitiateJump(grappleHeight);
        }
    }

    protected override void Jump()
    {
        if(!grappling && jumpPoint.worldPoint.y - transformPosition.worldPoint.y <= minGrappleJumpHeight)
        {
            base.Jump();
            return;
        }
        Grappling();
    }

    protected override void Jumping()
    {
        base.Jumping();
        if(grappling)
        {
            if(airVelocity.y <= 0f)
            {
                StopGrapple();
                return;
            }
            grapplePos = jumpPoint.worldPoint;
            enemy.Attack.ForceLookAtPoint(jumpPoint.worldPoint);
        }
    }

    protected override void AirSimulate()
    {
        if(!isJumping && jumpPoint.transform != null)
        {
            Grappling();
        }
        if(enemy.stunned && grappleLanded)
        {
            float dist = (transform.position - jumpPoint.worldPoint).magnitude;
            base.AirSimulate();
            Vector3 diff = transform.position - jumpPoint.worldPoint;
            diff = Vector3.ClampMagnitude(diff, dist);
            transform.position = jumpPoint.worldPoint + diff;

            return;
        }
        base.AirSimulate();
    }

    protected override void Land()
    {
        StopGrapple();
        base.Land();
    }

    void StopGrapple()
    {
        grappling = false;
        grappleLanded = false;
        enemy.Attack.StopForceLook();
    }
}
