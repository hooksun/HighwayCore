using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePathfinding : EnemyPathfinding
{
    public LineRenderer grapple;
    public float minGrappleJumpHeight, grappleSpeed, grappleGravity, grappleHeight;

    protected override float jumpGrav{get => (grappleLanded?grappleGravity:JumpGravity);}

    Vector3 grapplePos;
    float grapplingSpeed;
    bool grappling, grappleLanded;

    public override void Activate()
    {
        base.Activate();
        grapple.enabled = false;
        grappling = false;
        grappleLanded = false;
    }

    protected override void Update()
    {
        base.Update();
        if(grappling)
        {
            grapple.SetPosition(0, grapple.transform.position);
            grapple.SetPosition(1, grapplePos);
        }
    }

    protected virtual void Grappling()
    {
        if(!grappling)
        {
            if(enemy.stunned)
                return;
            grapple.enabled = true;
            grapplePos = grapple.transform.position;
            grappling = true;
            grappleLanded = false;
        }
        if(!grappleLanded)
        {
            grapplePos = Vector3.MoveTowards(grapplePos, jumpPoint.worldPoint, grappleSpeed * Time.deltaTime);
            if(grapplePos == jumpPoint.worldPoint)
            {
                grappleLanded = true;
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
        grapple.enabled = false;
        grappling = false;
        grappleLanded = false;
        base.Land();
    }
}
