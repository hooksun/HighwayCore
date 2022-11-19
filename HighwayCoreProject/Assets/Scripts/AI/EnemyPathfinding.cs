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

    PlatformAddress targetPlatform, lastPlatform;
    TransformPoint targetPoint, jumpPoint;
    Vector3 airVelocity;
    RaycastHit groundInfo;
    bool isGrounded = true;
    bool isJumping, groundCooldown;
    int backnForth;
    
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

    void Update()
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

    bool GroundCheck() => Physics.Raycast(transform.position, Vector3.down, out groundInfo, groundDist, GroundMask);
    
    void Simulate()
    {
        if(idleing || enemy.stunned)
            return;
        if((transform.position - enemy.targetPlayer.position).sqrMagnitude < DesiredDistance * DesiredDistance)
            return;
        
        PathFind();
    }

    void AirSimulate()
    {
        if(isJumping)
        {
            Jumping();
        }
        if(!groundCooldown && airVelocity.y <= 0 && GroundCheck() && transform.position.y - groundInfo.point.y <= transformOffset.y)
        {
            if(groundInfo.transform.gameObject.layer == 8)
            {
                enemy.Die();
                return;
            }
            
            lastPlatform = currentPlatform;
            if(transformPosition.transform != groundInfo.transform)
            {
                currentPlatform = groundInfo.transform.GetComponent<Vehicle>().ClosestPlatform(transform.position);
                transformPosition.transform = groundInfo.transform;
            }
            else
                currentPlatform = targetPlatform;
            transformPosition.point = transform.position - transformOffset - transformPosition.transform.position;
            isJumping = false;
            isGrounded = true;
            FindNewPath();
            return;
        }

        airVelocity.y -= (airVelocity.y > 0?JumpGravity:FallGravity) * Time.deltaTime;
        transform.position += airVelocity * Time.deltaTime;

        if(transform.position.y < -100f)
            enemy.Die();
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

    void PathFind()
    {
        if(targetPoint.transform == null)
        {
            FindNewPath();
            return;
        }
        
        if(transformPosition.point == targetPoint.point)
        {
            if(targetPlatform.lane == null || targetPlatform.platform == currentPlatform.platform || jumpPoint.transform == null)
            {
                StartCoroutine(Idleing());
                return;
            }
            
            Vector3 jumpWorldPoint = jumpPoint.worldPoint;
            jumpWorldPoint.y = transform.position.y;
            if((jumpWorldPoint - transform.position).sqrMagnitude > MaxJumpDistance * MaxJumpDistance)
            {
                FindNewPath();
                return;
            }
            
            isJumping = true;
            isGrounded = false;
            airVelocity = Vector3.zero;
            float height1 = Mathf.Max(currentPlatform.platform.height, targetPlatform.platform.height) + JumpHeight - currentPlatform.platform.height;
            float height2 = height1 + currentPlatform.platform.height - targetPlatform.platform.height;
            airVelocity.y = Mathf.Sqrt(2f * JumpGravity * height1);
            float fallVelY = Mathf.Sqrt(2f * FallGravity * height2);
            jumpTime = (2f * height1 / airVelocity.y) + (2f * height2 / fallVelY);
            transformPosition.transform = jumpPoint.transform;
            return;
        }

        //movetowards logic
        transformPosition.point = Vector3.MoveTowards(transformPosition.point, targetPoint.point, WalkSpeed * Time.deltaTime);
    }

    protected virtual void FindNewPath()
    {
        float sqrDist = Mathf.Infinity;
        bool found = false;
        List<PlatformAddress> neighbours = enemy.manager.RequestPlatformNeighbours(currentPlatform, JumpDistance);
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

    float jumpTime;
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