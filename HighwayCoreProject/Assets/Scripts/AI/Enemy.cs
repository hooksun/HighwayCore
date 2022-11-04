using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float DesiredDistance, MaxHeight, WalkSpeed, JumpHeight, JumpGravity, FallGravity, JumpDistance, MaxJumpDistance, idleTime;
    public int maxBacknForth = 1;
    public Vector3 transformOffset;
    public Transform targetPlayer;
    public EnemyManager manager;
    public EnemyState currentState;
    public PlatformAddress currentPlatform;
    public TransformPoint transformPosition;

    PlatformAddress targetPlatform, lastPlatform;
    TransformPoint targetPoint, jumpPoint;
    bool isJumping;
    int backnForth;
    
    void Update()
    {
        Simulate();
        if(isJumping)
        {
            transform.position += jumpVelocity * Time.deltaTime;
            return;
        }
        transform.position = transformPosition.worldPoint + transformOffset;
    }

    public void Activate()
    {
        
    }
    
    void Simulate()
    {
        if(transformPosition.transform == null || !transformPosition.transform.gameObject.activeInHierarchy)
        {
            Die();
            return;
        }
        
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
            if((transform.position - targetPlayer.position).sqrMagnitude > DesiredDistance * DesiredDistance)
                currentState = EnemyState.pathfinding;
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

    void Die()
    {
        print("ded");
        manager.ActiveEnemies.Remove(this);
        gameObject.SetActive(false);
    }

    bool idleing;
    IEnumerator Idleing()
    {
        idleing = true;
        yield return new WaitForSeconds(idleTime);
        idleing = false;
        currentState = EnemyState.pathfinding;
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
        transformPosition.point = Vector3.MoveTowards(transformPosition.point, targetPoint.point, WalkSpeed * Time.deltaTime);
    }

    protected virtual void FindNewPath()
    {
        float sqrDist = Mathf.Infinity;
        bool found = false;
        List<PlatformAddress> neighbours = manager.RequestPlatformNeighbours(currentPlatform, JumpDistance);
        for(int i = 0; i < neighbours.Count; i++)
        {
            PlatformAddress plat = neighbours[i];
            if(plat.platform.height - currentPlatform.platform.height > MaxHeight)
            {
                neighbours.RemoveAt(i);
                i--;
                continue;
            }
            float newSqrDist = (plat.ClosestPointTo(targetPlayer.position).worldPoint - targetPlayer.position).sqrMagnitude;
            if(newSqrDist < sqrDist)
            {
                if(lastPlatform.lane != null && lastPlatform.platform == plat.platform && backnForth >= maxBacknForth)
                    continue;
                found = true;
                sqrDist = newSqrDist;
                targetPlatform = plat;
            }
        }
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
            lastPlatform = currentPlatform;
            currentPlatform = targetPlatform;
            isJumping = false;
            FindNewPath();
            return;
        }

        Vector3 distance = jumpPoint.worldPoint - transform.position;
        distance.y = 0f;
        jumpVelocity = distance / jumpTime + Vector3.up * (jumpVelocity.y - (jumpVelocity.y > 0?JumpGravity:FallGravity) * Time.deltaTime);
        jumpTime -= Time.deltaTime;
    }
}

public enum EnemyState{idle, pathfinding, attack}

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
        point -= vehicle.transform.position;
        point += vehicle.transformOffset;
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