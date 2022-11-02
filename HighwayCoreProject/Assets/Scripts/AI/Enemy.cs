using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float DesiredDistance, MaxHeight, WalkSpeed, JumpHeight, JumpGravity, FallGravity;
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
        List<PlatformAddress> neighbours = manager.RequestPlatformNeighbours(currentPlatform);
        //pick one to be the targetPlatform
        //set targetPoint, jumpPoint

    }

    Vector3 jumpVelocity;
    float jumpTime;
    protected virtual void Jumping()
    {
        if(jumpTime < 0)
        {
            transformPosition = jumpPoint;
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

    public Vector3 worldPoint => transform.TransformPoint(point);
}