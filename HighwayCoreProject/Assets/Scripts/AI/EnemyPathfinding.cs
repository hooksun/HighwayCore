using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : EnemyBehaviour
{
    public float DesiredDistance, MaxHeight, WalkSpeed, JumpHeight, JumpGravity, FallGravity, JumpDistance, MaxJumpDistance;
    public float groundDist, groundStunResistance, groundCheckCooldown, velocityStunMulti, idleTime;
    public int maxBacknForth = 1;
    public Vector3 transformOffset;
    public LayerMask GroundMask;

    public PlatformAddress currentPlatform;
    public TransformPoint transformPosition;

    protected virtual float jumpGrav{get => JumpGravity;}
    protected virtual float fallGrav{get => FallGravity;}

    protected PlatformAddress targetPlatform, lastPlatform;
    protected TransformPoint targetPoint, jumpPoint;
    protected Vector3 airVelocity;
    protected RaycastHit groundInfo;
    protected bool isGrounded = true;
    protected bool isJumping, groundCooldown;
    protected int backnForth;
    
    public override void Activate()
    {
        isGrounded = true;
        isJumping = false;
        groundCooldown = false;
        backnForth = 0;
        idleing = false;
        targetPlatform.lane = null;
        lastPlatform.lane = null;
        FindNewPath();
    }

    protected virtual void Update()
    {
        if(transformPosition.transform == null || !transformPosition.transform.gameObject.activeInHierarchy)
        {
            enemy.Die();
            return;
        }
        if(isGrounded)
            Simulate();
        if(!isGrounded)
        {
            AirSimulate();
            return;
        }
        transform.position = transformPosition.worldPoint + transformOffset;
        if(GroundCheck())
        {
            transform.position = groundInfo.point + transformOffset;
        }
    }

    protected virtual bool GroundCheck() => Physics.Raycast(transform.position, Vector3.down, out groundInfo, groundDist, GroundMask);
    
    protected virtual void Simulate()
    {
        if(idleing || enemy.stunned)
            return;
        if((transform.position - enemy.targetPlayer.position).sqrMagnitude < DesiredDistance * DesiredDistance)
            return;
        
        PathFind();
    }

    protected virtual void AirSimulate()
    {
        if(transform.position.y < -100f)
        {
            enemy.Die();
            return;
        }

        if(isJumping)
        {
            Jumping();
        }

        airVelocity.y -= (airVelocity.y > 0?jumpGrav:fallGrav) * Time.deltaTime;
        if(airVelocity.y <= 0 && GroundCheck() && transform.position.y - groundInfo.point.y <= transformOffset.y)
        {
            if(!groundCooldown)
            {
                Land();
                return;
            }
            airVelocity.y = 0f;
        }

        transform.position += airVelocity * Time.deltaTime;
    }

    public override void Stun(Vector3 knockback)
    {
        if(isGrounded)
        {
            knockback *= 1f - groundStunResistance;
            isGrounded = false;
            StartCoroutine(GroundCooldown());
        }
        airVelocity *= velocityStunMulti;
        airVelocity += knockback;
        isJumping = false;
    }
    IEnumerator GroundCooldown()
    {
        groundCooldown = true;
        yield return new WaitForSeconds(groundCheckCooldown);
        groundCooldown = false;
    }

    bool idleing;
    IEnumerator Idleing()
    {
        idleing = true;
        yield return new WaitForSeconds(idleTime);
        idleing = false;
        FindNewPath();
    }

    protected virtual void PathFind()
    {
        if(targetPoint.transform == null)
        {
            FindNewPath();
            return;
        }
        
        if(transformPosition.point == targetPoint.point && transformPosition.transform == targetPoint.transform)
        {
            if(targetPlatform.lane == null || targetPlatform.platform == currentPlatform.platform || jumpPoint.transform == null)
            {
                StartCoroutine(Idleing());
                return;
            }
            
            Jump();
            return;
        }

        //movetowards logic
        transformPosition.point = Vector3.MoveTowards(transformPosition.point, targetPoint.point, WalkSpeed * Time.deltaTime);
    }

    protected virtual void FindNewPath()
    {
        float sqrDist = Mathf.Infinity;
        bool found = false;
        List<PlatformAddress> neighbours = enemy.manager.RequestPlatformNeighbours(currentPlatform, JumpDistance, MaxJumpDistance);
        for(int i = 0; i < neighbours.Count; i++)
        {
            PlatformAddress plat = neighbours[i];
            if(plat.platform.height - currentPlatform.platform.height > MaxHeight)
            {
                neighbours.RemoveAt(i);
                i--;
                continue;
            }
            float newSqrDist = (plat.ClosestPointTo(enemy.targetPlayer.position).worldPoint - enemy.targetPlayer.position).sqrMagnitude;
            if(newSqrDist < sqrDist)
            {
                if(lastPlatform.lane != null && lastPlatform.platform == plat.platform && backnForth >= maxBacknForth)
                    continue;
                found = true;
                sqrDist = newSqrDist;
                targetPlatform = plat;
            }
        }
        isGrounded = true;
        SetTargetPoint();
        if(found && lastPlatform.lane != null && lastPlatform.platform == targetPlatform.platform)
        {
            backnForth++;
            return;
        }
        backnForth = 0;
    }

    protected virtual void SetTargetPoint()
    {
        if(targetPlatform.lane == null || targetPlatform.platform == currentPlatform.platform)
        {
            targetPoint = currentPlatform.ClosestPointTo(enemy.targetPlayer.position);
            jumpPoint.transform = null;
            return;
        }
        jumpPoint = targetPlatform.ClosestPointTo(transform.position);
        targetPoint = currentPlatform.ClosestPointTo(jumpPoint.worldPoint);
    }

    protected virtual void Jump()
    {
        Vector3 jumpWorldPoint = jumpPoint.worldPoint;
        jumpWorldPoint.y = transform.position.y;
        float sqrDist = (jumpWorldPoint - transform.position).sqrMagnitude;
        if(sqrDist > MaxJumpDistance * MaxJumpDistance)
        {
            FindNewPath();
            return;
        }
        
        isJumping = true;
        isGrounded = false;
        airVelocity = Vector3.zero;
        CalculateJump(currentPlatform.platform.height, targetPlatform.platform.height, JumpHeight, JumpGravity, FallGravity);
        transformPosition.transform = jumpPoint.transform;
        if(Mathf.Sqrt(sqrDist) / jumpTime > WalkSpeed)
        {
            //jump animation
        }
    }

    protected void CalculateJump(float h1, float h2, float jumpH, float jumpG, float fallG)
    {
        float height1 = Mathf.Max(h1, h2) + jumpH - h1;
        float height2 = height1 + h1 - h2;
        airVelocity.y = Mathf.Sqrt(2f * jumpG * height1);
        float fallVelY = Mathf.Sqrt(2f * fallG * height2);
        jumpTime = (2f * height1 / airVelocity.y) + (2f * height2 / fallVelY);
    }

    protected virtual void Land()
    {
        if(groundInfo.transform.gameObject.layer == 8)
        {
            enemy.Die();
            return;
        }
        
        lastPlatform = currentPlatform;
        currentPlatform = targetPlatform;
        if(!isJumping || transformPosition.transform != groundInfo.transform)
        {
            currentPlatform = groundInfo.transform.GetComponent<Vehicle>().ClosestPlatform(transform.position);
            transformPosition.transform = groundInfo.transform;
        }
        transformPosition.point = transform.position - transformOffset - transformPosition.transform.position;
        isJumping = false;
        isGrounded = true;
        FindNewPath();
    }

    protected float jumpTime;
    protected virtual void Jumping()
    {
        if(jumpTime > 0)
        {
            Vector3 distance = jumpPoint.worldPoint - transform.position;
            distance.y = 0f;
            airVelocity = distance / jumpTime + Vector3.up * airVelocity.y;
            jumpTime -= Time.deltaTime;
        }
    }
}

public struct PlatformAddress
{
    public Lane lane;
    public Vehicle vehicle;
    public int platformIndex;

    public PlatformAddress(Lane _lane, Vehicle veh, int platI)
    {
        lane = _lane;
        vehicle = veh;
        platformIndex = platI;
    }

    public int laneIndex{get => lane.transform.GetSiblingIndex();}
    public int vehicleIndex{get => vehicle.transform.GetSiblingIndex();}
    public Platform platform{get => vehicle.Platforms[platformIndex];}

    public TransformPoint ClosestPointTo(Vector3 point)
    {
        point += vehicle.transformOffset - vehicle.transform.position;
        point.x = Mathf.Clamp(point.x, platform.BoundsStart.x, platform.BoundsEnd.x);
        point.z = Mathf.Clamp(point.z, platform.BoundsStart.y, platform.BoundsEnd.y);
        point -= vehicle.transformOffset;
        point.y = platform.height - vehicle.transformOffset.y;
        return new TransformPoint(vehicle.transform, point);
    }
}

public struct TransformPoint
{
    public Transform transform;
    public Vector3 point;

    public TransformPoint(Transform _transform, Vector3 _point)
    {
        transform = _transform;
        point = _point;
    }

    public Vector3 worldPoint => transform.position + point;
}