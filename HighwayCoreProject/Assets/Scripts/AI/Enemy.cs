using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float DesiredDistance, MaxHeight, WalkSpeed, JumpHeight, JumpGravity, FallGravity, MaxJumpDistance, idleTime;
    public Vector3 transformOffset;
    public Transform targetPlayer;
    public EnemyManager manager;
    public EnemyState currentState;
    public PlatformAddress currentPlatform;
    public TransformPoint transformPosition;

    PlatformAddress targetPlatform;
    TransformPoint targetPoint, jumpPoint;
    bool isJumping;
    
    void FixedUpdate()
    {
        Simulate();
        if(isJumping)
        {
            transform.position += jumpVelocity * Time.fixedDeltaTime;
            return;
        }
        transform.position = transformPosition.worldPoint + transformOffset;
    }
    
    void Simulate()
    {
        if(isJumping)
        {
            Jumping();
            return;
        }
        if((transform.position - targetPlayer.position).sqrMagnitude <= DesiredDistance * DesiredDistance)
        {
            currentState = EnemyState.attack;
        }
        if(currentState == EnemyState.attack)
        {

            return;
        }
        if(currentState == EnemyState.pathfinding)
        {
            PathFind();
            return;
        }
        if(currentState == EnemyState.idle)
        {
            if(!idleing)
                StartCoroutine(Idleing());
            return;
        }
    }

    bool idleing;
    IEnumerator Idleing()
    {
        idleing = true;
        yield return new WaitForSeconds(idleTime);
        idleing = false;
        currentState = EnemyState.pathfinding;
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
            if(targetPlatform.lane == null || targetPlatform.platform == currentPlatform.platform|| jumpPoint.transform == null)
            {
                currentState = EnemyState.idle;
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
            jumpVelocity = Vector3.zero;
            float height1 = Mathf.Max(currentPlatform.platform.height, targetPlatform.platform.height) + JumpHeight - currentPlatform.platform.height;
            float height2 = height1 + currentPlatform.platform.height - targetPlatform.platform.height;
            jumpVelocity.y = Mathf.Sqrt(2f * JumpGravity * height1);
            float fallVelY = Mathf.Sqrt(2f * FallGravity * height2);
            jumpTime = (2f * height1 / jumpVelocity.y) + (2f * height2 / fallVelY);
            return;
        }

        //movetowards logic
        transformPosition.point = Vector3.MoveTowards(transformPosition.point, targetPoint.point, WalkSpeed * Time.fixedDeltaTime);
    }

    protected virtual void FindNewPath()
    {
        float sqrDist = Mathf.Infinity;
        List<PlatformAddress> neighbours = manager.RequestPlatformNeighbours(currentPlatform);
        for(int i = 0; i < neighbours.Count; i++)
        {
            PlatformAddress plat = neighbours[i];
            if(Mathf.Abs(currentPlatform.platform.height - plat.platform.height) > MaxHeight)
            {
                neighbours.RemoveAt(i);
                i--;
                continue;
            }
            float newSqrDist = (plat.ClosestPointTo(targetPlayer.position).worldPoint - targetPlayer.position).sqrMagnitude;
            if(newSqrDist < sqrDist)
            {
                sqrDist = newSqrDist;
                targetPlatform = plat;
            }
        }
        SetTargetPoint();
    }

    protected virtual void SetTargetPoint()
    {
        if(targetPlatform.lane == null || targetPlatform.platform == currentPlatform.platform)
        {
            targetPoint = currentPlatform.ClosestPointTo(targetPlayer.position);
            return;
        }
        jumpPoint = targetPlatform.ClosestPointTo(transform.position);
        targetPoint = currentPlatform.ClosestPointTo(jumpPoint.worldPoint);
    }

    Vector3 jumpVelocity;
    float jumpTime;
    protected virtual void Jumping()
    {
        if(jumpTime < 0)
        {
            transformPosition = jumpPoint;
            currentPlatform = targetPlatform;
            isJumping = false;
            FindNewPath();
            return;
        }

        Vector3 distance = jumpPoint.worldPoint - transform.position;
        distance.y = 0f;
        jumpVelocity = distance / jumpTime;
        jumpTime -= Time.fixedDeltaTime;
    }
}

public enum EnemyState{idle, pathfinding, attack}

public struct PlatformAddress
{
    public Lane lane;
    public int laneIndex, vehicleIndex, platformIndex;

    public PlatformAddress(Lane _lane, int laneI, int vehI, int platI)
    {
        lane = _lane;
        laneIndex = laneI;
        vehicleIndex = vehI;
        platformIndex = platI;
    }

    public Vehicle vehicle{get => lane.Vehicles[vehicleIndex];}
    public Platform platform{get => vehicle.Platforms[platformIndex];}

    public TransformPoint ClosestPointTo(Vector3 point)
    {
        Vector2 boundsStart = new Vector3(platform.BoundsStart.x, 0f, platform.BoundsStart.y) + vehicle.transform.position - vehicle.transformOffset;
        Vector2 boundsEnd = new Vector3(platform.BoundsEnd.x, 0f, platform.BoundsEnd.y) + vehicle.transform.position - vehicle.transformOffset;
        point.x = Mathf.Clamp(point.x, boundsStart.x, boundsEnd.x);
        point.z = Mathf.Clamp(point.z, boundsStart.y, boundsEnd.y);

        point -= vehicle.transform.position;
        point.y = vehicle.transform.position.y + platform.height - vehicle.transformOffset.y;
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